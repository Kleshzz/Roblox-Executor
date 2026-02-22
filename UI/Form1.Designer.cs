using RobloxExecutor.UI.Controls;

namespace RobloxExecutor.UI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.richTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnInject = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.minimizeButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();

            // === richTextBox1 (FastColoredTextBox) ===
            this.richTextBox1.Language = FastColoredTextBoxNS.Language.Lua;
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(18, 18, 18);
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.richTextBox1.CaretColor = System.Drawing.Color.FromArgb(100, 180, 255);
            this.richTextBox1.IndentBackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            this.richTextBox1.LineNumberColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.richTextBox1.CurrentLineColor = System.Drawing.Color.FromArgb(30, 100, 180, 255);
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 10.5F);
            this.richTextBox1.Location = new System.Drawing.Point(10, 45);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(780, 310);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            // === titlePanel ===
            this.titlePanel.BackColor = System.Drawing.Color.FromArgb(15, 15, 15);
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.Size = new System.Drawing.Size(800, 38);
            this.titlePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseUp);
            this.titlePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseDown);
            this.titlePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseMove);

            // === titleLabel ===
            this.titleLabel.Text = "⚡ Velocity Executor";
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 180, 255);
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.titleLabel.Location = new System.Drawing.Point(12, 8);
            this.titleLabel.Size = new System.Drawing.Size(200, 22);
            this.titleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseUp);
            this.titleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseDown);
            this.titleLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseMove);

            // === closeButton ===
            this.closeButton.Text = "✕";
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(200, 40, 40);
            this.closeButton.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.closeButton.Location = new System.Drawing.Point(762, 4);
            this.closeButton.Size = new System.Drawing.Size(30, 28);
            this.closeButton.TabStop = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);

            // === minimizeButton ===
            this.minimizeButton.Text = "─";
            this.minimizeButton.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
            this.minimizeButton.BackColor = System.Drawing.Color.Transparent;
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.FlatAppearance.BorderSize = 0;
            this.minimizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.minimizeButton.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.minimizeButton.Location = new System.Drawing.Point(730, 4);
            this.minimizeButton.Size = new System.Drawing.Size(30, 28);
            this.minimizeButton.TabStop = false;
            this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);

            // === settingsButton ===
            this.settingsButton.Text = "⚙";
            this.settingsButton.ForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
            this.settingsButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.FlatAppearance.BorderSize = 0;
            this.settingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.settingsButton.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.settingsButton.Location = new System.Drawing.Point(694, 1);
            this.settingsButton.Size = new System.Drawing.Size(36, 36);
            this.settingsButton.TabStop = false;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);

            // === bottomPanel ===
            this.bottomPanel.BackColor = System.Drawing.Color.FromArgb(15, 15, 15);
            this.bottomPanel.Location = new System.Drawing.Point(0, 362);
            this.bottomPanel.Size = new System.Drawing.Size(800, 60);

            // === btnExecute ===
            this.btnExecute.Text = "▶  Execute";
            this.btnExecute.ForeColor = System.Drawing.Color.White;
            this.btnExecute.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecute.FlatAppearance.BorderSize = 1;
            this.btnExecute.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnExecute.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this.btnExecute.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExecute.Location = new System.Drawing.Point(12, 10);
            this.btnExecute.Size = new System.Drawing.Size(110, 38);
            this.btnExecute.TabIndex = 1;
            this.btnExecute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExecute.Click += new System.EventHandler(this.button1_Click);

            // === btnInject ===
            this.btnInject.Text = "⚡  Inject";
            this.btnInject.ForeColor = System.Drawing.Color.White;
            this.btnInject.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.btnInject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInject.FlatAppearance.BorderSize = 1;
            this.btnInject.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnInject.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this.btnInject.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnInject.Location = new System.Drawing.Point(678, 10);
            this.btnInject.Size = new System.Drawing.Size(110, 38);
            this.btnInject.TabIndex = 2;
            this.btnInject.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInject.Click += new System.EventHandler(this.button2_Click);

            // === btnClear ===
            this.btnClear.Text = "🗑  Clear";
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.FlatAppearance.BorderSize = 1;
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnClear.Location = new System.Drawing.Point(560, 10);
            this.btnClear.Size = new System.Drawing.Size(110, 38);
            this.btnClear.TabIndex = 3;
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.Click += new System.EventHandler(this.button3_Click);

            // === btnOpen ===
            this.btnOpen.Text = "📂  Open";
            this.btnOpen.ForeColor = System.Drawing.Color.White;
            this.btnOpen.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.FlatAppearance.BorderSize = 1;
            this.btnOpen.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 10);
            this.btnOpen.Location = new System.Drawing.Point(130, 10);
            this.btnOpen.Size = new System.Drawing.Size(110, 38);
            this.btnOpen.TabIndex = 4;
            this.btnOpen.Cursor = System.Windows.Forms.Cursors.Hand;

            // === btnSave ===
            this.btnSave.Text = "💾  Save";
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 1;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10);
            this.btnSave.Location = new System.Drawing.Point(248, 10);
            this.btnSave.Size = new System.Drawing.Size(110, 38);
            this.btnSave.TabIndex = 5;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;


            // Добавляем в панели
            this.titlePanel.Controls.Add(this.titleLabel);
            this.titlePanel.Controls.Add(this.settingsButton);
            this.titlePanel.Controls.Add(this.minimizeButton);
            this.titlePanel.Controls.Add(this.closeButton);

            this.bottomPanel.Controls.Add(this.btnExecute);
            this.bottomPanel.Controls.Add(this.btnOpen);
            this.bottomPanel.Controls.Add(this.btnSave);
            this.bottomPanel.Controls.Add(this.btnClear);
            this.bottomPanel.Controls.Add(this.btnInject);

            // === Form1 ===
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 422);
            this.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Velocity Executor";
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.titlePanel);
            this.Controls.Add(this.bottomPanel);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

        private FastColoredTextBoxNS.FastColoredTextBox richTextBox1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button minimizeButton;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Panel settingsOverlay;
        private System.Windows.Forms.Button settingsBackButton;
        private System.Windows.Forms.Label settingsHeaderLabel;
        private ToggleSwitch chkAlwaysOnTop;
        private ToggleSwitch chkAutoInject;
        private ToggleSwitch chkAutoExecute;
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.Label topDesc;
        private System.Windows.Forms.Label injectLabel;
        private System.Windows.Forms.Label injectDesc;
        private System.Windows.Forms.Label executeLabel;
        private System.Windows.Forms.Label executeDesc;
    }
}
