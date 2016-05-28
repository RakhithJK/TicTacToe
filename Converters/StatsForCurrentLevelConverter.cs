using System;
using System.Globalization;
using System.Windows.Data;

namespace TicTacToe
{
    // TODO: This Level Stats don't Update Automatically.
    public class StatsForCurrentLevelConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var winsPerLevel = value[0] as WinsPerLevel;

            var level = (Level)value[1];

            switch (level)
            {
                case Level.Beginner:
                    return winsPerLevel.Beginner;

                case Level.Intermediate:
                    return winsPerLevel.Intermediate;

                case Level.Advanced:
                    return winsPerLevel.Advanced;

                case Level.Expert:
                    return winsPerLevel.Expert;

                default:
                    return winsPerLevel.Total;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) => null;
    }
}