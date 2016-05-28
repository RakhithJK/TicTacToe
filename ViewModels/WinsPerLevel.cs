namespace TicTacToe
{
    public class WinsPerLevel : ViewModelBase
    {
        readonly int[] _wins = new int[5];

        public void Increment(Level Level)
        {
            ++Total;

            switch (Level)
            {
                case Level.Beginner:
                    ++Beginner;
                    break;

                case Level.Intermediate:
                    ++Intermediate;
                    break;

                case Level.Advanced:
                    ++Advanced;
                    break;

                case Level.Expert:
                    ++Expert;
                    break;
            }
        }
        
        public int Total
        {
            get { return _wins[4]; }
            private set
            {
                _wins[4] = value;
                OnPropertyChanged();
            }
        }

        public int Beginner
        {
            get { return _wins[0]; }
            private set
            {
                _wins[0] = value;
                OnPropertyChanged();
            }
        }

        public int Intermediate
        {
            get { return _wins[1]; }
            private set
            {
                _wins[1] = value;
                OnPropertyChanged();
            }
        }

        public int Advanced
        {
            get { return _wins[2]; }
            private set
            {
                _wins[2] = value;
                OnPropertyChanged();
            }
        }

        public int Expert
        {
            get { return _wins[3]; }
            private set
            {
                _wins[3] = value;
                OnPropertyChanged();
            }
        }
    }
}