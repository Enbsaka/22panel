namespace ascendedAuth_v2.UI
{
    partial class LoginForm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.lczxy7AnimatedButton11 = new LczxyCustom.Lczxy7AnimatedButton1();
            this.Username = new LczxyCustom.Lczxy7TextBox();
            this.status = new LczxyCustom.Lczxy7Label();
            this.lczxy7Label1 = new LczxyCustom.Lczxy7Label();
            this.lczxy7AnimatedButton12 = new LczxyCustom.Lczxy7AnimatedButton1();
            this.SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 15;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.ShadowColor = System.Drawing.Color.Red;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // lczxy7AnimatedButton11
            // 
            this.lczxy7AnimatedButton11.AnimationFillDirection = LczxyCustom.Lczxy7AnimatedButton1.FillDirection.BottomToTop;
            this.lczxy7AnimatedButton11.AnimationFillStyle = LczxyCustom.Lczxy7AnimatedButton1.FillStyle.Solid;
            this.lczxy7AnimatedButton11.AnimationSpeed = 1.5F;
            this.lczxy7AnimatedButton11.BackColor = System.Drawing.Color.Transparent;
            this.lczxy7AnimatedButton11.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.lczxy7AnimatedButton11.CornerRadius = 6;
            this.lczxy7AnimatedButton11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lczxy7AnimatedButton11.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lczxy7AnimatedButton11.HoverColor = System.Drawing.Color.Red;
            this.lczxy7AnimatedButton11.InsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(18)))));
            this.lczxy7AnimatedButton11.Location = new System.Drawing.Point(119, 193);
            this.lczxy7AnimatedButton11.Name = "lczxy7AnimatedButton11";
            this.lczxy7AnimatedButton11.ShowToolTip = false;
            this.lczxy7AnimatedButton11.Size = new System.Drawing.Size(150, 40);
            this.lczxy7AnimatedButton11.TabIndex = 0;
            this.lczxy7AnimatedButton11.Text = "LOGIN";
            this.lczxy7AnimatedButton11.TextColor = System.Drawing.Color.White;
            this.lczxy7AnimatedButton11.TextHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.lczxy7AnimatedButton11.ToolTipIcon = "";
            this.lczxy7AnimatedButton11.ToolTipMessage = "";
            this.lczxy7AnimatedButton11.Click += new System.EventHandler(this.lczxy7AnimatedButton11_Click);
            // 
            // Username
            // 
            this.Username.BackColor = System.Drawing.Color.Transparent;
            this.Username.BorderColor = System.Drawing.Color.Black;
            this.Username.BorderRadius = 10;
            this.Username.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Username.DefaultText = "KEY:";
            this.Username.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.Username.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.Username.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.Username.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.Username.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Username.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Username.ForeColor = System.Drawing.Color.White;
            this.Username.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.Username.Location = new System.Drawing.Point(72, 146);
            this.Username.Margin = new System.Windows.Forms.Padding(4);
            this.Username.Name = "Username";
            this.Username.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.Username.PlaceholderForeColor = System.Drawing.Color.White;
            this.Username.PlaceholderText = "";
            this.Username.SelectedText = "";
            this.Username.Size = new System.Drawing.Size(250, 30);
            this.Username.TabIndex = 1;
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.BackColor = System.Drawing.Color.Transparent;
            this.status.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.status.HorizontalTextAlignment = LczxyCustom.Lczxy7Label.HorizontalAlignment.Center;
            this.status.Location = new System.Drawing.Point(12, 332);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(23, 17);
            this.status.TabIndex = 2;
            this.status.Text = "N/A";
            this.status.UseCompatibleTextRendering = true;
            this.status.VerticalTextAlignment = LczxyCustom.Lczxy7Label.VerticalAlignment.Middle;
            // 
            // lczxy7Label1
            // 
            this.lczxy7Label1.AutoSize = true;
            this.lczxy7Label1.BackColor = System.Drawing.Color.Transparent;
            this.lczxy7Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lczxy7Label1.ForeColor = System.Drawing.Color.Red;
            this.lczxy7Label1.HorizontalTextAlignment = LczxyCustom.Lczxy7Label.HorizontalAlignment.Center;
            this.lczxy7Label1.Location = new System.Drawing.Point(114, 76);
            this.lczxy7Label1.Name = "lczxy7Label1";
            this.lczxy7Label1.Size = new System.Drawing.Size(155, 30);
            this.lczxy7Label1.TabIndex = 3;
            this.lczxy7Label1.Text = "TWENTY TWO";
            this.lczxy7Label1.UseCompatibleTextRendering = true;
            this.lczxy7Label1.VerticalTextAlignment = LczxyCustom.Lczxy7Label.VerticalAlignment.Middle;
            // 
            // lczxy7AnimatedButton12
            // 
            this.lczxy7AnimatedButton12.AnimationFillDirection = LczxyCustom.Lczxy7AnimatedButton1.FillDirection.BottomToTop;
            this.lczxy7AnimatedButton12.AnimationFillStyle = LczxyCustom.Lczxy7AnimatedButton1.FillStyle.Solid;
            this.lczxy7AnimatedButton12.AnimationSpeed = 1.5F;
            this.lczxy7AnimatedButton12.BackColor = System.Drawing.Color.Transparent;
            this.lczxy7AnimatedButton12.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.lczxy7AnimatedButton12.CornerRadius = 6;
            this.lczxy7AnimatedButton12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lczxy7AnimatedButton12.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lczxy7AnimatedButton12.HoverColor = System.Drawing.Color.Red;
            this.lczxy7AnimatedButton12.InsideColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(18)))));
            this.lczxy7AnimatedButton12.Location = new System.Drawing.Point(351, 12);
            this.lczxy7AnimatedButton12.Name = "lczxy7AnimatedButton12";
            this.lczxy7AnimatedButton12.ShowToolTip = false;
            this.lczxy7AnimatedButton12.Size = new System.Drawing.Size(30, 20);
            this.lczxy7AnimatedButton12.TabIndex = 9;
            this.lczxy7AnimatedButton12.Text = "X";
            this.lczxy7AnimatedButton12.TextColor = System.Drawing.Color.White;
            this.lczxy7AnimatedButton12.TextHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.lczxy7AnimatedButton12.ToolTipIcon = "";
            this.lczxy7AnimatedButton12.ToolTipMessage = "";
            this.lczxy7AnimatedButton12.Click += new System.EventHandler(this.lczxy7AnimatedButton12_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(393, 358);
            this.Controls.Add(this.lczxy7AnimatedButton12);
            this.Controls.Add(this.lczxy7Label1);
            this.Controls.Add(this.status);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.lczxy7AnimatedButton11);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.Text = "22";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private LczxyCustom.Lczxy7AnimatedButton1 lczxy7AnimatedButton11;
        private LczxyCustom.Lczxy7Label lczxy7Label1;
        private LczxyCustom.Lczxy7Label status;
        private LczxyCustom.Lczxy7TextBox Username;
        private LczxyCustom.Lczxy7AnimatedButton1 lczxy7AnimatedButton12;
    }
}

