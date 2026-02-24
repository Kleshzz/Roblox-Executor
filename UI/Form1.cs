using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using VelocityAPI;
using RobloxExecutor.Core;
using RobloxExecutor.UI.Controls;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace RobloxExecutor.UI
{
    public partial class Form1 : Form
    {
        private VelAPI vel = new VelAPI();
        private Timer autoInjectTimer;

        // Для перетаскивания окна
        private bool dragging = false;
        private Point dragStart;
        
        // Font
        private PrivateFontCollection _pfc = new PrivateFontCollection();
        private FontFamily _satoshiFamily;
        public Font CustomFont(float size, FontStyle style = FontStyle.Regular) => new Font(_satoshiFamily, size, style);
        
        // Кэшированные иконки
        private Bitmap _iconPlus;
        private Bitmap _iconScript;

        private Label _titleSettingsLabel;

        // ── Form Rounding (Region based) ────────────────────────────────
        private void ApplyRoundedRegion(int radius = 12)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                if (this.Region != null)
                {
                    var old = this.Region;
                    this.Region = null;
                    old.Dispose();
                }
                return;
            }

            using (var path = RoundedRect(new Rectangle(0, 0, this.Width, this.Height), radius))
            {
                var old = this.Region;
                this.Region = new Region(path);
                old?.Dispose();
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            float d = radius * 2f;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyRoundedRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Тонкая рамка вокруг формы
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = RoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), 12))
            using (var pen = new Pen(Color.FromArgb(55, 55, 55), 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        // ── Кастомная система вкладок ────────────────────────────────────
        private readonly List<TabEntry> _tabs = new List<TabEntry>();
        private int _activeIndex = -1;
        private int _tabCounter = 0;
        private int _tabScrollOffset = 0;

        // Размеры элементов таб-стрипа
        private const int TAB_WIDTH  = 110;
        private const int TAB_HEIGHT = 30;
        private const int PLUS_WIDTH = 28;

        private class TabEntry
        {
            public string Title;
            public FastColoredTextBox Editor;
            public Rectangle Rect; // рассчитывается при отрисовке
        }

        public Form1()
        {
            InitializeComponent();
            LoadSatoshiFont();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.Log("Application started");
            AppSettings.Load();
            DiscordManager.Initialize();

            // Инициализация иконок
            _iconPlus = new Bitmap(Properties.Resources.plus, 14, 14);
            _iconScript = new Bitmap(Properties.Resources.script, 12, 12);

            // Логотип и кнопки заголовка
            logoImage.Image = Properties.Resources.velocity;
            settingsButton.Image = ResizeImage(Properties.Resources.settings, 18, 18);
            minimizeButton.Image = ResizeImage(Properties.Resources.minimize, 14, 14);
            closeButton.Image    = ResizeImage(Properties.Resources.cross, 14, 14);

            // Кнопки нижней панели
            btnExecute.Image = ResizeImage(Properties.Resources.execute, 22, 22);
            btnClear.Image   = ResizeImage(Properties.Resources.clear, 18, 18); // X icon
            btnOpen.Image    = ResizeImage(Properties.Resources.open, 18, 18);
            btnSave.Image    = ResizeImage(Properties.Resources.save, 20, 20);
            btnInject.Image  = ResizeImage(Properties.Resources.inject, 20, 20);
            btnMonitor.Image = ResizeImage(Properties.Resources.monitor, 20, 20);

            AddNewTab();
            RefreshTabScroll();

            InitSettingsOverlay();

            chkAlwaysOnTop.Checked = AppSettings.AlwaysOnTop;
            chkAutoInject.Checked  = AppSettings.AutoInject;
            chkAutoExecute.Checked = AppSettings.AutoExecute;
            this.TopMost = AppSettings.AlwaysOnTop;

            autoInjectTimer = new Timer { Interval = 3000 };
            autoInjectTimer.Tick += autoInjectTimer_Tick;
            if (AppSettings.AutoInject) autoInjectTimer.Start();

            // Включить ClearType для всей формы
            try { 
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false); 
            } catch { }
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer, true);

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

        private Bitmap ResizeImage(Bitmap source, int w, int h)
        {
            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, w, h);
            }
            return bmp;
        }

        // ── Tab System ──────────────────────────────────────────────────

        private FastColoredTextBox AddNewTab(string title = null)
        {
            if (title == null)
            {
                _tabCounter++;
                title = "Script " + _tabCounter;
            }

            var editor = new FastColoredTextBox();
            ((System.ComponentModel.ISupportInitialize)editor).BeginInit();
            editor.Dock        = DockStyle.Fill;
            editor.BackColor   = Color.FromArgb(18, 18, 18);
            editor.ForeColor   = Color.FromArgb(220, 220, 220);
            editor.Font        = new Font("Consolas", 10.5F);
            editor.CaretColor  = Color.FromArgb(100, 180, 255);
            editor.CurrentLineColor = Color.FromArgb(20, 100, 130, 150);
            editor.SelectionColor   = Color.FromArgb(50, 80, 100, 130);
            editor.IndentBackColor  = Color.FromArgb(22, 22, 22);
            editor.LineNumberColor  = Color.FromArgb(80, 80, 80);
            editor.ShowLineNumbers  = true;
            editor.BorderStyle      = BorderStyle.None;
            editor.Paddings         = new Padding(4, 4, 0, 0);
            editor.AutoCompleteBracketsList = new char[] {
                '(',')','{','}','[',']','"','"','\'','\''
            };
            ((System.ComponentModel.ISupportInitialize)editor).EndInit();

            LuaAutocomplete.Init(editor);
            LuaStyle.Apply(editor);

            var entry = new TabEntry { Title = title, Editor = editor };
            _tabs.Add(entry);

            // Скрываем все редакторы, показываем новый
            foreach (var t in _tabs) t.Editor.Visible = false;
            tabContent.Controls.Add(editor);
            editor.Visible = true;

            _activeIndex = _tabs.Count - 1;
            RefreshTabScroll();
            tabStrip.Invalidate();

            return editor;
        }

        private FastColoredTextBox CurrentEditor()
        {
            if (_activeIndex < 0 || _activeIndex >= _tabs.Count) return null;
            return _tabs[_activeIndex].Editor;
        }

        private void ActivateTab(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;

            foreach (var t in _tabs) t.Editor.Visible = false;
            _tabs[index].Editor.Visible = true;
            _activeIndex = index;
            tabStrip.Invalidate();

            // Фокусируем редактор
            _tabs[index].Editor.Focus();
        }

        private void CloseTab(int index)
        {
            if (_tabs.Count <= 1) return;
            if (index < 0 || index >= _tabs.Count) return;

            var ed = _tabs[index].Editor;
            tabContent.Controls.Remove(ed);
            ed.Dispose();
            _tabs.RemoveAt(index);

            // Выбираем соседнюю вкладку
            _activeIndex = Math.Min(index, _tabs.Count - 1);
            ActivateTab(_activeIndex);
            RefreshTabScroll();
        }

        // ── Tab Strip Drawing (кастомный Paint) ─────────────────────────

        private void TabStrip_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.Clear(Color.FromArgb(20, 20, 20));

            int totalWidth = _tabs.Count * TAB_WIDTH + PLUS_WIDTH + 8;
            bool needScroll = totalWidth > tabStrip.Width;
            int startX = needScroll ? 20 : 0;
            int endX = needScroll ? tabStrip.Width - 20 : tabStrip.Width;

            // Клиппинг для вкладок (чтобы не рисовались поверх стрелок)
            if (needScroll)
            {
                g.SetClip(new Rectangle(startX, 0, endX - startX, tabStrip.Height));
            }

            int x = startX - _tabScrollOffset;
            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                bool isActive = (i == _activeIndex);

                // Сохраняем реальные координаты для клика (без учета боковых стрелок и смещения отрисовки)
                // Но adjustedLoc в MouseClick будет e.X + _tabScrollOffset, так что Rect должен быть i*TAB_WIDTH
                tab.Rect = new Rectangle(i * TAB_WIDTH, 0, TAB_WIDTH, TAB_HEIGHT);

                var drawRect = new Rectangle(x, 0, TAB_WIDTH, TAB_HEIGHT);

                // Проверка на видимость (опционально, клиппинг и так работает)
                if (drawRect.Right >= startX && drawRect.Left <= endX)
                {
                    // Отрисовка закругленной вкладки (пилюля)
                    int radius = 8;
                    using (var path = new GraphicsPath())
                    {
                        var rect = new Rectangle(drawRect.X + 2, drawRect.Y + 2, drawRect.Width - 4, drawRect.Height - 4);
                        path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                        path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                        path.CloseAllFigures();

                        Color bg = isActive ? Color.FromArgb(50, 50, 50) : Color.FromArgb(22, 22, 22);
                        using (var b = new SolidBrush(bg))
                            g.FillPath(b, path);

                        if (isActive)
                        {
                            using (var pen = new Pen(Color.FromArgb(70, 70, 70), 1))
                                g.DrawPath(pen, path);
                        }
                    }

                    if (!isActive)
                    {
                        // Разделитель между вкладками (тонкий)
                        using (var p = new Pen(Color.FromArgb(40, 40, 40), 1))
                            g.DrawLine(p, drawRect.Right - 1, drawRect.Top + 8, drawRect.Right - 1, drawRect.Bottom - 8);
                    }

                    Color fg = isActive ? Color.White : Color.FromArgb(140, 140, 140);
                    using (var font = CustomFont(9F, FontStyle.Bold))
                    using (var b = new SolidBrush(fg))
                    {
                        var textRect = new Rectangle(drawRect.X + 26, drawRect.Y, drawRect.Width - 44, drawRect.Height);
                        var sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Trimming      = StringTrimming.EllipsisCharacter,
                            FormatFlags   = StringFormatFlags.NoWrap
                        };
                        
                        // Иконка скрипта
                        g.DrawImage(_iconScript, drawRect.X + 4, drawRect.Y + (drawRect.Height - 12) / 2);
                        
                        g.DrawString(tab.Title, font, b, textRect, sf);
                    }

                    if (_tabs.Count > 1)
                    {
                        var closeRect = GetCloseRect(drawRect);
                        Color cx = isActive ? Color.FromArgb(170, 170, 170) : Color.FromArgb(90, 90, 90);
                        using (var font = CustomFont(11F))
                        using (var b = new SolidBrush(cx))
                        {
                            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            g.DrawString("×", font, b, closeRect, sf);
                        }
                    }
                }
                x += TAB_WIDTH;
            }

            // Кнопка "+"
            var plusRectDraw = new Rectangle(x + 4, 0, PLUS_WIDTH, TAB_HEIGHT);
            if (plusRectDraw.Right >= startX && plusRectDraw.Left <= endX)
            {
                g.DrawImage(_iconPlus, plusRectDraw.X + (plusRectDraw.Width - 14) / 2, plusRectDraw.Y + (plusRectDraw.Height - 14) / 2);
            }

            if (needScroll)
            {
                g.ResetClip();

                // Отрисовка стрелок поверх
                DrawArrow(g, "‹", new Rectangle(0, 0, 20, TAB_HEIGHT), _tabScrollOffset > 0);
                
                int visibleWidth = tabStrip.Width - 40;
                int maxScroll = Math.Max(0, totalWidth - visibleWidth);
                DrawArrow(g, "›", new Rectangle(tabStrip.Width - 20, 0, 20, TAB_HEIGHT), _tabScrollOffset < maxScroll);
            }
        }

        private void DrawArrow(Graphics g, string arrow, Rectangle rect, bool active)
        {
            using (var bBg = new SolidBrush(Color.FromArgb(20, 20, 20)))
                g.FillRectangle(bBg, rect);
            
            Color fg = active ? Color.FromArgb(160, 160, 160) : Color.FromArgb(50, 50, 50);
            using (var font = CustomFont(12F))
            using (var b = new SolidBrush(fg))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(arrow, font, b, rect, sf);
            }
        }

        private Rectangle GetCloseRect(Rectangle tabRect)
        {
            return new Rectangle(tabRect.Right - 20, tabRect.Top + 6, 16, 16);
        }

        private Rectangle GetPlusRect()
        {
            int x = _tabs.Count * TAB_WIDTH + 4;
            return new Rectangle(x, 0, PLUS_WIDTH, TAB_HEIGHT);
        }

        // ── Tab Strip Mouse ─────────────────────────────────────────────

        private void TabStrip_MouseClick(object sender, MouseEventArgs e)
        {
            int totalWidth = _tabs.Count * TAB_WIDTH + PLUS_WIDTH + 8;
            bool needScroll = totalWidth > tabStrip.Width;

            if (needScroll)
            {
                // Клик на левую стрелку
                if (e.X < 20)
                {
                    _tabScrollOffset = Math.Max(0, _tabScrollOffset - TAB_WIDTH);
                    tabStrip.Invalidate();
                    return;
                }
                // Клик на правую стрелку
                if (e.X > tabStrip.Width - 20)
                {
                    int visibleWidth = tabStrip.Width - 40;
                    int maxScroll = Math.Max(0, totalWidth - visibleWidth);
                    _tabScrollOffset = Math.Min(maxScroll, _tabScrollOffset + TAB_WIDTH);
                    tabStrip.Invalidate();
                    return;
                }
            }

            int startX = needScroll ? 20 : 0;
            var adjustedLoc = new Point(e.X - startX + _tabScrollOffset, e.Y);

            // Клик на "+"
            var plusRect = GetPlusRect();
            if (plusRect.Contains(adjustedLoc))
            {
                AddNewTab();
                return;
            }

            // Клик по вкладкам
            for (int i = 0; i < _tabs.Count; i++)
            {
                var tabRect = _tabs[i].Rect;
                if (!tabRect.Contains(adjustedLoc)) continue;

                // Клик на крестик
                if (_tabs.Count > 1 && GetCloseRect(tabRect).Contains(adjustedLoc))
                {
                    CloseTab(i);
                    return;
                }

                // Активация вкладки
                ActivateTab(i);
                return;
            }
        }

        private void RefreshTabScroll()
        {
            int visibleWidth = tabStrip.Width - 40;
            int totalWidth = _tabs.Count * TAB_WIDTH + PLUS_WIDTH + 8;
            int maxScroll = Math.Max(0, totalWidth - visibleWidth);
            
            _tabScrollOffset = Math.Min(_tabScrollOffset, maxScroll);
            tabStrip.Invalidate();
        }

        // ── Перетаскивание окна ─────────────────────────────────────────

        private void TitlePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { dragging = true; dragStart = e.Location; }
        }

        private void TitlePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
                this.Location = Point.Add(this.Location, new Size(Point.Subtract(e.Location, new Size(dragStart))));
        }

        private void TitlePanel_MouseUp(object sender, MouseEventArgs e) { dragging = false; }

        private void closeButton_Click(object sender, EventArgs e)
        {
            try { vel.StopCommunication(); } catch { }
            Application.Exit();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            settingsOverlay.Visible = !settingsOverlay.Visible;
            if (settingsOverlay.Visible)
            {
                settingsOverlay.BringToFront();
                logoImage.Visible = true; // иконка остаётся
                // Показываем лейбл "Settings" рядом с иконкой
                if (_titleSettingsLabel == null)
                {
                    _titleSettingsLabel = new Label
                    {
                        Text = "Settings",
                        ForeColor = Color.White,
                        BackColor = Color.Transparent,
                        Font = CustomFont(10F, FontStyle.Bold),
                        Location = new Point(38, 0),
                        Size = new Size(80, 38),
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    titlePanel.Controls.Add(_titleSettingsLabel);
                }
                _titleSettingsLabel.Visible = true;
                ApplyCustomFont(settingsOverlay);
            }
            else
            {
                logoImage.Visible = true;
                if (_titleSettingsLabel != null)
                    _titleSettingsLabel.Visible = false;
            }
        }

        private void InitSettingsOverlay()
        {
            settingsOverlay = new Panel();
            settingsOverlay.BackColor = Color.FromArgb(22, 22, 22);
            settingsOverlay.Location  = new Point(0, 36);
            settingsOverlay.Size      = new Size(this.Width, this.Height - 36);
            settingsOverlay.Visible   = false;
            this.Controls.Add(settingsOverlay);

            settingsBackButton = new Button
            {
                Text = "← Back", FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White, BackColor = Color.Transparent,
                Font = CustomFont(10F),
                Location = new Point(10, 10), Size = new Size(80, 28)
            };
            settingsBackButton.FlatAppearance.BorderSize = 0;
            settingsBackButton.FlatAppearance.MouseOverBackColor  = Color.Transparent;
            settingsBackButton.FlatAppearance.MouseDownBackColor  = Color.Transparent;
            settingsBackButton.Click += (s, ev) => {
                settingsOverlay.Visible = false;
                logoImage.Visible = true;
                if (_titleSettingsLabel != null)
                    _titleSettingsLabel.Visible = false;
            };

            settingsHeaderLabel = new Label
            {
                Text = "Settings", ForeColor = Color.White,
                Font = CustomFont(13F, FontStyle.Bold),
                Location = new Point(15, 50), AutoSize = true
            };

            chkAlwaysOnTop = new ToggleSwitch { Location = new Point(15, 95) };
            topLabel = new Label { Text = "Always on top", ForeColor = Color.White, Font = CustomFont(10F, FontStyle.Bold), Location = new Point(65, 92), AutoSize = true };
            topDesc  = new Label { Text = "The window will stay on top of other windows", ForeColor = Color.FromArgb(120,120,120), Font = CustomFont(8F), Location = new Point(65, 110), AutoSize = true };
            chkAlwaysOnTop.CheckedChanged += (s, ev) => { AppSettings.AlwaysOnTop = chkAlwaysOnTop.Checked; AppSettings.Save(); this.TopMost = chkAlwaysOnTop.Checked; };

            chkAutoInject = new ToggleSwitch { Location = new Point(15, 155) };
            injectLabel = new Label { Text = "Auto-inject", ForeColor = Color.White, Font = CustomFont(10F, FontStyle.Bold), Location = new Point(65, 152), AutoSize = true };
            injectDesc  = new Label { Text = "Velocity will automatically attach to Roblox", ForeColor = Color.FromArgb(120,120,120), Font = CustomFont(8F), Location = new Point(65, 170), AutoSize = true };
            chkAutoInject.CheckedChanged += (s, ev) => { AppSettings.AutoInject = chkAutoInject.Checked; AppSettings.Save(); if (chkAutoInject.Checked) autoInjectTimer.Start(); else autoInjectTimer.Stop(); };

            chkAutoExecute = new ToggleSwitch { Location = new Point(15, 215) };
            executeLabel = new Label { Text = "Auto-execute", ForeColor = Color.White, Font = CustomFont(10F, FontStyle.Bold), Location = new Point(65, 212), AutoSize = true };
            executeDesc  = new Label { Text = "Velocity will automatically execute scripts inside the /autoexec folder", ForeColor = Color.FromArgb(120,120,120), Font = CustomFont(8F), Location = new Point(65, 230), Size = new Size(350, 30) };
            chkAutoExecute.CheckedChanged += (s, ev) => { AppSettings.AutoExecute = chkAutoExecute.Checked; AppSettings.Save(); };

            settingsOverlay.Controls.AddRange(new Control[] {
                settingsBackButton, settingsHeaderLabel,
                chkAlwaysOnTop, topLabel, topDesc,
                chkAutoInject, injectLabel, injectDesc,
                chkAutoExecute, executeLabel, executeDesc
            });
            ApplyCustomFont(settingsOverlay);
        }

        private async void autoInjectTimer_Tick(object sender, EventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("RobloxPlayerBeta");
            if (procs.Length > 0)
            {
                int pid = procs[0].Id;
                if (!vel.injected_pids.Contains(pid))
                {
                    VelocityStates state = await vel.Attach(pid);
                    if (state == VelocityStates.Attached && AppSettings.AutoExecute)
                        await RunAutoExec();
                }
            }
        }

        private async Task RunAutoExec()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoExec");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            foreach (var file in Directory.GetFiles(path).Where(f => f.EndsWith(".lua") || f.EndsWith(".txt")))
            {
                try { vel.Execute(File.ReadAllText(file)); await Task.Delay(100); } catch { }
            }
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Execute
        private void button1_Click(object sender, EventArgs e)
        {
            var editor = CurrentEditor();
            if (editor == null) return;
            string script = editor.Text;
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
            btnInject.Text = "Inject";
            if (state == VelocityStates.Attached)
            {
                btnInject.BackColor = Color.FromArgb(20, 160, 80);
                DiscordManager.SetInjected();
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
            CurrentEditor()?.Clear();
        }

        // Open File
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Lua files (*.lua)|*.lua|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        var editor = CurrentEditor();
                        if (editor != null)
                        {
                            editor.Text = File.ReadAllText(ofd.FileName);
                            _tabs[_activeIndex].Title = Path.GetFileName(ofd.FileName);
                            tabStrip.Invalidate();
                        }
                    }
                }
            }
            catch (Exception ex) { Logger.LogException("btnOpen_Click", ex); }
        }

        // Save File
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter   = "Lua files (*.lua)|*.lua|Text files (*.txt)|*.txt";
                    sfd.FileName = "script.lua";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var editor = CurrentEditor();
                        if (editor != null)
                        {
                            File.WriteAllText(sfd.FileName, editor.Text);
                            _tabs[_activeIndex].Title = Path.GetFileName(sfd.FileName);
                            tabStrip.Invalidate();
                        }
                    }
                }
            }
            catch (Exception ex) { Logger.LogException("btnSave_Click", ex); }
        }

        private void LoadSatoshiFont()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string resourceName = "RobloxExecutor.Assets.Fonts.Satoshi-Regular.otf";
                
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) 
                    {
                        resourceName = assembly.GetManifestResourceNames().FirstOrDefault(rn => rn.Contains("Satoshi"));
                        if (resourceName == null) return;
                        using (var stream2 = assembly.GetManifestResourceStream(resourceName)) { LoadFontFromStream(stream2); }
                    }
                    else { LoadFontFromStream(stream); }
                }

                ApplyCustomFont(this);
            }
            catch (Exception ex) { Logger.LogException("LoadSatoshiFont", ex); }
        }

        private void LoadFontFromStream(Stream stream)
        {
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            IntPtr ptr = Marshal.AllocCoTaskMem(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            _pfc.AddMemoryFont(ptr, data.Length);
            Marshal.FreeCoTaskMem(ptr);
            if (_pfc.Families.Length > 0) _satoshiFamily = _pfc.Families[0];
        }

        private void ApplyCustomFont(Control parent)
        {
            if (_satoshiFamily == null) return;
            foreach (Control c in parent.Controls)
            {
                if (!(c is FastColoredTextBox))
                {
                    float size = 9F;
                    FontStyle style = FontStyle.Regular;

                    // Specific sizing logic
                    if (c == btnExecute || c == btnInject || c == btnClear || c == btnOpen || c == btnSave || c == btnMonitor) { size = 10F; }
                    else if (c == settingsButton || c == minimizeButton || c == closeButton) { size = 10F; }
                    else { size = c.Font.Size; style = c.Font.Style; }

                    c.Font = CustomFont(size, style);

                    if (c is ButtonBase btn) btn.UseCompatibleTextRendering = false;
                    if (c is Label lbl) lbl.UseCompatibleTextRendering = false;

                    if (c.HasChildren) ApplyCustomFont(c);
                }
            }
        }

        private void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            // Тонкая линия разделитель сверху #424242
            using (var pen = new Pen(Color.FromArgb(66, 66, 66), 1))
            {
                e.Graphics.DrawLine(pen, 0, 0, bottomPanel.Width, 0);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Log("Application closed");
            DiscordManager.Shutdown();
            try { vel.StopCommunication(); } catch { }
            
            _iconPlus?.Dispose();
            _iconScript?.Dispose();
            _pfc.Dispose();
            
            base.OnFormClosing(e);
        }
    }
}
