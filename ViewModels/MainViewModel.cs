using System;
using System.Linq;
using System.Media;

namespace TicTacToe
{
    public class MainViewModel : ViewModelBase
    {
        int _myWay, _myNextMove, _done;
        bool _playerStarts = true;

        CellViewModel _temporary;

        static readonly Random Random = new Random();

        public CellCollection Cells { get; }

        readonly CellViewModel[,] _waysToWin;
        readonly CellViewModel[] _game = new CellViewModel[9];

        Occupier _currentOccupier = Occupier.Player1;

        public Occupier CurrentTurn
        {
            get { return _currentOccupier; }
            private set
            {
                _currentOccupier = value;
                OnPropertyChanged();
            }
        }

        public StatsViewModel Stats { get; } = new StatsViewModel();

        public MainViewModel()
        {
            Cells = new CellCollection(Click);

            _waysToWin = new[,]
            {
                { Cells[0,0], Cells[0,1], Cells[0,2] },
                { Cells[1,0], Cells[1,1], Cells[1,2] },
                { Cells[2,0], Cells[2,1], Cells[2,2] },

                { Cells[0,0], Cells[1,0], Cells[2,0] },
                { Cells[0,1], Cells[1,1], Cells[2,1] },
                { Cells[0,2], Cells[1,2], Cells[2,2] },
                
                { Cells[0,0], Cells[1,1], Cells[2,2] },
                { Cells[0,2], Cells[1,1], Cells[2,0] }
            };
        }

        Level _level = Level.Intermediate;

        public Level Level
        {
            get { return _level; }
            set
            {
                if (_level == value)
                    return;

                _level = value;

                OnPropertyChanged();

                CurrentTurn = Occupier.Player1;

                StartNewGame();
            }
        }

        GameType _gameType = GameType.SinglePlayer;

        public GameType GameType
        {
            get { return _gameType; }
            set
            {
                if (_gameType == value)
                    return;

                _gameType = value;

                OnPropertyChanged();

                CurrentTurn = Occupier.Player1;

                StartNewGame();
            }
        }

        string _status = "Ready";

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
                    return;

                _status = value;

                OnPropertyChanged();
            }
        }

        public void StartNewGame()
        {
            _done = 0;

            foreach (var cell in Cells)
                cell.Occupier = Occupier.None;

            Status = "(c) Mathew Sachin";
            _game.Initialize();

            if (GameType == GameType.SinglePlayer)
            {
                if (!_playerStarts)
                    ComputerTurn();
            }
            else CurrentTurn = _playerStarts ? Occupier.Player1 : Occupier.ComputerOrPlayer2;
        }

        void FindWinner(Occupier Occupier)
        {
            var isWon = false;

            for (var n = 0; n < 8; ++n)
            {
                if (_waysToWin[n, 0].Occupier != Occupier
                    || _waysToWin[n, 1].Occupier != Occupier
                    || _waysToWin[n, 2].Occupier != Occupier)
                    continue;

                isWon = true;
                break;
            }

            if (isWon)
            {
                if (Occupier == Occupier.Player1)
                {
                    if (GameType == GameType.SinglePlayer)
                        Stats.SinglePlayer.Player.Increment(Level);

                    else Stats.TwoPlayer.Player1++;

                    _playerStarts = true;

                    GameOver?.Invoke(Occupier.Player1);
                }

                else
                {
                    if (GameType == GameType.SinglePlayer)
                        Stats.SinglePlayer.Computer.Increment(Level);

                    else Stats.TwoPlayer.Player2++;

                    _playerStarts = false;

                    GameOver?.Invoke(Occupier.ComputerOrPlayer2);
                }
            }

            else
            {
                if (_done > 8)
                {
                    if (GameType == GameType.SinglePlayer)
                        Stats.SinglePlayer.Draws.Increment(Level);

                    else Stats.TwoPlayer.Draws++;

                    _playerStarts = !_playerStarts;
                    GameOver?.Invoke(Occupier.None);
                }

                else if (Occupier == Occupier.Player1
                         && GameType == GameType.SinglePlayer)
                    ComputerTurn();
            }
        }

        #region Computer
        void ComputerTurn()
        {
            _temporary = null;

            ComputerStrategy(true);

            if (_temporary == null)
                ComputerStrategy(false);

            if (_temporary == null
                && Level > Level.Intermediate)
                ComputerDontLose();

            // Random
            if (_temporary == null)
            {
                do _temporary = Cells[Random.Next(0, 3),Random.Next(0, 3)];
                while (_temporary.Occupier != Occupier.None);
            }

            _game[_done] = _temporary;
            _temporary.Occupier = Occupier.ComputerOrPlayer2;

            _done++;

            FindWinner(Occupier.ComputerOrPlayer2);
        }

        void ComputerStrategy(bool IsToWin)
        {
            if (Level == Level.Beginner)
                return;

            var occupier = IsToWin ? Occupier.ComputerOrPlayer2 : Occupier.Player1;

            for (var n = 0; n < 8; n++)
            {
                if (_waysToWin[n, 0].Occupier == occupier
                    && _waysToWin[n, 1].Occupier == occupier
                    && _waysToWin[n, 2].Occupier == Occupier.None)
                    _temporary = _waysToWin[n, 2];

                if (_waysToWin[n, 0].Occupier == occupier
                    && _waysToWin[n, 2].Occupier == occupier
                    && _waysToWin[n, 1].Occupier == Occupier.None)
                    _temporary = _waysToWin[n, 1];

                if (_waysToWin[n, 1].Occupier == occupier
                    && _waysToWin[n, 2].Occupier == occupier
                    && _waysToWin[n, 0].Occupier == Occupier.None)
                    _temporary = _waysToWin[n, 0];
            }
        }

        void ComputerDontLose()
        {
            if (!_playerStarts)
            {
                switch (_done)
                {
                    case 0:
                        _temporary = Cells[2 * Random.Next(0, 5)];
                        
                        _myWay = _temporary == Cells[1,1] ? 1 : 2;
                        break;

                    case 2:
                        switch (_myWay)
                        {
                            case 1:
                                if (Cells.Corners.Contains(_game[1]))
                                    _temporary = Cells.GetByCellNum(44 - _game[1].CellNum);

                                else
                                {
                                    var dlta = 22 - _game[1].CellNum;

                                    _temporary = Cells.GetByCellNum(Random.Next(0, 2) == 1 ? 22 + dlta - 10 / dlta
                                                                    : 22 + dlta + 10 / dlta);
                                }
                                break;

                            case 2:
                                if (_game[1] == Cells[1,1])
                                {
                                    _temporary = Cells.GetByCellNum(44 - _game[0].CellNum);
                                    _myWay = 21;
                                }
                                else if (Cells.Corners.Contains(_game[1]))
                                {
                                    SelectCorner(true);
                                    _myWay = 22;
                                }
                                else
                                {
                                    _temporary = Cells[1,1];
                                    _myWay = 23;
                                }
                                break;
                        }
                        break;

                    case 4:
                        switch (_myWay)
                        {
                            case 22:
                                for (var i = 0; i < 4; i++)
                                    if (Cells.Corners[i].Occupier == Occupier.None)
                                        _temporary = Cells.Corners[i];
                                break;

                            case 23:
                                int dlta = _game[1].CellNum - _game[0].CellNum,
                                    op0 = 44 - (_game[1].CellNum + dlta),
                                    op1 = (op0 + _game[0].CellNum) / 2;

                                _temporary = Cells.GetByCellNum(Random.Next(0, 2) == 1 ? op1 : op0);
                                break;

                            case 1:
                                _temporary = Cells.GetByCellNum(44 + _game[2].CellNum - 2 * _game[3].CellNum);
                                break;
                        }
                        break;
                }
            }

            else if (Level == Level.Expert)
            {
                switch (_done)
                {
                    case 1:
                        if (Cells.Corners.Contains(_game[0]))
                        {
                            _temporary = Cells[1,1];
                            _myWay = 1;
                        }
                        else if (_game[0] == Cells[1,1])
                        {
                            SelectCorner(false);
                            _myWay = 2;
                        }
                        else
                        {
                            _temporary = Cells[1,1];
                            _myWay = 3;
                        }
                        break;

                    case 3:
                        switch (_myWay)
                        {
                            case 1:
                                if (_game[2] == Cells.GetByCellNum(44 - _game[0].CellNum))
                                    _temporary = Cells[1 + 2 * Random.Next(0, 4)];

                                else _temporary = Cells.GetByCellNum(44 - _game[0].CellNum);
                                break;

                            case 2:
                                if (_game[2] == Cells.GetByCellNum(44 - _game[1].CellNum))
                                    SelectCorner(true);
                                break;

                            case 3:
                                if (Cells.Corners.Contains(_game[2]))
                                    _temporary = Cells.GetByCellNum(44 - _game[2].CellNum);

                                if (_game[2] == Cells.GetByCellNum(44 - _game[0].CellNum))
                                {
                                    var dlta = 22 - _game[2].CellNum;
                                    _temporary = Cells.GetByCellNum(22 + 10 / dlta);
                                    _myNextMove = _temporary.CellNum + dlta;
                                }

                                else
                                {
                                    var dlta = 22 - _game[0].CellNum;

                                    switch (Random.Next(0, 3))
                                    {
                                        case 0:
                                            _temporary = Cells.GetByCellNum(_game[0].CellNum + 10 / dlta);
                                            break;

                                        case 1:
                                            _temporary = Cells.GetByCellNum(_game[0].CellNum - 10 / dlta);
                                            break;

                                        case 2:
                                            _temporary = Cells.GetByCellNum(_game[2].CellNum + dlta);
                                            break;
                                    }
                                }
                                break;
                        }
                        break;

                    default:
                        if (_done == 5 && _myWay == 3)
                            _temporary = Cells.GetByCellNum(_myNextMove);
                        break;
                }
            }
        }
        
        void SelectCorner(bool Empty)
        {
            if (Empty)
            {
                do _temporary = Cells.Corners[Random.Next(0, 4)];
                while (_temporary.Occupier != Occupier.None);
            }

            else _temporary = Cells.Corners[Random.Next(0, 4)];
        }
        #endregion
        
        void Click(CellViewModel Cell)
        {
            Status = "(c) Mathew Sachin";
            
            if (Cell.Occupier == Occupier.None)
            {
                if (GameType == GameType.SinglePlayer)
                {
                    Cell.Occupier = Occupier.Player1;
                    _game[_done++] = Cell;
                    FindWinner(Occupier.Player1);
                }
                else
                {
                    Cell.Occupier = CurrentTurn;

                    _game[_done++] = Cell;

                    FindWinner(CurrentTurn);
                    CurrentTurn = CurrentTurn == Occupier.Player1 ? Occupier.ComputerOrPlayer2 : Occupier.Player1;
                }
            }
            else
            {
                SystemSounds.Asterisk.Play();
                Status = "You Cannot Move Here!";
            }
        }

        public event GameOverEventHandler GameOver;
    }
}