using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VelocityAPI;
using RobloxExecutor.Core;
using RobloxExecutor.UI.Controls;

namespace RobloxExecutor.UI
{
    public partial class Form1 : Form
    {
        private VelAPI vel = new VelAPI();
        private Timer autoInjectTimer;

        // Для перетаскивания окна
        private bool dragging = false;
        private Point dragStart;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.Log("Application started");
            AppSettings.Load();
            LuaAutocomplete.Init(richTextBox1);
            InitSettingsOverlay();

            // Применяем начальные настройки
            chkAlwaysOnTop.Checked = AppSettings.AlwaysOnTop;
            chkAutoInject.Checked = AppSettings.AutoInject;
            chkAutoExecute.Checked = AppSettings.AutoExecute;
            this.TopMost = AppSettings.AlwaysOnTop;

            autoInjectTimer = new Timer { Interval = 3000 };
            autoInjectTimer.Tick += autoInjectTimer_Tick;
            if (AppSettings.AutoInject) autoInjectTimer.Start();

            Task.Run(() =>
            {
                try { vel.StartCommunication(); }
                catch (Exception ex) 
                { 
                    Logger.LogException("StartCommunication", ex);
                    MessageBox.Show("Ошибка при запуске: " + ex.Message); 
                }
            });
        }

        // Перетаскивание окна
        private void TitlePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragStart = e.Location;
            }
        }

        private void TitlePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(e.Location, new Size(dragStart));
                this.Location = Point.Add(this.Location, new Size(diff));
            }
        }

        private void TitlePanel_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        // Закрыть
        private void closeButton_Click(object sender, EventArgs e)
        {
            try { vel.StopCommunication(); } catch { }
            Application.Exit();
        }

        // Настройки
        private void settingsButton_Click(object sender, EventArgs e)
        {
            settingsOverlay.Visible = !settingsOverlay.Visible;
            if (settingsOverlay.Visible)
            {
                settingsOverlay.BringToFront();
                titleLabel.Text = "Settings";
            }
            else
            {
                titleLabel.Text = "⚡ Velocity Executor";
            }
        }

        private void InitSettingsOverlay()
        {
            settingsOverlay = new Panel();
            settingsOverlay.BackColor = Color.FromArgb(22, 22, 22);
            settingsOverlay.Location = new Point(0, 36);
            settingsOverlay.Size = new Size(this.Width, this.Height - 36);
            settingsOverlay.Visible = false;
            this.Controls.Add(settingsOverlay);

            // Кнопка Back
            settingsBackButton = new Button
            {
                Text = "← Back",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10),
                Size = new Size(80, 28)
            };
            settingsBackButton.FlatAppearance.BorderSize = 0;
            settingsBackButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            settingsBackButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            settingsBackButton.Click += (s, e) => 
            {
                settingsOverlay.Visible = false;
                titleLabel.Text = "⚡ Velocity Executor";
            };

            // Заголовок Settings
            settingsHeaderLabel = new Label
            {
                Text = "Settings",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Location = new Point(15, 50),
                AutoSize = true
            };

            // 1. Always on top
            chkAlwaysOnTop = new ToggleSwitch { Location = new Point(15, 95) };
            topLabel = new Label { Text = "Always on top", ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(65, 92), AutoSize = true };
            topDesc = new Label { Text = "The window will stay on top of other windows", ForeColor = Color.FromArgb(120, 120, 120), Font = new Font("Segoe UI", 8), Location = new Point(65, 110), AutoSize = true };
            chkAlwaysOnTop.CheckedChanged += (s, e) =>
            {
                AppSettings.AlwaysOnTop = chkAlwaysOnTop.Checked;
                AppSettings.Save();
                this.TopMost = chkAlwaysOnTop.Checked;
            };

            // 2. Auto-inject
            chkAutoInject = new ToggleSwitch { Location = new Point(15, 155) };
            injectLabel = new Label { Text = "Auto-inject", ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(65, 152), AutoSize = true };
            injectDesc = new Label { Text = "Velocity will automatically attach to Roblox", ForeColor = Color.FromArgb(120, 120, 120), Font = new Font("Segoe UI", 8), Location = new Point(65, 170), AutoSize = true };
            chkAutoInject.CheckedChanged += (s, e) =>
            {
                AppSettings.AutoInject = chkAutoInject.Checked;
                AppSettings.Save();
                if (chkAutoInject.Checked) autoInjectTimer.Start();
                else autoInjectTimer.Stop();
            };

            // 3. Auto-execute
            chkAutoExecute = new ToggleSwitch { Location = new Point(15, 215) };
            executeLabel = new Label { Text = "Auto-execute", ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(65, 212), AutoSize = true };
            executeDesc = new Label { Text = "Velocity will automatically execute scripts inside the /autoexec folder", ForeColor = Color.FromArgb(120, 120, 120), Font = new Font("Segoe UI", 8), Location = new Point(65, 230), Size = new Size(350, 30) };
            chkAutoExecute.CheckedChanged += (s, e) =>
            {
                AppSettings.AutoExecute = chkAutoExecute.Checked;
                AppSettings.Save();
            };

            settingsOverlay.Controls.AddRange(new Control[] {
                settingsBackButton, settingsHeaderLabel,
                chkAlwaysOnTop, topLabel, topDesc,
                chkAutoInject, injectLabel, injectDesc,
                chkAutoExecute, executeLabel, executeDesc
            });
        }

        private async void autoInjectTimer_Tick(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("RobloxPlayerBeta");
            if (procs.Length > 0)
            {
                int pid = procs[0].Id;
                // Проверка vel.injected_pids как запрошено
                if (!vel.injected_pids.Contains(pid))
                {
                    VelocityStates state = await vel.Attach(pid);
                    if (state == VelocityStates.Attached)
                    {
                        if (AppSettings.AutoExecute) await RunAutoExec();
                    }
                }
            }
        }

        private async Task RunAutoExec()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoExec");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var files = Directory.GetFiles(path).Where(f => f.EndsWith(".lua") || f.EndsWith(".txt"));
            foreach (var file in files)
            {
                try
                {
                    string script = File.ReadAllText(file);
                    vel.Execute(script);
                    await Task.Delay(100); // Небольшая задержка между скриптами
                }
                catch { }
            }
        }

        // Свернуть
        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Execute
        private void button1_Click(object sender, EventArgs e)
        {
            string script = richTextBox1.Text;

            if (string.IsNullOrWhiteSpace(script))
            {
                MessageBox.Show("Script is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            VelocityStates state = vel.Execute(script);

            if (state == VelocityStates.NotAttached)
                MessageBox.Show("Firstly Inject!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Inject
        private async void button2_Click(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("RobloxPlayerBeta");
            if (procs.Length == 0)
            {
                MessageBox.Show("Roblox isn't running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnInject.Enabled = false;
            btnInject.Text = "Injecting...";

            VelocityStates state = await vel.Attach(procs[0].Id);

            btnInject.Enabled = true;
            btnInject.Text = "⚡  Inject";

            if (state == VelocityStates.Attached)
            {
                btnInject.BackColor = Color.FromArgb(20, 160, 80);
                if (AppSettings.AutoExecute) await RunAutoExec();
            }
            else
            {
                MessageBox.Show("Error: " + state.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clear
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Log("Application closed");
            try { vel.StopCommunication(); } catch { }
            base.OnFormClosing(e);
        }
    }
}
