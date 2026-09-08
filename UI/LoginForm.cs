using ascendedAuth_v2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ascendedAuth_v2.UI
{
    public partial class LoginForm : Form
    {
        private ParticleBackgroundControl _background;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 0x0002;

        public static api KeyAuthApp = new api(
            name: "ascendedAuth",
            ownerid: "Y4SNEVMdoD",
            secret: "c01f0bfc9022e7d4fd5822de7abf5812f242bcef45d449f4a5b8fc736a158ccc",
            version: "1.0"
        );

        public LoginForm()
        {
            InitializeComponent();
            Icon = CreateAppIcon();
            ShowIcon = true;
            KeyAuthApp.init();
            LoadStoredKey();
            SetupParticles();
        }

        private static Icon CreateAppIcon()
        {
            Bitmap bmp = new Bitmap(256, 256);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Black);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (SolidBrush brush = new SolidBrush(Color.Red))
                    {
                        float r = 14f;
                        float x = (bmp.Width - r) / 2f;
                        float y = (bmp.Height - r) / 2f;
                        g.FillEllipse(brush, x, y, r, r);
                    }
                }

                IntPtr hIcon = bmp.GetHicon();
                try
                {
                    return (Icon)Icon.FromHandle(hIcon).Clone();
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
            finally
            {
                bmp.Dispose();
            }
        }

        private void SetupParticles()
        {
            if (_background != null) return;
            _background = new ParticleBackgroundControl();
            _background.Dock = DockStyle.Fill;
            Controls.Add(_background);
            _background.SendToBack();

            MouseDown += LoginForm_MouseDown;
            _background.MouseDown += LoginForm_MouseDown;
            lczxy7Label1.MouseDown += LoginForm_MouseDown;
            status.MouseDown += LoginForm_MouseDown;
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void LoadStoredKey()
        {
            string storedKey = ascendedAuth_v2.Properties.Settings.Default.LicenseKey;
            if (!string.IsNullOrWhiteSpace(storedKey))
            {
                Username.Text = storedKey;
            }
        }

        private async void lczxy7AnimatedButton11_Click(object sender, EventArgs e)
        {
            string licenseKey = Username.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(licenseKey) || licenseKey == "KEY:")
            {
                status.Text = "Status: Digite a chave!";
                return;
            }

            status.Text = "Status: Validando...";

            KeyAuthApp.license(licenseKey);

            if (KeyAuthApp.response.success)
            {
                // Store the key on successful login
                ascendedAuth_v2.Properties.Settings.Default.LicenseKey = licenseKey;
                ascendedAuth_v2.Properties.Settings.Default.Save();

                status.Text = "Status: Acesso liberado!";
                await Task.Delay(800);
                
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                status.Text = "Status: " + KeyAuthApp.response.message;
                await Task.Delay(7000);
                status.Text = "Status: Tente novamente";
            }
        }

        // FECHAR PAINEL
        private void lczxy7AnimatedButton12_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private sealed class ParticleBackgroundControl : Control
        {
            private sealed class Particle
            {
                public float X;
                public float Y;
                public float Vx;
                public float Vy;
                public float Radius;
            }

            private readonly List<Particle> _particles = new List<Particle>();
            private readonly Random _random = new Random();
            private readonly Timer _timer;
            private Point _mousePosition;
            private bool _mouseInside;
            private bool _seeded;
            private int _lastWidth;
            private int _lastHeight;

            public ParticleBackgroundControl()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                TabStop = false;

                _timer = new Timer();
                _timer.Interval = 16;
                _timer.Tick += (s, e) =>
                {
                    StepParticles();
                    Invalidate();
                };
                _timer.Start();

                Resize += (s, e) => EnsureParticles();
                MouseMove += (s, e) =>
                {
                    _mousePosition = e.Location;
                    _mouseInside = true;
                };
                MouseLeave += (s, e) => _mouseInside = false;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_timer != null) _timer.Stop();
                    if (_timer != null) _timer.Dispose();
                }
                base.Dispose(disposing);
            }

            private void EnsureParticles()
            {
                if (Width <= 0 || Height <= 0) return;

                int targetCount = Math.Max(90, Math.Min(220, (Width * Height) / 5500));

                if (!_seeded || _lastWidth <= 0 || _lastHeight <= 0)
                {
                    _particles.Clear();
                    for (int i = 0; i < targetCount; i++)
                    {
                        _particles.Add(CreateParticle());
                    }
                    _seeded = true;
                    _lastWidth = Width;
                    _lastHeight = Height;
                    return;
                }

                _lastWidth = Width;
                _lastHeight = Height;

                while (_particles.Count < targetCount)
                {
                    _particles.Add(CreateParticle());
                }
                while (_particles.Count > targetCount)
                {
                    _particles.RemoveAt(_particles.Count - 1);
                }

                for (int i = 0; i < _particles.Count; i++)
                {
                    var p = _particles[i];
                    if (p.X < 0 || p.X > Width || p.Y < 0 || p.Y > Height)
                    {
                        p.X = (float)(_random.NextDouble() * Width);
                        p.Y = (float)(_random.NextDouble() * Height);
                    }
                }
            }

            private Particle CreateParticle()
            {
                float radius = (float)(_random.NextDouble() * 1.8 + 1.4);
                float speed = (float)(_random.NextDouble() * 0.6 + 0.2);
                double angle = _random.NextDouble() * Math.PI * 2;

                return new Particle
                {
                    X = (float)(_random.NextDouble() * Width),
                    Y = (float)(_random.NextDouble() * Height),
                    Vx = (float)(Math.Cos(angle) * speed),
                    Vy = (float)(Math.Sin(angle) * speed),
                    Radius = radius
                };
            }

            private void StepParticles()
            {
                if (Width <= 0 || Height <= 0) return;

                if (!_seeded)
                {
                    EnsureParticles();
                }

                float repelRadius = 130f;
                float repelStrength = 0.085f;
                float minSpeed = 0.35f;
                float maxSpeed = 2.2f;
                float wander = 0.018f;

                foreach (var p in _particles)
                {
                    p.Vx += (float)((_random.NextDouble() - 0.5) * wander);
                    p.Vy += (float)((_random.NextDouble() - 0.5) * wander);

                    if (_mouseInside)
                    {
                        float dx = p.X - _mousePosition.X;
                        float dy = p.Y - _mousePosition.Y;
                        float dist2 = dx * dx + dy * dy;
                        if (dist2 > 1f && dist2 < repelRadius * repelRadius)
                        {
                            float dist = (float)Math.Sqrt(dist2);
                            float force = (repelRadius - dist) * repelStrength;
                            p.Vx += (dx / dist) * force;
                            p.Vy += (dy / dist) * force;
                        }
                    }

                    float speed2 = p.Vx * p.Vx + p.Vy * p.Vy;
                    if (speed2 > 0.0001f)
                    {
                        float speed = (float)Math.Sqrt(speed2);
                        if (speed < minSpeed)
                        {
                            float scale = minSpeed / speed;
                            p.Vx *= scale;
                            p.Vy *= scale;
                        }
                        else if (speed > maxSpeed)
                        {
                            float scale = maxSpeed / speed;
                            p.Vx *= scale;
                            p.Vy *= scale;
                        }
                    }
                    else
                    {
                        double angle = _random.NextDouble() * Math.PI * 2;
                        p.Vx = (float)Math.Cos(angle) * minSpeed;
                        p.Vy = (float)Math.Sin(angle) * minSpeed;
                    }

                    p.X += p.Vx;
                    p.Y += p.Vy;

                    if (p.X < 0) { p.X = 0; p.Vx = -p.Vx; }
                    if (p.Y < 0) { p.Y = 0; p.Vy = -p.Vy; }
                    if (p.X > Width) { p.X = Width; p.Vx = -p.Vx; }
                    if (p.Y > Height) { p.Y = Height; p.Vy = -p.Vy; }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.Clear(Color.Black);

                if (!_seeded)
                {
                    EnsureParticles();
                }

                using (var dotBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
                using (var linePen = new Pen(Color.FromArgb(55, 255, 255, 255), 1f))
                using (var mousePen = new Pen(Color.FromArgb(70, 255, 0, 0), 1f))
                {
                    float connectDist = 120f;
                    float connectDist2 = connectDist * connectDist;
                    float mouseLink = 140f;
                    float mouseLink2 = mouseLink * mouseLink;

                    for (int i = 0; i < _particles.Count; i++)
                    {
                        var a = _particles[i];

                        for (int j = i + 1; j < _particles.Count; j++)
                        {
                            var b = _particles[j];
                            float dx = a.X - b.X;
                            float dy = a.Y - b.Y;
                            float d2 = dx * dx + dy * dy;
                            if (d2 <= connectDist2)
                            {
                                float t = 1f - (float)Math.Sqrt(d2) / connectDist;
                                linePen.Color = Color.FromArgb((int)(55 * t), 255, 255, 255);
                                e.Graphics.DrawLine(linePen, a.X, a.Y, b.X, b.Y);
                            }
                        }

                        if (_mouseInside)
                        {
                            float mdx = a.X - _mousePosition.X;
                            float mdy = a.Y - _mousePosition.Y;
                            float md2 = mdx * mdx + mdy * mdy;
                            if (md2 <= mouseLink2)
                            {
                                float t = 1f - (float)Math.Sqrt(md2) / mouseLink;
                                int alpha = Math.Min(255, Math.Max(90, (int)(160 + 95 * t)));
                                int g = Math.Min(255, Math.Max(0, (int)(255 * (1f - t))));
                                int b = g;
                                dotBrush.Color = Color.FromArgb(alpha, 255, g, b);
                                mousePen.Color = Color.FromArgb((int)(90 * t), 255, 0, 0);
                                e.Graphics.DrawLine(mousePen, a.X, a.Y, _mousePosition.X, _mousePosition.Y);
                            }
                            else
                            {
                                dotBrush.Color = Color.FromArgb(200, 255, 255, 255);
                            }
                        }
                        else
                        {
                            dotBrush.Color = Color.FromArgb(200, 255, 255, 255);
                        }

                        e.Graphics.FillEllipse(dotBrush, a.X - a.Radius, a.Y - a.Radius, a.Radius * 2, a.Radius * 2);
                    }
                }
            }
        }
    }
}
