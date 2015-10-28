using System.Windows.Controls;
using System.Windows.Media;

namespace TicTacToe
{
    class TicTacToeButton : Button
    {
        public void Cross()
        {
            Foreground = new SolidColorBrush(Colors.Blue);
            Content = "X";
        }

        public void Clear() { Content = string.Empty; }

        public void Nought()
        {
            Foreground = new SolidColorBrush(Colors.Red);
            Content = "O";
        }
    }
}