using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RobloxExecutor.UI
{
    public class RoundedButton : Button
    {
        public int BorderRadius { get; set; } = 6;

        public RoundedButton()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0, 0);
            FlatAppearance.MouseOverBackColor = Color.FromArgb(27, 27, 27);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 40, 40);
            TabStop = false;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Закрашиваем весь фон цветом родителя чтобы убрать артефакты по углам
            Color parentBg = Parent != null ? Parent.BackColor : Color.FromArgb(16, 16, 16);
            using (var bgBrush = new SolidBrush(parentBg))
                g.FillRectangle(bgBrush, 0, 0, Width, Height);

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var path = GetRoundedRectPath(rect, BorderRadius);

            using (var brush = new SolidBrush(BackColor))
                g.FillPath(brush, path);

            int iconSize = 18;

            if (Image != null)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    var imgRect = new Rectangle((Width - iconSize) / 2, (Height - iconSize) / 2, iconSize, iconSize);
                    g.DrawImage(Image, imgRect);
                }
                else
                {
                    int xPadding = 12;
                    var imgRect = new Rectangle(xPadding, (Height - iconSize) / 2, iconSize, iconSize);
                    g.DrawImage(Image, imgRect);

                    var textRect = new Rectangle(xPadding + iconSize + 6, 0, Width - (xPadding + iconSize + 10), Height);
                    var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near };
                    using (var brush = new SolidBrush(ForeColor))
                        g.DrawString(Text, Font, brush, textRect, sf);
                }
            }
            else
            {
                var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                using (var brush = new SolidBrush(ForeColor))
                    g.DrawString(Text, Font, brush, rect, sf);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            float d = radius * 2f;
            if (d > rect.Height) d = rect.Height;
            if (d > rect.Width) d = rect.Width;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            BackColor = Color.FromArgb(45, 45, 45);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            BackColor = Color.FromArgb(27, 27, 27);
            Invalidate();
        }
    }
}
