using System;
using System.Drawing;
using System.Windows.Forms;
using XPTable.Models;

namespace BecquerelMonitor.RoiWizard
{
    // Палитра веб-версии инструмента (styles/becqmoni.css), перенесённая в форму
    // один в один. Веб-страница — эталон интерфейса: на ней обкатывались и раскладка,
    // и цвета, поэтому окно в BecqMoni обязано выглядеть так же, а не «примерно так».
    //
    // Числа взяты из переменных темы, имена сохранены, чтобы правку в CSS было легко
    // перенести сюда: --card, --panel, --head, --ink, --muted, --line, --grid,
    // --accent, --accent-ink, --sel, --tabbg.
    static class WizardTheme
    {
        public static readonly Color Card = Color.FromArgb(0xFF, 0xFF, 0xFF);        // --card
        public static readonly Color Panel = Color.FromArgb(0xEC, 0xEF, 0xF3);       // --panel
        public static readonly Color Head = Color.FromArgb(0x1F, 0x3A, 0x5F);        // --head
        public static readonly Color Ink = Color.FromArgb(0x1A, 0x1A, 0x1A);         // --ink
        public static readonly Color Muted = Color.FromArgb(0x5A, 0x66, 0x72);       // --muted
        public static readonly Color Line = Color.FromArgb(0xAD, 0xAD, 0xAD);        // --line
        public static readonly Color Grid = Color.FromArgb(0xEE, 0xF0, 0xF2);        // --grid
        public static readonly Color Accent = Color.FromArgb(0x12, 0x50, 0x7A);      // --accent
        public static readonly Color AccentInk = Color.FromArgb(0x1F, 0x3A, 0x5F);   // --accent-ink
        public static readonly Color Selection = Color.FromArgb(0xCD, 0xE4, 0xF7);   // --sel
        public static readonly Color TabBack = Color.FromArgb(0xE4, 0xE4, 0xE4);     // --tabbg

        // 12px/1.4 "Segoe UI" из темы — это 9 pt
        public static Font BaseFont
        {
            get { return new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point); }
        }

        public static Font LegendFont
        {
            get { return new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point); }
        }

        // Применяется после InitializeComponent: обходит дерево контролов и красит то,
        // что в вебе окрашено темой. Системные цвета трогаются только там, где тема
        // задаёт своё — фон окна (#f0f0f0) и так совпадает с системным.
        public static void Apply(Control root)
        {
            root.Font = BaseFont;
            Walk(root);
        }

        static void Walk(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                GroupBox box = control as GroupBox;
                if (box != null)
                {
                    // легенда панели — акцентным цветом и полужирным, как .gbox > .lg
                    box.ForeColor = AccentInk;
                    box.Font = LegendFont;
                    Walk(box);
                    // содержимое панели остаётся обычным шрифтом
                    foreach (Control child in box.Controls)
                    {
                        child.Font = BaseFont;
                    }
                    continue;
                }

                Table table = control as Table;
                if (table != null)
                {
                    table.GridColor = Grid;
                    table.GridLines = GridLines.Both;
                    table.SelectionBackColor = Selection;
                    table.SelectionForeColor = Ink;
                    table.ForeColor = Ink;
                    table.BackColor = Card;
                    continue;
                }

                StatusStrip status = control as StatusStrip;
                if (status != null)
                {
                    status.BackColor = Panel;
                    status.ForeColor = AccentInk;
                    continue;
                }

                ListBox list = control as ListBox;
                if (list != null)
                {
                    list.ForeColor = Accent;      // чипы «Выбрано» — акцентным, как в вебе
                    continue;
                }

                Label label = control as Label;
                if (label != null && label.Text.EndsWith(":", StringComparison.Ordinal))
                {
                    label.ForeColor = Muted;      // подписи-заголовки списков приглушены
                    continue;
                }

                Walk(control);
            }
        }
    }
}
