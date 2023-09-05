using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MultipleFilesRename
{
    public static class StringExtension
    {
        public static bool IsEmpty(this string value)
        {
            bool result = false;
            result = value.Length == 0;

            return result;
        }
    }
    internal class FileNameRule : ValidationRule
    {
        public int longestFileNameLength { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            string @buffer = (string)value;

            if (buffer.IsEmpty())
            {
                result = new ValidationResult(false, "Input cannot be empty!");
            }
            else
            {
                string pattern = @"[\\\/:?*|" + "<>]";
                var matcher = new Regex(pattern, RegexOptions.Compiled);
                bool matched = matcher.IsMatch(buffer);
                if (matched)
                {
                    result = new ValidationResult(false,
                        "File name no special characters!");
                }
                else if (buffer.Length + longestFileNameLength > 255)
                {
                    result = new ValidationResult(false,
                            $"File name length cannot exceed 255!");
                }
                else
                {
                    result = ValidationResult.ValidResult;
                }
            }

            return result;
        }
    }

    internal class PositiveNumberRule : ValidationRule
    {
        public int longestFileNameLength { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = ValidationResult.ValidResult;

            string buffer = (string)value;

            if (buffer.IsEmpty())
            {
                result = new ValidationResult(false, "Input cannot be empty!");

            }
            else
            {
                string pattern = @"^[0-9]*$";
                var matcher = new Regex(pattern, RegexOptions.Compiled);
                bool matched = matcher.IsMatch(buffer);
                if (!matched)
                {
                    result = new ValidationResult(false,
                        "Enter positive numbers only!");
                }
                else if (buffer.Length + longestFileNameLength > 255)
                {
                    result = new ValidationResult(false,
                            $"File name length cannot exceed 255!");
                }
                else
                {
                    result = ValidationResult.ValidResult;
                }

            }

            return result;
        }
    }
}
