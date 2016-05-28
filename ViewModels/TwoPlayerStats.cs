namespace TicTacToe
{
    public class TwoPlayerStats : ViewModelBase
    {
        int _player1;

        public int Player1
        {
            get { return _player1; }
            set
            {
                _player1 = value;

                OnPropertyChanged();
            }
        }

        int _player2;

        public int Player2
        {
            get { return _player2; }
            set
            {
                _player2 = value;

                OnPropertyChanged();
            }
        }

        int _draws;

        public int Draws
        {
            get { return _draws; }
            set
            {
                _draws = value;

                OnPropertyChanged();
            }
        }
    }
}