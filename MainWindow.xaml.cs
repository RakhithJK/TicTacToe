using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            R = new Random();

            InitializeComponent();

            CurrentTurn.Cross();

            Buttons[11] = Button11;
            Buttons[12] = Button12;
            Buttons[13] = Button13;
            Buttons[21] = Button21;
            Buttons[22] = Button22;
            Buttons[23] = Button23;
            Buttons[31] = Button31;
            Buttons[32] = Button32;
            Buttons[33] = Button33;

            Level1.IsChecked = true;
        }

        void NewGame()
        {
            Done = 0;
            UpdateScores();
            Winner.Content = string.Empty;
            Winner.Visibility = Visibility.Collapsed;
            PlayerPanel.IsEnabled = true;
            if (Players == 1) LevelPanel.IsEnabled = true;

            foreach (var B in Buttons.Values)
            {
                B.Opacity = 1;
                B.IsEnabled = true;
                B.Clear();
            }

            Status.Content = "(c) Mathew Sachin";
            Game.Initialize();

            if (Players == 1)
            {
                if (!PlayerStarts) ComputerTurn();
            }
            else
            {
                if (PlayerStarts) CurrentTurn.Cross();
                else CurrentTurn.Nought();
            }
        }

        #region Statics
        static readonly int[,] Ways = new int[9, 4]
        {
           { 0, 0, 0, 0 },
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
        static Random R;
        #endregion

        #region Fields
        int MyWay, MyNextMove, Temporary, Done;
        bool PlayerStarts = true;

        int[] Game = new int[9];
        Dictionary<int, TicTacToeButton> Buttons = new Dictionary<int, TicTacToeButton>();
        #endregion

        #region ScoreBoard
        int[] PCWins = { 0, 0, 0, 0 };
        int[] PlayerWins = { 0, 0, 0, 0 };
        int[] Draws = { 0, 0, 0, 0 };

        int Player1Wins = 0, Player2Wins = 0, Player2Draws = 0;

        void UpdateScores()
        {
            if (Players == 1)
            {
                CompThisLevel.Content = PCWins[Level];
                CompTotal.Content = PCWins[0] + PCWins[1] + PCWins[2] + PCWins[3];

                PlayerThisLevel.Content = PlayerWins[Level];
                PlayerTotal.Content = PlayerWins[0] + PlayerWins[1] + PlayerWins[2] + PlayerWins[3];

                DrawsThisLevel.Content = Draws[Level];
                DrawsTotal.Content = Draws[0] + Draws[1] + Draws[2] + Draws[3];
            }
            else
            {
                Player1Total.Content = Player1Wins;
                Player2Total.Content = Player2Wins;
                Draws2Total.Content = Player2Draws;
            }
        }

        void FindWinner(Occupier Occupier)
        {
            bool IsWon = false;

            for (int n = 1; n <= 8; ++n)
            {
                if ((Buttons[Ways[n, 1]].OccupiedBy == Occupier) && (Buttons[Ways[n, 2]].OccupiedBy == Occupier) && (Buttons[Ways[n, 3]].OccupiedBy == Occupier))
                {
                    IsWon = true;
                    break;
                }
            }

            if (IsWon)
            {
                if (Occupier == Occupier.Player1)
                {
                    if (Players == 1) PlayerWins[Level]++;
                    else Player1Wins++;
                    PlayerStarts = true;
                    AnnounceResult(Players == 1 ? "You Won" : "Player 1 Won");
                }

                else
                {
                    if (Players == 1) PCWins[Level]++;
                    else Player2Wins++;
                    PlayerStarts = false;
                    AnnounceResult(Players == 1 ? "Computer Won" : "Player 2 Won");
                }
            }

            else
            {
                if (Done > 8)
                {
                    if (Players == 1) Draws[Level]++;
                    else Player2Draws++;
                    PlayerStarts = !PlayerStarts;
                    AnnounceResult("Draw");
                }

                else if (Occupier == Occupier.Player1 && Players == 1) ComputerTurn();
            }
        }

        void AnnounceResult(string Result)
        {
            Winner.Content = Result;
            Winner.Visibility = Visibility.Visible;
            LevelPanel.IsEnabled = PlayerPanel.IsEnabled = false;

            foreach (Button B in Buttons.Values)
            {
                B.Opacity = 0.2;
                B.IsEnabled = false;
            }

            new Thread(delegate()
            {
                Thread.Sleep(3000);
                Dispatcher.Invoke(new Action(delegate() { NewGame(); }));
            }).Start();
        }
        #endregion

        #region Level
        int level = 1;

        int Level
        {
            get { return level; }
            set
            {
                if (level != value) level = value;
                NewGame();
            }
        }

        void ChangeLevel(object sender, RoutedEventArgs e) { Level = int.Parse(((RadioButton)sender).Name.Remove(0, 5)); }
        #endregion

        #region Players
        int Players = 1;

        void TwoPlayer(object sender, RoutedEventArgs e)
        {
            LevelPanel.IsEnabled = false;
            Players = 2;
            OnePlayerScoreboard.Visibility = Visibility.Collapsed;
            TwoPlayerScoreboard.Visibility = Visibility.Visible;
            CurrentTurn.Cross();

            NewGame();
        }

        void OnePlayer(object sender, RoutedEventArgs e)
        {
            try
            {
                LevelPanel.IsEnabled = true;
                Players = 1;
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
            Temporary = 0;
            ComputerStrategy(true);
            if (Temporary == 0) ComputerStrategy(false);
            if (Temporary == 0 && Level > 1) ComputerDontLose();

            // Random
            if (Temporary == 0)
            {
                do Temporary = Choices[R.Next(0, 9)];
                while (Buttons[Temporary].OccupiedBy != 0);
            }

            Game[Done] = Temporary;
            Buttons[Temporary].Nought();
            Done++;
            FindWinner(Occupier.ComputerOrPlayer2);
        }

        void ComputerStrategy(bool IsToWin)
        {
            if (Level > 0)
            {
                Occupier Occupier = IsToWin ? Occupier.ComputerOrPlayer2 : Occupier.Player1;
                
                for (int n = 1; n <= 8; n++)
                {
                    if ((Buttons[Ways[n, 1]].OccupiedBy == Occupier) 
                        && (Buttons[Ways[n, 2]].OccupiedBy == Occupier) 
                        && (Buttons[Ways[n, 3]].OccupiedBy == 0))
                        Temporary = Ways[n, 3];

                    if ((Buttons[Ways[n, 1]].OccupiedBy == Occupier) 
                        && (Buttons[Ways[n, 3]].OccupiedBy == Occupier) 
                        && (Buttons[Ways[n, 2]].OccupiedBy == 0))
                        Temporary = Ways[n, 2];

                    if ((Buttons[Ways[n, 2]].OccupiedBy == Occupier) 
                        && (Buttons[Ways[n, 3]].OccupiedBy == Occupier) 
                        && (Buttons[Ways[n, 1]].OccupiedBy == 0))
                        Temporary = Ways[n, 1];
                }
            }
        }

        void ComputerDontLose()
        {
            if (!PlayerStarts)
            {
                if (Done == 0)
                {
                    Temporary = Choices[2 * (int)Math.Floor(R.NextDouble() * 5)];
                    if (Temporary == 22) MyWay = 1;
                    else MyWay = 2;
                }

                else if (Done == 2)
                {
                    if (MyWay == 1)
                    {
                        if (Game[1] == 11 || Game[1] == 13 || Game[1] == 31 || Game[1] == 33)
                            Temporary = 44 - Game[1];

                        else
                        {
                            int dlta = 22 - Game[1],
                                op0 = 22 + dlta + (10 / dlta),
                                op1 = 22 + dlta - (10 / dlta);

                            Temporary = R.Next(0, 2) == 1 ? op1 : op0;
                        }
                    }
                    else if (MyWay == 2)
                    {
                        if (Game[1] == 22)
                        {
                            Temporary = 44 - Game[0];
                            MyWay = 21;
                        }

                        else if (Game[1] == 11 || Game[1] == 13 || Game[1] == 31 || Game[1] == 33)
                        {
                            SelectCorner(true);
                            MyWay = 22;
                        }

                        else
                        {
                            Temporary = 22;
                            MyWay = 23;
                        }
                    }
                }

                else if (Done == 4)
                {
                    if (MyWay == 22)
                    {
                        for (int i = 0; i < 4; i++)
                            if (Buttons[Corners[i]].OccupiedBy == 0)
                                Temporary = Corners[i];
                    }

                    else if (MyWay == 23)
                    {
                        int dlta = Game[1] - Game[0], 
                            op0 = 44 - (Game[1] + dlta),
                            op1 = (op0 + Game[0]) / 2;

                        Temporary = R.Next(0, 2) == 1 ? op1 : op0;
                    }

                    else if (MyWay == 1)
                        Temporary = 44 + Game[2] - (2 * Game[3]);
                }
            }

            else if (Level == 3)
            {
                if (Done == 1)
                {
                    if (Game[0] == 11 || Game[0] == 13 || Game[0] == 31 || Game[0] == 33)
                    {
                        Temporary = 22;
                        MyWay = 1;
                    }

                    else if (Game[0] == 22)
                    {
                        SelectCorner(false);
                        MyWay = 2;
                    }

                    else
                    {
                        Temporary = 22;
                        MyWay = 3;
                    }
                }

                else if (Done == 3)
                {
                    if (MyWay == 1)
                    {
                        if (Game[2] == 44 - Game[0]) 
                            Temporary = Choices[1 + (2 * (int)Math.Floor(R.NextDouble() * 4))];
                        else Temporary = 44 - Game[0];
                    }

                    else if (MyWay == 2)
                    {
                        if (Game[2] == 44 - Game[1]) 
                            SelectCorner(true);
                    }

                    else if (MyWay == 3)
                    {
                        if (Game[2] == 11 || Game[2] == 13 || Game[2] == 31 || Game[2] == 33) Temporary = 44 - Game[2];
                        
                        if (Game[2] == 44 - Game[0])
                        {
                            int dlta = 22 - Game[2];
                            Temporary = 22 + (10 / dlta);
                            MyNextMove = Temporary + dlta;
                        }

                        else
                        {
                            int dlta = 22 - Game[0], 
                                op0 = Game[0] + (10 / dlta), 
                                op1 = Game[0] - (10 / dlta),
                                op2 = Game[2] + dlta;
                            
                            switch (R.Next(0, 3))
                            {
                                case 0: Temporary = op0;
                                    break;

                                case 1: Temporary = op1;
                                    break;

                                case 2: Temporary = op2;
                                    break;
                            }
                        }
                    }
                }

                else if (Done == 5 && MyWay == 3) Temporary = MyNextMove;
            }
        }
        #endregion

        void SelectCorner(bool Empty)
        {
            if (Empty)
            {
                do Temporary = Corners[R.Next(0, 4)];
                while (Buttons[Temporary].OccupiedBy != 0);
            }
            else Temporary = Corners[R.Next(0, 4)];
        }

        void Clicked(object sender, RoutedEventArgs e)
        {
            Status.Content = "(c) Mathew Sachin";
            int CellNumber = (sender as TicTacToeButton).CellNum;

            if (Buttons[CellNumber].OccupiedBy == Occupier.None)
            {
                if (Players == 1)
                {
                    Buttons[CellNumber].Cross();
                    Game[Done++] = CellNumber;
                    FindWinner(Occupier.Player1);
                }
                else
                {
                    if (CurrentTurn.OccupiedBy == Occupier.Player1) Buttons[CellNumber].Cross();
                    else Buttons[CellNumber].Nought();

                    Game[Done++] = CellNumber;

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

        #region Window Chrome
        void Minimise(object sender, MouseButtonEventArgs e) { WindowState = WindowState.Minimized; }

        void Exit(object sender, MouseButtonEventArgs e) { Close(); }

        void Drag(object sender, MouseButtonEventArgs e) { DragMove(); }
        #endregion
    }
}
