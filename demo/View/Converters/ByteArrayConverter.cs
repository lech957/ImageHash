namespace Demo.View.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(byte[]), typeof(string))]
    public class ByteArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes)
            {
                // .NET 5+ (recommended)
                return System.Convert.ToHexString(bytes);
                // For older .NET: return BitConverter.ToString(bytes).Replace("-", "");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Optional: implement if you need two-way binding
            throw new NotImplementedException();
        }
    }
}
