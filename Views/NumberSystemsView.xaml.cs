using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TipaDiplom.Views
{
    public partial class NumberSystemsView : UserControl
    {
        public NumberSystemsView()
        {
            InitializeComponent();
        }

        private long ParseFromBase(string number, int fromBase)
        {
            number = number.Trim().ToUpper();
            long result = 0;
            foreach (char c in number)
            {
                int digit;
                if (c >= '0' && c <= '9') digit = c - '0';
                else if (c >= 'A' && c <= 'Z') digit = c - 'A' + 10;
                else throw new FormatException($"Недопустимый символ: {c}");

                if (digit >= fromBase)
                    throw new FormatException($"Цифра {c} не допустима в системе с основанием {fromBase}");

                result = result * fromBase + digit;
            }
            return result;
        }

        private string ConvertToBase(long number, int toBase)
        {
            if (number == 0) return "0";

            var sb = new StringBuilder();
            bool negative = number < 0;
            if (negative) number = -number;

            while (number > 0)
            {
                int remainder = (int)(number % toBase);
                sb.Insert(0, remainder < 10 ? (char)('0' + remainder) : (char)('A' + remainder - 10));
                number /= toBase;
            }

            if (negative) sb.Insert(0, '-');
            return sb.ToString();
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int fromBase = int.Parse(FromBaseInput.Text);
                int toBase = int.Parse(ToBaseInput.Text);
                long dec = ParseFromBase(NumberInput.Text, fromBase);
                string result = ConvertToBase(dec, toBase);

                ResultText.Text = $"{NumberInput.Text} (основание {fromBase}) = " +
                                  $"{result} (основание {toBase})\n" +
                                  $"Десятичное значение: {dec}";
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }

        private void ConvertAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int fromBase = int.Parse(FromBaseInput.Text);
                long dec = ParseFromBase(NumberInput.Text, fromBase);

                var sb = new StringBuilder();
                sb.AppendLine($"Число: {NumberInput.Text} (основание {fromBase})");
                sb.AppendLine($"Десятичное: {dec}\n");

                int[] bases = { 2, 3, 4, 5, 6, 7, 8, 10, 12, 16, 32 };
                foreach (int b in bases)
                {
                    sb.AppendLine($"Основание {b,2}: {ConvertToBase(dec, b)}");
                }

                // Дополнительно: двоичное представление
                sb.AppendLine($"\n═══ Двоичное представление ═══");
                string binary = ConvertToBase(dec, 2);
                sb.AppendLine($"Двоичное: {binary}");
                sb.AppendLine($"Количество бит: {binary.Length}");
                sb.AppendLine($"Количество единиц: {binary.Count(c => c == '1')}");
                sb.AppendLine($"Количество нулей: {binary.Count(c => c == '0')}");

                ResultText.Text = sb.ToString();
            }
            catch (Exception ex) { ResultText.Text = $"Ошибка: {ex.Message}"; }
        }
    }
}