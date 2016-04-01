using System.Windows.Controls;
using System.Windows.Media;

namespace TicTacToe
{
    enum Occupier { None, Player1, ComputerOrPlayer2 }

    class TicTacToeButton : Button
    {
        public void Cross()
        {
            Foreground = new SolidColorBrush(Colors.Blue);
            Content = "X";
            OccupiedBy = Occupier.Player1;
        }

        public void Clear()
        {
            Content = string.Empty;
            OccupiedBy = Occupier.None;
        }

        public void Toggle()
        {
            switch (OccupiedBy)
            {
                case Occupier.Player1:
                    Nought();
                    break;

                default:
                    Cross();
                    break;
            }
        }

        public int CellNum => int.Parse(Name.Remove(0, 6));

        public Occupier OccupiedBy { get; private set; }

        public void Nought()
        {
            Foreground = new SolidColorBrush(Colors.Red);
            Content = "O";
            OccupiedBy = Occupier.ComputerOrPlayer2;
        }
    }
}