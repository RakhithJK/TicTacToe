using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TicTacToe
{
    public class OccupierToStyleConverter : IValueConverter
    {
        public Style BaseStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var occupier = (Occupier)value;

            var style = new Style();

            foreach (var setter in BaseStyle.Setters)
                style.Setters.Add(setter);

            style.Setters.Add(new Setter(ContentControl.ContentProperty, OccupierToContent(occupier)));
            style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(OccupierToColor(occupier))));

            return style;
        }

        static string OccupierToContent(Occupier Occupier)
        {
            switch (Occupier)
            {
                case Occupier.Player1:
                    return "O";

                case Occupier.ComputerOrPlayer2:
                    return "X";

                default:
                    return null;
            }
        }

        static Color OccupierToColor(Occupier Occupier)
        {
            switch (Occupier)
            {
                case Occupier.Player1:
                    return Colors.Red;

                default:
                    return Colors.Blue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}