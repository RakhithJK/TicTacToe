using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TicTacToe
{
    public partial class MainWindow
    {
        readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = DataContext as MainViewModel;
            
            _mainViewModel.PropertyChanged += MainViewModelOnPropertyChanged;

            _mainViewModel.GameOver += OnGameOver;
        }

        void MainViewModelOnPropertyChanged(object Sender, PropertyChangedEventArgs PropertyChangedEventArgs)
        {
            if (PropertyChangedEventArgs.PropertyName != nameof(_mainViewModel.Level))
                return;

            PlayerThisLevel.SetBinding(ContentProperty, new Binding($"Stats.SinglePlayer.Player.{_mainViewModel.Level}"));
            ComputerThisLevel.SetBinding(ContentProperty, new Binding($"Stats.SinglePlayer.Computer.{_mainViewModel.Level}"));
            DrawsThisLevel.SetBinding(ContentProperty, new Binding($"Stats.SinglePlayer.Draws.{_mainViewModel.Level}"));
        }

        async void OnGameOver(Occupier Winner)
        {
            WinnerLabel.Content = WinnerTextToDisplay(Winner);

            WinnerLabel.Visibility = Visibility.Visible;

            await Task.Delay(2000);

            WinnerLabel.Visibility = Visibility.Collapsed;

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
