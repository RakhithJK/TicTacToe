namespace TicTacToe
{
    public class StatsViewModel : ViewModelBase
    {
        public TwoPlayerStats TwoPlayer { get; } = new TwoPlayerStats();

        public SinglePlayerStats SinglePlayer { get; } = new SinglePlayerStats();
    }
}