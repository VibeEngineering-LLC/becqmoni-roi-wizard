using System;
using System.Drawing;
using System.Windows.Forms;
using XPTable.Events;
using XPTable.Renderers;

namespace BecquerelMonitor.RoiWizard
{
    // Список нуклидов на странице — не таблица, а набор строк со своей вёрсткой:
    // имя, цветные бейджи семейств и приглушённый хвост «T½ γN XN». Штатная ячейка
    // XPTable знает один цвет и один шрифт на ячейку, поэтому три колонки списка
    // рисуются своими рендерерами. Шрифт держится полем: OnPaint зовётся на каждую
    // видимую ячейку при каждой перерисовке.

    // Бейджи семейств — правило .fbadge темы: прямоугольник без скругления,
    // 9.5 px полужирным, свой цвет фона и текста на каждый код.
    public class FamilyBadgeCellRenderer : CellRenderer
    {
        // padding 0 4px, margin-right 3px, line-height 14px — числа из темы
        const int PadX = 4;
        const int Gap = 3;
        const int BadgeHeight = 14;

        Font font = WizardTheme.BadgeFont;

        protected override void OnPaint(PaintCellEventArgs e)
        {
            base.OnPaint(e);
            if (e.Cell == null || string.IsNullOrEmpty(e.Cell.Text))
            {
                return;
            }

            Rectangle rect = this.ClientRectangle;
            int x = rect.X;
            int y = rect.Y + (rect.Height - BadgeHeight) / 2;
            foreach (string code in e.Cell.Text.Split(' '))
            {
                if (code.Length == 0)
                {
                    continue;
                }
                string caption = code.ToUpperInvariant();
                int width = TextRenderer.MeasureText(e.Graphics, caption, this.font,
                    new Size(rect.Width, BadgeHeight), TextFormatFlags.NoPadding).Width + PadX * 2;
                if (x + width > rect.Right)
                {
                    break;                     // не влезло — так же обрывается строка на странице
                }
                Color back;
                Color fore;
                WizardTheme.FamilyColors(code, out back, out fore);
                using (SolidBrush brush = new SolidBrush(back))
                {
                    e.Graphics.FillRectangle(brush, x, y, width, BadgeHeight);
                }
                TextRenderer.DrawText(e.Graphics, caption, this.font,
                    new Rectangle(x + PadX, y, width - PadX * 2, BadgeHeight), fore,
                    TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter);
                x += width + Gap;
            }
        }

        public override void Dispose()
        {
            if (this.font != null)
            {
                this.font.Dispose();
                this.font = null;
            }
            base.Dispose();
        }
    }

    // Счётчики линий: «γ12 X4» — γ акцентным цветом, X сиреневым, как в списке
    // на странице. Числа приходят текстом ячейки вида «12 4»; X при нуле не рисуется.
    public class LineCountCellRenderer : CellRenderer
    {
        Font font = WizardTheme.HintFont;

        protected override void OnPaint(PaintCellEventArgs e)
        {
            base.OnPaint(e);
            if (e.Cell == null || string.IsNullOrEmpty(e.Cell.Text))
            {
                return;
            }
            string[] parts = e.Cell.Text.Split(' ');
            if (parts.Length < 2)
            {
                return;
            }

            Rectangle rect = this.ClientRectangle;
            int x = rect.X;
            x += Draw(e.Graphics, "γ" + parts[0], this.font, WizardTheme.Accent, rect, x);
            if (!string.Equals(parts[1], "0", StringComparison.Ordinal))
            {
                Draw(e.Graphics, " X" + parts[1], this.font, WizardTheme.Xray, rect, x);
            }
        }

        static int Draw(Graphics graphics, string text, Font font, Color color, Rectangle rect, int x)
        {
            Size size = TextRenderer.MeasureText(graphics, text, font,
                new Size(rect.Width, rect.Height), TextFormatFlags.NoPadding);
            TextRenderer.DrawText(graphics, text, font,
                new Rectangle(x, rect.Y, size.Width, rect.Height), color,
                TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter);
            return size.Width;
        }

        public override void Dispose()
        {
            if (this.font != null)
            {
                this.font.Dispose();
                this.font = null;
            }
            base.Dispose();
        }
    }

    // Приглушённый хвост строки (.nuc .hl): 11 px цветом --muted. Цвет берётся
    // из ячейки, если он задан — так серым гаснет нуклид без линий.
    public class HintCellRenderer : CellRenderer
    {
        Font font = WizardTheme.HintFont;

        protected override void OnPaint(PaintCellEventArgs e)
        {
            base.OnPaint(e);
            if (e.Cell == null || string.IsNullOrEmpty(e.Cell.Text))
            {
                return;
            }
            Color color = this.ForeColor.IsEmpty || this.ForeColor == Color.Transparent
                ? WizardTheme.Muted
                : this.ForeColor;
            TextRenderer.DrawText(e.Graphics, e.Cell.Text, this.font, this.ClientRectangle, color,
                TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public override void Dispose()
        {
            if (this.font != null)
            {
                this.font.Dispose();
                this.font = null;
            }
            base.Dispose();
        }
    }
}
