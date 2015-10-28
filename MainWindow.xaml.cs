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

        bool IsWon;

        void UpdateScores()
        {
            CompThisLevel.Content = PCWins[Level];
            CompTotal.Content = PCWins[0] + PCWins[1] + PCWins[2] + PCWins[3];

            PlayerThisLevel.Content = PlayerWins[Level];
            PlayerTotal.Content = PlayerWins[0] + PlayerWins[1] + PlayerWins[2] + PlayerWins[3];

            DrawsThisLevel.Content = Draws[Level];
            DrawsTotal.Content = Draws[0] + Draws[1] + Draws[2] + Draws[3];
        }

        void FindWinner(bool IsPlayer)
        {
            Moved me = IsPlayer ? Moved.Player : Moved.Computer;
            for (int n = 1; n <= 8; ++n)
            {
                if ((Buttons[Ways[n, 1]].Moved == me) && (Buttons[Ways[n, 2]].Moved == me) && (Buttons[Ways[n, 3]].Moved == me))
                {
                    IsWon = true;
                    break;
                }
            }
            if (IsWon)
            {
                if (IsPlayer)
                {
                    PlayerWins[Level]++;
                    PlayerStarts = true;
                    AnnounceResult("You Won");
                }
                else
                {
                    PCWins[Level]++;
                    PlayerStarts = false;
                    AnnounceResult("Computer Won");
                }
            }
            else
            {
                if (Done > 8)
                {
                    Draws[Level]++;
                    PlayerStarts = !PlayerStarts;
                    AnnounceResult("Draw");
                }
                else if (IsPlayer) PCTurn();
            }
        }

        void AnnounceResult(string Result)
        {
            Winner.Content = Result;
            Winner.Visibility = Visibility.Visible;
            LevelPanel.IsEnabled = false;

            foreach (Button B in Buttons.Values)
            {
                B.Opacity = 0.2;
                B.IsEnabled = false;
            }

            Reset();
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
                Initialise();
            }
        }

        void ChangeLevel(object sender, RoutedEventArgs e) { Level = int.Parse(((RadioButton)sender).Name.Remove(0, 5)); }
        #endregion

        public MainWindow()
        {
            R = new Random();

            InitializeComponent();

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

        void Initialise()
        {
            IsWon = false;
            Done = 0;
            UpdateScores();
            Winner.Content = string.Empty;
            Winner.Visibility = Visibility.Collapsed;
            LevelPanel.IsEnabled = true;

            foreach (var B in Buttons.Values)
            {
                B.Opacity = 1;
                B.IsEnabled = true;
                B.Clear();
            }

            Status.Content = "(c) Mathew Sachin";
            Game.Initialize();
            if (!PlayerStarts) PCTurn();
        }

        void Reset()
        {
            new Thread(delegate()
                {
                    Thread.Sleep(3000);
                    Dispatcher.Invoke(new Action(delegate() { Initialise(); }));
                }).Start();
        }

        #region PC
        void PCTurn()
        {
            Temporary = 0;
            PCStrategy(true);
            if (Temporary == 0) PCStrategy(false);
            if (Temporary == 0 && Level > 1) PCDontLose();

            // Random
            if (Temporary == 0)
            {
                do Temporary = Choices[R.Next(0, 9)];
                while (Buttons[Temporary].Moved != 0);
            }

            Game[Done] = Temporary;
            Buttons[Temporary].Nought();
            Done++;
            FindWinner(false);
        }
        
        void PCStrategy(bool IsToWin)
        {
            if (Level > 0)
            {
                Moved str = IsToWin ? Moved.Computer : Moved.Player;
                for (int n = 1; n <= 8; n++)
                {
                    if ((Buttons[Ways[n, 1]].Moved == str) && (Buttons[Ways[n, 2]].Moved == str) && (Buttons[Ways[n, 3]].Moved == 0)) Temporary = Ways[n, 3];
                    if ((Buttons[Ways[n, 1]].Moved == str) && (Buttons[Ways[n, 3]].Moved == str) && (Buttons[Ways[n, 2]].Moved == 0)) Temporary = Ways[n, 2];
                    if ((Buttons[Ways[n, 2]].Moved == str) && (Buttons[Ways[n, 3]].Moved == str) && (Buttons[Ways[n, 1]].Moved == 0)) Temporary = Ways[n, 1];
                }
            }
        }

        void PCDontLose()
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
                        if (Game[1] == 11 || Game[1] == 13 || Game[1] == 31 || Game[1] == 33) Temporary = 44 - Game[1];
                        else
                        {
                            int dlta = 22 - Game[1];
                            int op0 = 22 + dlta + (10 / dlta);
                            int op1 = 22 + dlta - (10 / dlta);
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
                        for (int i = 0; i < 4; i++) if (Buttons[Corners[i]].Moved == 0) Temporary = Corners[i];
                    }
                    else if (MyWay == 23)
                    {
                        int dlta = Game[1] - Game[0];
                        int op0 = 44 - (Game[1] + dlta);
                        int op1 = (op0 + Game[0]) / 2;
                        Temporary = R.Next(0, 2) == 1 ? op1 : op0;
                    }
                    else if (MyWay == 1) Temporary = 44 + Game[2] - (2 * Game[3]);
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
                        if (Game[2] == 44 - Game[0]) Temporary = Choices[1 + (2 * (int)Math.Floor(R.NextDouble() * 4))];
                        else Temporary = 44 - Game[0];
                    }
                    else if (MyWay == 2)
                    {
                        if (Game[2] == 44 - Game[1]) SelectCorner(true);
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
                            int dlta = 22 - Game[0];
                            int op0 = Game[0] + (10 / dlta);
                            int op1 = Game[0] - (10 / dlta);
                            int op2 = Game[2] + dlta;
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
                while (Buttons[Temporary].Moved != 0);
            }
            else Temporary = Corners[R.Next(0, 4)];
        }
        
        void ClickButton(object sender, RoutedEventArgs e)
        {
            Status.Content = "(c) Mathew Sachin";
            int CellNumber = (sender as TicTacToeButton).CellNum;

            if (!IsWon)
            {
                if (Buttons[CellNumber].Moved == Moved.Not)
                {
                    Buttons[CellNumber].Cross();
                    Game[Done] = CellNumber;
                    Done++;
                    FindWinner(true);
                }
                else
                {
                    SystemSounds.Asterisk.Play();
                    Status.Content = "You Cannot Move Here!";
                }
            }
        }
        
        #region Window Chrome
        void Minimise(object sender, MouseButtonEventArgs e) { WindowState = WindowState.Minimized; }

        void Exit(object sender, MouseButtonEventArgs e) { Close(); }

        void Drag(object sender, MouseButtonEventArgs e) { DragMove(); }
        #endregion
    }
}
