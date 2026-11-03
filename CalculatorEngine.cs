using System;
using System.Globalization;

namespace BasicCalculator
{
    public sealed class CalculatorEngine
    {
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("ru-RU");
        private readonly Random random = new Random();
        private decimal stored;
        private string operation = "";
        private bool newEntry = true;
        private bool error;

        public string Display { get; private set; } = "0";
        public string Expression { get; private set; } = "";

        private decimal Current
        {
            get { return decimal.Parse(Display, NumberStyles.Number, Culture); }
        }

        private static string Format(decimal value)
        {
            return value.ToString("0.############################", Culture);
        }

        public void Clear()
        {
            Display = "0";
            Expression = "";
            stored = 0;
            operation = "";
            newEntry = true;
            error = false;
        }

        public void Digit(string digit)
        {
            if (error) Clear();
            if (newEntry)
            {
                Display = "0";
                newEntry = false;
                if (operation == "") Expression = "";
            }
        
            if (Display.Replace("-", "").Replace(",", "").Length >= 24) return;
            if (Display == "0") Display = digit;
            else if (Display == "-0") Display = "-" + digit;
            else Display += digit;
        }

        public void Separator()
        {
            if (error) Clear();
            if (newEntry)
            {
                Display = "0";
                newEntry = false;
                if (operation == "") Expression = "";
            }
           
            if (!Display.Contains(",")) Display += ",";
        }

        public void Backspace()
        {
            if (error) { Clear(); return; }
            
            if (newEntry && operation != "") return;
            newEntry = false;
            Expression = "";
            if (Display.Length > 1) Display = Display.Substring(0, Display.Length - 1);
            else Display = "0";
            if (Display == "-" || Display == "") Display = "0";
        }

        public void ChangeSign()
        {
            if (error) return;
            if (newEntry && operation != "") Display = "0";
            if (operation == "") Expression = "";
            newEntry = false;
            if (Current == 0) return;
            Display = Display.StartsWith("-") ? Display.Substring(1) : "-" + Display;
        }

        public void RandomNumber()
        {
            if (error) Clear();
            Display = random.Next(0, 1000).ToString(Culture);
            newEntry = false;
            if (operation == "") Expression = "";
        }

        public void SetOperation(string symbol)
        {
            if (error) return;
          
            if (operation != "" && !newEntry)
            {
                if (!Calculate(Current)) return;
            }
            else stored = Current;
            operation = symbol;
            Expression = Format(stored) + " " + symbol;
            newEntry = true;
        }

        public void EqualsResult()
        {
         
            if (error || operation == "" || newEntry) return;
            decimal right = Current;
            string expression = Format(stored) + " " + operation + " " + Format(right) + " =";
            if (!Calculate(right)) return;
            Expression = expression;
            operation = "";
            newEntry = true;
        }

        private bool Calculate(decimal right)
        {
            try
            {
                decimal result;
                switch (operation)
                {
                    case "+": result = stored + right; break;
                    case "-": result = stored - right; break;
                    case "*": result = stored * right; break;
                    case "/":
                        if (right == 0) { Fail("Деление на ноль невозможно"); return false; }
                        result = stored / right;
                        break;
                    default: return false;
                }
                stored = result;
                Display = Format(result);
                return true;
            }
            catch (OverflowException)
            {
                Fail("Результат слишком большой");
                return false;
            }
        }

        private void Fail(string message)
        {
            Display = "Ошибка";
            Expression = message;
            operation = "";
            newEntry = true;
            error = true;
        }
    }
}
