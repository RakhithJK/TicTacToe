using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    public partial class MainWindow
    {
        #region Fields
        int _myWay, _myNextMove, _temporary, _done;
        bool _playerStarts = true;

        readonly int[] _game = new int[9];
        readonly Dictionary<int, TicTacToeButton> _buttons = new Dictionary<int, TicTacToeButton>();

        static readonly int[,] Ways =
        {
           { 0, 11, 12, 13 },
           { 0, 21, 22, 23 },
           { 0, 31, 32, 33 },
           { 0, 11, 21, 31 },
           { 0, 12, 22, 32 },
           { 0, 13, 23, 33 },
           { 0, 11, 22, 33 },
           { 0, 13, 22, 31 }
        };

        static readonly int[] Choices = { 11, 12, 13, 21, 22, 23, 31, 32, 33 };
        static readonly int[] Corners = { 11, 13, 31, 33 };
        static Random _r;
        #endregion

        public MainWindow()
        {
            _r = new Random();

            InitializeComponent();

            CurrentTurn.Cross();

            foreach (var i in Choices)
                _buttons[i] = FindName($"Button{i}") as TicTacToeButton;
            
            Level1.IsChecked = true;
        }

        void NewGame()
        {
            _done = 0;
            UpdateScores();
            Winner.Content = string.Empty;
            Winner.Visibility = Visibility.Collapsed;
            PlayerPanel.IsEnabled = true;

            if (_players == 1)
                LevelPanel.IsEnabled = true;

            foreach (var b in _buttons.Values)
            {
                b.Opacity = 1;
                b.IsEnabled = true;
                b.Clear();
            }

            Status.Content = "(c) Mathew Sachin";
            _game.Initialize();

            if (_players == 1)
            {
                if (!_playerStarts)
                    ComputerTurn();
            }
            else
            {
                if (_playerStarts)
                    CurrentTurn.Cross();
                else CurrentTurn.Nought();
            }
        }
        
        #region ScoreBoard
        readonly int[] _pcWins = new int[4],
                       _playerWins = new int[4],
                       _draws = new int[4];

        int _player1Wins, _player2Wins, _player2Draws;

        void UpdateScores()
        {
            if (_players == 1)
            {
                CompThisLevel.Content = _pcWins[Level];
                CompTotal.Content = _pcWins[0] + _pcWins[1] + _pcWins[2] + _pcWins[3];

                PlayerThisLevel.Content = _playerWins[Level];
                PlayerTotal.Content = _playerWins[0] + _playerWins[1] + _playerWins[2] + _playerWins[3];

                DrawsThisLevel.Content = _draws[Level];
                DrawsTotal.Content = _draws[0] + _draws[1] + _draws[2] + _draws[3];
            }
            else
            {
                Player1Total.Content = _player1Wins;
                Player2Total.Content = _player2Wins;
                Draws2Total.Content = _player2Draws;
            }
        }

        void FindWinner(Occupier Occupier)
        {
            var isWon = false;

            for (var n = 0; n < 8; ++n)
            {
                if (_buttons[Ways[n, 1]].OccupiedBy != Occupier
                    || _buttons[Ways[n, 2]].OccupiedBy != Occupier
                    || _buttons[Ways[n, 3]].OccupiedBy != Occupier)
                    continue;

                isWon = true;
                break;
            }

            if (isWon)
            {
                if (Occupier == Occupier.Player1)
                {
                    if (_players == 1)
                        _playerWins[Level]++;

                    else _player1Wins++;

                    _playerStarts = true;

                    AnnounceResult(_players == 1 ? "You Won" : "Player 1 Won");
                }

                else
                {
                    if (_players == 1)
                        _pcWins[Level]++;

                    else _player2Wins++;

                    _playerStarts = false;

                    AnnounceResult(_players == 1 ? "Computer Won" : "Player 2 Won");
                }
            }

            else
            {
                if (_done > 8)
                {
                    if (_players == 1)
                        _draws[Level]++;

                    else _player2Draws++;

                    _playerStarts = !_playerStarts;
                    AnnounceResult("Draw");
                }

                else if (Occupier == Occupier.Player1
                         && _players == 1)
                    ComputerTurn();
            }
        }

        void AnnounceResult(string Result)
        {
            Winner.Content = Result;
            Winner.Visibility = Visibility.Visible;
            LevelPanel.IsEnabled = PlayerPanel.IsEnabled = false;

            foreach (var b in _buttons.Values)
            {
                b.Opacity = 0.2;
                b.IsEnabled = false;
            }

            new Thread(() =>
            {
                Thread.Sleep(3000);
                Dispatcher.Invoke(NewGame);
            }).Start();
        }
        #endregion

        #region Level
        int _level = 1;

        int Level
        {
            get { return _level; }
            set
            {
                if (_level == value)
                    return;

                _level = value;
                NewGame();
            }
        }

        void ChangeLevel(object Sender, RoutedEventArgs E) => Level = int.Parse(((RadioButton)Sender).Name.Remove(0, 5));
        #endregion

        #region Players
        int _players = 1;

        void TwoPlayer(object Sender, RoutedEventArgs E)
        {
            LevelPanel.IsEnabled = false;
            _players = 2;
            OnePlayerScoreboard.Visibility = Visibility.Collapsed;
            TwoPlayerScoreboard.Visibility = Visibility.Visible;
            CurrentTurn.Cross();

            NewGame();
        }

        void OnePlayer(object Sender, RoutedEventArgs E)
        {
            try
            {
                LevelPanel.IsEnabled = true;
                _players = 1;
                OnePlayerScoreboard.Visibility = Visibility.Visible;
                TwoPlayerScoreboard.Visibility = Visibility.Collapsed;
                CurrentTurn.Cross();

                NewGame();
            }
            catch { }
        }
        #endregion

        #region Computer
        void ComputerTurn()
        {
            _temporary = 0;

            ComputerStrategy(true);

            if (_temporary == 0)
                ComputerStrategy(false);

            if (_temporary == 0 
                && Level > 1)
                ComputerDontLose();

            // Random
            if (_temporary == 0)
            {
                do _temporary = Choices[_r.Next(0, 9)];
                while (_buttons[_temporary].OccupiedBy != Occupier.None);
            }

            _game[_done] = _temporary;
            _buttons[_temporary].Nought();

            _done++;

            FindWinner(Occupier.ComputerOrPlayer2);
        }

        void ComputerStrategy(bool IsToWin)
        {
            if (Level <= 0)
                return;

            var occupier = IsToWin ? Occupier.ComputerOrPlayer2 : Occupier.Player1;
                
            for (var n = 0; n < 8; n++)
            {
                if (_buttons[Ways[n, 1]].OccupiedBy == occupier
                    && _buttons[Ways[n, 2]].OccupiedBy == occupier
                    && _buttons[Ways[n, 3]].OccupiedBy == Occupier.None)
                    _temporary = Ways[n, 3];

                if (_buttons[Ways[n, 1]].OccupiedBy == occupier
                    && _buttons[Ways[n, 3]].OccupiedBy == occupier
                    && _buttons[Ways[n, 2]].OccupiedBy == Occupier.None)
                    _temporary = Ways[n, 2];

                if (_buttons[Ways[n, 2]].OccupiedBy == occupier
                    && _buttons[Ways[n, 3]].OccupiedBy == occupier
                    && _buttons[Ways[n, 1]].OccupiedBy == Occupier.None)
                    _temporary = Ways[n, 1];
            }
        }

        void ComputerDontLose()
        {
            if (!_playerStarts)
            {
                switch (_done)
                {
                    case 0:
                        _temporary = Choices[2 * (int)Math.Floor(_r.NextDouble() * 5)];
                        _myWay = _temporary == 22 ? 1 : 2;
                        break;

                    case 2:
                        switch (_myWay)
                        {
                            case 1:
                                if (_game[1].Is(11, 13, 31, 33))
                                    _temporary = 44 - _game[1];

                                else
                                {
                                    var dlta = 22 - _game[1];

                                    _temporary = _r.Next(0, 2) == 1 ? 22 + dlta - 10 / dlta 
                                                                    : 22 + dlta + 10 / dlta;
                                }
                                break;

                            case 2:
                                switch (_game[1])
                                {
                                    case 22:
                                        _temporary = 44 - _game[0];
                                        _myWay = 21;
                                        break;

                                    case 11:
                                    case 13:
                                    case 31:
                                    case 33:
                                        SelectCorner(true);
                                        _myWay = 22;
                                        break;

                                    default:
                                        _temporary = 22;
                                        _myWay = 23;
                                        break;
                                }
                                break;
                        }
                        break;

                    case 4:
                        switch (_myWay)
                        {
                            case 22:
                                for (var i = 0; i < 4; i++)
                                    if (_buttons[Corners[i]].OccupiedBy == Occupier.None)
                                        _temporary = Corners[i];
                                break;

                            case 23:
                                int dlta = _game[1] - _game[0], 
                                    op0 = 44 - (_game[1] + dlta),
                                    op1 = (op0 + _game[0]) / 2;

                                _temporary = _r.Next(0, 2) == 1 ? op1 : op0;
                                break;

                            case 1:
                                _temporary = 44 + _game[2] - 2 * _game[3];
                                break;
                        }
                        break;
                }
            }

            else if (Level == 3)
            {
                switch (_done)
                {
                    case 1:
                        switch (_game[0])
                        {
                            case 11:
                            case 13:
                            case 31:
                            case 33:
                                _temporary = 22;
                                _myWay = 1;
                                break;

                            case 22:
                                SelectCorner(false);
                                _myWay = 2;
                                break;

                            default:
                                _temporary = 22;
                                _myWay = 3;
                                break;
                        }
                        break;

                    case 3:
                        switch (_myWay)
                        {
                            case 1:
                                if (_game[2] == 44 - _game[0]) 
                                    _temporary = Choices[1 + 2 * (int)Math.Floor(_r.NextDouble() * 4)];

                                else _temporary = 44 - _game[0];
                                break;

                            case 2:
                                if (_game[2] == 44 - _game[1]) 
                                    SelectCorner(true);
                                break;

                            case 3:
                                if (_game[2].Is(11, 13, 31, 33))
                                    _temporary = 44 - _game[2];
                        
                                if (_game[2] == 44 - _game[0])
                                {
                                    var dlta = 22 - _game[2];
                                    _temporary = 22 + 10 / dlta;
                                    _myNextMove = _temporary + dlta;
                                }

                                else
                                {
                                    var dlta = 22 - _game[0];
                            
                                    switch (_r.Next(0, 3))
                                    {
                                        case 0:
                                            _temporary = _game[0] + 10 / dlta;
                                            break;

                                        case 1:
                                            _temporary = _game[0] - 10 / dlta;
                                            break;

                                        case 2:
                                            _temporary = _game[2] + dlta;
                                            break;
                                    }
                                }
                                break;
                        }
                        break;

                    default:
                        if (_done == 5 && _myWay == 3)
                            _temporary = _myNextMove;
                        break;
                }
            }
        }
        #endregion

        void SelectCorner(bool Empty)
        {
            if (Empty)
            {
                do _temporary = Corners[_r.Next(0, 4)];
                while (_buttons[_temporary].OccupiedBy != Occupier.None);
            }

            else _temporary = Corners[_r.Next(0, 4)];
        }

        void Clicked(object Sender, RoutedEventArgs E)
        {
            Status.Content = "(c) Mathew Sachin";
            var cellNumber = (Sender as TicTacToeButton).CellNum;

            if (_buttons[cellNumber].OccupiedBy == Occupier.None)
            {
                if (_players == 1)
                {
                    _buttons[cellNumber].Cross();
                    _game[_done++] = cellNumber;
                    FindWinner(Occupier.Player1);
                }
                else
                {
                    if (CurrentTurn.OccupiedBy == Occupier.Player1)
                        _buttons[cellNumber].Cross();
                    else _buttons[cellNumber].Nought();

                    _game[_done++] = cellNumber;

                    FindWinner(CurrentTurn.OccupiedBy);
                    CurrentTurn.Toggle();
                }
            }
            else
            {
                SystemSounds.Asterisk.Play();
                Status.Content = "You Cannot Move Here!";
            }
        }
    }
}
