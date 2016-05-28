using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace TicTacToe
{
    public class ComposingConverter : IValueConverter
    {
        public List<IValueConverter> Converters { get; } = new List<IValueConverter>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converters.Aggregate(value, (Current, Converter) => Converter.Convert(Current, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IEnumerable<IValueConverter>)Converters).Reverse().Aggregate(value, (Current, Converter) => Converter.ConvertBack(Current, targetType, parameter, culture));
        }
    }
}