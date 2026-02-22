using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RobloxExecutor.UI.Controls
{
    public class ToggleSwitch : Control
    {
        private bool _checked = false;
        private Color _onColor = Color.FromArgb(60, 130, 60);
        private Color _offColor = Color.FromArgb(50, 50, 50);
        private Color _knobColor = Color.White;

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    OnCheckedChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public event EventHandler CheckedChanged;

        public ToggleSwitch()
        {
            this.Size = new Size(40, 22);
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
        }

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Checked = !Checked;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            GraphicsPath path = GetRoundedRect(rect, Height / 2);

            using (SolidBrush brush = new SolidBrush(_checked ? _onColor : _offColor))
            {
                g.FillPath(brush, path);
            }

            // Draw knob
            int knobSize = Height - 6;
            int knobX = _checked ? Width - knobSize - 3 : 3;
            int knobY = 3;

            g.FillEllipse(Brushes.White, knobX, knobY, knobSize, knobSize);
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
