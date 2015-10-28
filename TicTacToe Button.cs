using System.Windows.Controls;
using System.Windows.Media;

namespace TicTacToe
{
    enum Moved { Not, Player, Computer }

    class TicTacToeButton : Button
    {
        public void Cross()
        {
            Foreground = new SolidColorBrush(Colors.Blue);
            Content = "X";
            Moved = Moved.Player;
        }

        public void Clear()
        {
            Content = string.Empty;
            Moved = Moved.Not;
        }

        public Moved Moved;

        public void Nought()
        {
            Foreground = new SolidColorBrush(Colors.Red);
            Content = "O";
            Moved = Moved.Computer;
        }
    }
}