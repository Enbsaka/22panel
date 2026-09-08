using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ascendedAuth_v2.Core;
using ascendedAuth_v2.Features;
using ascendedAuth_v2.Models;

namespace ascendedAuth_v2.UI
{
    public partial class MainForm : Form
    {
        public static bool Streaming;

        [DllImport("user32.dll")]
        public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_HOTKEY = 0x0312;
        private const int HotkeyId = 0x2237;
        private const uint VK_INSERT = 0x2D;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 0x0002;

        private readonly MemoryManager _memoryManager;
        private readonly Dictionary<string, FeatureBase> _features = new Dictionary<string, FeatureBase>();
        private const string TargetProcess = "HD-Player";
        private ParticleBackgroundControl _background;
        private bool _suppressToggleEvents;
        private Color? _previousShadowColor1;
        private Color? _previousShadowColor2;

        public MainForm()
        {
            InitializeComponent();
            Icon = CreateAppIcon();
            ShowIcon = true;
            _memoryManager = new MemoryManager(TargetProcess);
            InitializeFeatures();
            SetupUI();
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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RegisterHotKey(Handle, HotkeyId, 0, VK_INSERT);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UnregisterHotKey(Handle, HotkeyId);
            base.OnHandleDestroyed(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HotkeyId)
            {
                TogglePanelVisibility();
                return;
            }
            base.WndProc(ref m);
        }

        private void TogglePanelVisibility()
        {
            if (Visible)
            {
                Hide();
                return;
            }

            Show();
            WindowState = FormWindowState.Normal;
            BringToFront();
            Activate();
        }

        private void InitializeFeatures()
        {
            // DLL Features
            _features["Chams"] = new DllFeature("Menu Chams", "ascendedAuth_v2.MENU CHAMS 64 BITS.dll", "MENU CHAMS 64 BITS.dll", TargetProcess);
            _features["BandW_Chams"] = new DllFeature("B&W Chams", "ascendedAuth_v2.BLUE_AND_WHITE.dll", "BLUE_AND_WHITE.dll", TargetProcess);
            _features["BasicChams"] = new DllFeature("Basic Chams", "ascendedAuth_v2.transparent.dll", "transparent.dll", TargetProcess);
            _features["LinesESP"] = new DllFeature("Lines ESP", "ascendedAuth_v2.hakulines.dll", "hakulines.dll", TargetProcess);

            // Memory Scan Features (Aimbot)
            _features["AimbotOmbro"] = new MemoryScanFeature("Aimbot Ombro", _memoryManager, 
                "FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 A5 43", 
                0xE8, 0xA8);

            _features["AimbotRage"] = new MemoryScanFeature("Aimbot Rage", _memoryManager, 
                "FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? A5 43 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ??", 
                0xA4, 0xA8);

            _features["AimbotLegit"] = new MemoryScanFeature("Aimbot Legit", _memoryManager, 
                "FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? A5 43 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ??", 
                0xDC, 0xA8);

            // Memory Pattern Features (Pixel, Antena, Camera, Tracking)
            _features["PixelExtendido"] = new MemoryFeature("Pixel Extendido", _memoryManager, 
                new MemoryPattern("41 01 00 00 00 00 00 c0 3f 00 00 00 3f 00 00 80 3f", 
                                  "41 01 00 00 00 00 00 c0 9f 00 00 00 3f 00 00 80 3f"));

            _features["AntenaMale"] = new MemoryFeature("Antena Male", _memoryManager, 
                new MemoryPattern("f0 8c 99 33 00 00 80 3f", 
                                  "f0 8c 99 33 00 80 8d 43"));

            _features["NoRecoil"] = new MemoryFeature("No Recoil", _memoryManager,
                new MemoryPattern("01 EE 00 0A 81 EE 10 0A 10 EE 10 8C BD E8 00 00 7A 44 F0 48 2D E9 10 B0 8D E2 02 8B 2D ED 08 D0 4D E2 00 50 A0 E1",
                                  "01 EE 00 0A 81 EE 10 0A 10 EE 10 8C BD E8 00 00 7A FF F0 48 2D E9 10 B0 8D E2 02 8B 2D ED 08 D0 4D E2 00 50 A0 E1"));

            _features["CameraDireita"] = new MemoryFeature("Camera Direita", _memoryManager, 
                new MemoryPattern("00 00 00 00 00 00 00 00 00 00 00 00 00 80 3f 00 00 80 3f 00 00 80 3f 00 00 00 00 00 00 80 3f 00 00 00 00 00 00 00 00 00 00 80 bf 00 00 00 00 00 00 80 bf 00 00 00 00 00 00 00 00 00 00", 
                                  "00 00 00 00 00 00 00 00 00 00 00 00 00 80 3f 00 00 80 3f 00 00 80 3f 00 00 00 00 00 00 80 40 00 00 00 00 00 00 00 00 00 00 80 bf 00 00 00 00 00 00 80 bf 00 00 00 00 00 00 00 00 00 00"));

            _features["Tracking2x"] = new MemoryFeature("2x Tracking", _memoryManager, 
                new MemoryPattern("C0 3F 33 33 93 3F 8F C2 F5 3C CD CC CC 3D ?? 00 00 00 EC 51 B8 3D CD CC 4C 3F 00 00 00 00 00 00 A0 42 00 00 C0 3F 33 33 13 40 00 00 F0 3F 00 00 80 3F 01",
                                  "C0 3F 33 33 93 3F 8F C2 F5 3C CD CC CC 3D ?? 00 00 00 EC 51 B8 3D CD CC 4C 3F 00 00 00 00 00 00 A0 42 00 00 C0 3F 33 33 13 40 00 00 F0 3F 00 00 80 5C 01"));

        }

        private void SetupUI()
        {
            if (_background == null)
            {
                _background = new ParticleBackgroundControl();
                _background.Dock = DockStyle.Fill;
                Controls.Add(_background);
                _background.SendToBack();
            }

            panelAim.Visible = true;
            panelVisual.Visible = false;
            panelMisc.Visible = false;
            panelSafety.Visible = false;

            lczxy7CustomCheckBox20.Text = "Basic Chams";
            lczxy7CustomCheckBox19.Text = "B&W Chams";
            lczxy7CustomCheckBox21.Text = "Lines ESP";
            lczxy7CustomCheckBox22.Checked = false;
            lczxy7CustomCheckBox22.Enabled = true;

            btnAimTab.ForeColor = Color.White;
            btnVisualTab.ForeColor = Color.Gray;
            btnMiscTab.ForeColor = Color.Gray;
            btnSafetyTab.ForeColor = Color.Gray;

            MouseDown += MainForm_MouseDown;
            panelAim.MouseDown += MainForm_MouseDown;
            panelVisual.MouseDown += MainForm_MouseDown;
            panelMisc.MouseDown += MainForm_MouseDown;
            panelSafety.MouseDown += MainForm_MouseDown;
            lczxy7Label1.MouseDown += MainForm_MouseDown;
            _background.MouseDown += MainForm_MouseDown;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void ShowCategory(string category)
        {
            panelAim.Visible = category == "Aim";
            panelVisual.Visible = category == "Visual";
            panelMisc.Visible = category == "Misc";
            panelSafety.Visible = category == "Safety";
            
            btnAimTab.ForeColor = category == "Aim" ? Color.White : Color.Gray;
            btnVisualTab.ForeColor = category == "Visual" ? Color.White : Color.Gray;
            btnMiscTab.ForeColor = category == "Misc" ? Color.White : Color.Gray;
            btnSafetyTab.ForeColor = category == "Safety" ? Color.White : Color.Gray;

            if (_background != null) _background.SendToBack();

            btnAimTab.BringToFront();
            btnVisualTab.BringToFront();
            btnMiscTab.BringToFront();
            btnSafetyTab.BringToFront();
            lczxy7Label1.BringToFront();
            lczxy7AnimatedButton11.BringToFront();
            status.BringToFront();

            if (category == "Aim") panelAim.BringToFront();
            if (category == "Visual") panelVisual.BringToFront();
            if (category == "Misc") panelMisc.BringToFront();
            if (category == "Safety") panelSafety.BringToFront();
        }

        private void btnAimTab_Click(object sender, EventArgs e)
        {
            ShowCategory("Aim");
        }

        private void btnVisualTab_Click(object sender, EventArgs e)
        {
            ShowCategory("Visual");
        }

        private void btnMiscTab_Click(object sender, EventArgs e)
        {
            ShowCategory("Misc");
        }

        private void btnSafetyTab_Click(object sender, EventArgs e)
        {
            ShowCategory("Safety");
        }

        private async Task HandleFeatureToggle(string featureKey, bool enable, string activationMsg, string deactivationMsg)
        {
            if (!_features.ContainsKey(featureKey)) return;

            status.Text = enable ? "Ativando..." : "Desativando...";
            
            bool success = await _features[featureKey].Toggle(enable);

            if (success)
            {
                status.Text = enable ? activationMsg : deactivationMsg;
                if (enable) Console.Beep(400, 300);
            }
            else
            {
                status.Text = _memoryManager.LastError ?? ("Erro ao " + (enable ? "ativar " : "desativar ") + _features[featureKey].Name);
            }
        }

        // --- VISUAL CATEGORY ---
        private void lczxy7CustomCheckBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("Chams", lczxy7CustomCheckBox11.Checked, "Chams injetada!", "Chams desativada!");
        }

        private void lczxy7CustomCheckBox19_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("BandW_Chams", lczxy7CustomCheckBox19.Checked, "B&W Chams injetada!", "B&W Chams desativada!");
        }

        private void lczxy7CustomCheckBox20_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("BasicChams", lczxy7CustomCheckBox20.Checked, "Basic Chams injetada!", "Basic Chams desativada!");
        }

        private void lczxy7CustomCheckBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("LinesESP", lczxy7CustomCheckBox21.Checked, "Lines ESP injetada!", "Lines ESP desativada!");
        }

        // --- AIM CATEGORY ---
        private void Aimsholder_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("AimbotOmbro", Aimsholder.Checked, "Aimbot Ombro ativado", "Aimbot Ombro desativado");
        }

        private void lczxy7CustomCheckBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("AimbotRage", lczxy7CustomCheckBox12.Checked, "Aimbot Rage ativado", "Aimbot Rage desativado");
        }

        private void lczxy7CustomCheckBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("AimbotLegit", lczxy7CustomCheckBox13.Checked, "Aimbot Legit ativado", "Aimbot Legit desativado");
        }

        private void lczxy7CustomCheckBox110_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("NoRecoil", lczxy7CustomCheckBox110.Checked, "No Recoil ativado!", "No Recoil desativado!");
        }

        // --- MISC CATEGORY ---
        private void lczxy7CustomCheckBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("PixelExtendido", lczxy7CustomCheckBox14.Checked, "Pixel extendido ativado!", "Pixel extendido desativado!");
        }

        private void lczxy7CustomCheckBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("AntenaMale", lczxy7CustomCheckBox15.Checked, "Antena male ativada!", "Antena male desativada!");
        }

        private void lczxy7CustomCheckBox16_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("CameraDireita", lczxy7CustomCheckBox16.Checked, "Camera direita ativada!", "Camera direita desativada!");
        }

        private void lczxy7CustomCheckBox17_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;
            _ = HandleFeatureToggle("Tracking2x", lczxy7CustomCheckBox17.Checked, "2x Tracking ativado!", "2x Tracking desativado!");
        }

        private void lczxy7CustomCheckBox18_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;

            if (lczxy7CustomCheckBox18.Checked)
            {
                base.ShowInTaskbar = false;
                Streaming = true;
                ApplyStreamMode(true);
                status.Text = "Stream Mode Ativado!";
            }
            else
            {
                base.ShowInTaskbar = true;
                Streaming = false;
                ApplyStreamMode(false);
                status.Text = "Stream Mode Desativado!";
            }
        }

        private void ApplyStreamMode(bool enabled)
        {
            try
            {
                if (enabled)
                {
                    if (_previousShadowColor1 == null) _previousShadowColor1 = guna2BorderlessForm1.ShadowColor;
                    if (_previousShadowColor2 == null) _previousShadowColor2 = guna2BorderlessForm2.ShadowColor;

                    guna2BorderlessForm1.ShadowColor = Color.Transparent;
                    guna2BorderlessForm2.ShadowColor = Color.Transparent;
                    guna2BorderlessForm1.DockIndicatorTransparencyValue = 1D;
                    guna2BorderlessForm2.DockIndicatorTransparencyValue = 1D;

                    uint ok = SetWindowDisplayAffinity(Handle, 17U);
                    if (ok == 0)
                    {
                        SetWindowDisplayAffinity(Handle, 1U);
                    }
                }
                else
                {
                    SetWindowDisplayAffinity(Handle, 0U);

                    if (_previousShadowColor1 != null) guna2BorderlessForm1.ShadowColor = _previousShadowColor1.Value;
                    if (_previousShadowColor2 != null) guna2BorderlessForm2.ShadowColor = _previousShadowColor2.Value;
                    guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
                    guna2BorderlessForm2.DockIndicatorTransparencyValue = 0.6D;
                }
            }
            catch
            {
            }
        }

        private async void btnBypass_Click(object sender, EventArgs e)
        {
            await RunSafetyBypass();
        }

        //  BYPASS 50 PLAYERS
        private void lczxy7CustomCheckBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressToggleEvents) return;

            _suppressToggleEvents = true;
            lczxy7CustomCheckBox22.Checked = false;
            _suppressToggleEvents = false;
            status.Text = "Safety: Função não implementada.";
        }

        private async Task RunSafetyBypass()
        {
            try
            {
                _suppressToggleEvents = true;
                status.Text = "Safety: Desativando tudo...";

                var keys = _features.Keys.ToList();
                foreach (var key in keys)
                {
                    try
                    {
                        await _features[key].Toggle(false);
                    }
                    catch
                    {
                    }
                }

                Aimsholder.Checked = false;
                lczxy7CustomCheckBox12.Checked = false;
                lczxy7CustomCheckBox13.Checked = false;
                lczxy7CustomCheckBox11.Checked = false;
                lczxy7CustomCheckBox19.Checked = false;
                lczxy7CustomCheckBox20.Checked = false;
                lczxy7CustomCheckBox21.Checked = false;
                lczxy7CustomCheckBox14.Checked = false;
                lczxy7CustomCheckBox15.Checked = false;
                lczxy7CustomCheckBox16.Checked = false;
                lczxy7CustomCheckBox17.Checked = false;
                lczxy7CustomCheckBox18.Checked = false;

                base.ShowInTaskbar = true;
                Streaming = false;
                ApplyStreamMode(false);

                status.Text = "Safety: Limpando cache do painel...";
                SafeClearAppCache();

                status.Text = "Safety: Finalizando...";
                await Task.Delay(200);
            }
            finally
            {
                _suppressToggleEvents = false;
                Application.Exit();
            }
        }

        private void SafeClearAppCache()
        {
            try
            {
                ascendedAuth_v2.Properties.Settings.Default.LicenseKey = string.Empty;
                ascendedAuth_v2.Properties.Settings.Default.Save();
            }
            catch
            {
            }

            string[] dllNames =
            {
                "MENU CHAMS 64 BITS.dll",
                "BLUE_AND_WHITE.dll",
                "transparent.dll",
                "hakulines.dll"
            };

            string extractedDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "22");

            foreach (var name in dllNames)
            {
                TryDeleteFile(System.IO.Path.Combine(extractedDir, name));
            }
        }

        private static void TryDeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch
            {
            }
        }

        private void lczxy7AnimatedButton11_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
