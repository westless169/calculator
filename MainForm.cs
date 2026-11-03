using System;
using System.Drawing;
using System.Windows.Forms;

namespace BasicCalculator
{
    public sealed class MainForm : Form
    {
        private readonly CalculatorEngine calculator = new CalculatorEngine();
        private readonly TextBox display = new TextBox();
        private readonly Label expression = new Label();

        public MainForm()
        {
            Text = "Базовый калькулятор";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(380, 480);
            MinimumSize = new Size(340, 480);
            Font = new Font("Segoe UI", 12F);
            AutoScaleMode = AutoScaleMode.Font;
            KeyPreview = true;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 4,
                RowCount = 8
            };
            for (int i = 0; i < 4; i++)
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            for (int i = 0; i < 6; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 6F));
            Controls.Add(layout);

            expression.TextAlign = ContentAlignment.MiddleRight;
            expression.AutoEllipsis = true;
            expression.Dock = DockStyle.Fill;
            expression.Font = new Font("Segoe UI", 10F);
            layout.Controls.Add(expression, 0, 0);
            layout.SetColumnSpan(expression, 4);

            display.ReadOnly = true;
            display.TextAlign = HorizontalAlignment.Right;
            display.Font = new Font("Segoe UI", 23F);
            display.Dock = DockStyle.Fill;
            display.BackColor = Color.White;
            display.TabStop = false;
            layout.Controls.Add(display, 0, 1);
            layout.SetColumnSpan(display, 4);

            AddButton(layout, "C", 0, 2, calculator.Clear);
            AddButton(layout, "⌫", 1, 2, calculator.Backspace);
            AddButton(layout, "+/−", 2, 2, calculator.ChangeSign);
            AddButton(layout, "+", 3, 2, delegate { calculator.SetOperation("+"); });
            string[,] keys = { { "7", "8", "9", "-" }, { "4", "5", "6", "*" }, { "1", "2", "3", "/" } };
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    string key = keys[row, col];
                    AddButton(layout, key, col, row + 3, delegate
                    {
                        if (key == "-" || key == "*" || key == "/") calculator.SetOperation(key);
                        else calculator.Digit(key);
                    });
                }
            }
            AddButton(layout, "0", 0, 6, delegate { calculator.Digit("0"); });
            AddButton(layout, ",", 1, 6, calculator.Separator);
            Button equals = AddButton(layout, "=", 2, 6, calculator.EqualsResult);
            layout.SetColumnSpan(equals, 2);
            Button random = AddButton(layout, "Случайное число (0–999)", 0, 7, calculator.RandomNumber);
            layout.SetColumnSpan(random, 4);
            RefreshDisplay();
        }

        private Button AddButton(TableLayoutPanel layout, string caption, int column, int row, Action action)
        {
            var button = new Button
            {
                Text = caption,
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                UseVisualStyleBackColor = true
            };
            button.Click += delegate { action(); RefreshDisplay(); };
            layout.Controls.Add(button, column, row);
            return button;
        }

        private void RefreshDisplay()
        {
            display.Text = calculator.Display;
            expression.Text = calculator.Expression;
        }

        // Обрабатываем клавиши до стандартной навигации кнопок.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            bool plain = (keyData & Keys.Modifiers) == Keys.None;
            if (plain && key >= Keys.D0 && key <= Keys.D9)
                calculator.Digit(((int)key - (int)Keys.D0).ToString());
            else if (plain && key >= Keys.NumPad0 && key <= Keys.NumPad9)
                calculator.Digit(((int)key - (int)Keys.NumPad0).ToString());
            else if (plain && (key == Keys.Decimal || key == Keys.Oemcomma || key == Keys.OemPeriod)) calculator.Separator();
            else if (plain && key == Keys.Back) calculator.Backspace();
            else if (plain && (key == Keys.Escape || key == Keys.Delete)) calculator.Clear();
            else if (plain && key == Keys.Enter) calculator.EqualsResult();
            else if (plain && key == Keys.F9) calculator.ChangeSign();
            else if (plain && key == Keys.Add) calculator.SetOperation("+");
            else if (plain && (key == Keys.Subtract || key == Keys.OemMinus)) calculator.SetOperation("-");
            else if (plain && key == Keys.Multiply) calculator.SetOperation("*");
            else if (plain && (key == Keys.Divide || key == Keys.OemQuestion)) calculator.SetOperation("/");
            else if (keyData == (Keys.Shift | Keys.Oemplus)) calculator.SetOperation("+");
            else if (keyData == (Keys.Shift | Keys.D8)) calculator.SetOperation("*");
            else if (plain && key == Keys.Oemplus) calculator.EqualsResult();
            else return base.ProcessCmdKey(ref msg, keyData);
            RefreshDisplay();
            return true;
        }
    }
}
