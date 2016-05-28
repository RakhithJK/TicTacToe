using System.Threading.Tasks;
using System.Windows;

namespace TicTacToe
{
    public partial class MainWindow
    {
        readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = DataContext as MainViewModel;
            
            _mainViewModel.GameOver += OnGameOver;
        }

        async void OnGameOver(Occupier Winner)
        {
            WinnerLabel.Content = WinnerTextToDisplay(Winner);

            WinnerLabel.Visibility = Visibility.Visible;

            await Task.Delay(2000);

            WinnerLabel.Visibility = Visibility.Collapsed;

            //TODO: Why do Buttons stay disabled?
            _mainViewModel.StartNewGame();
        }

        string WinnerTextToDisplay(Occupier Winner)
        {
            switch (Winner)
            {
                case Occupier.ComputerOrPlayer2:
                    return _mainViewModel.GameType == GameType.SinglePlayer ? "Computer Won" : "Player 2 Won";

                case Occupier.Player1:
                    return _mainViewModel.GameType == GameType.SinglePlayer ? "You Won" : "Player 1 Won";

                default:
                    return "Draw";
            }
        }
    }
}
