using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using System.Windows.Markup;
using System.Text.RegularExpressions;

namespace MultipleFilesRename
{
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = long.Parse(value.ToString());
            string unit = "";

            var sizeUnit = new List<string>() { "B", "KB", "MB", "GB" };
            foreach (var i in sizeUnit)
            {
                if (Math.Log(size, 1024) < 1)
                {
                    unit = i;
                    break;
                }
                else
                    size /= 1024;
            }

            if (unit == "")
            {
                unit = "GB";
            }

            return size.ToString("F1") + " " + unit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RelativePathExtension : MarkupExtension
    {
        private readonly string _relativePath;

        public RelativePathExtension(string relativePath)
        {
            _relativePath = relativePath;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Uri uri = new Uri(_relativePath, UriKind.Relative);
            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, uri.ToString());
            return new Cursor(absolutePath);
        }
    }
}
