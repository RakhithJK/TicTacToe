namespace TicTacToe
{
    public class SinglePlayerStats
    {
        public WinsPerLevel Player { get; } = new WinsPerLevel();

        public WinsPerLevel Computer { get; } = new WinsPerLevel();

        public WinsPerLevel Draws { get; } = new WinsPerLevel();
    }
}