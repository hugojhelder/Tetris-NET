using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TetrisClient.Logic;
using TetrisClient.Ui.Controls;

namespace TetrisClient.Controller
{
    public class GameController : IGameController
    {
        public TetrisControl Control { get; }
        public Game Game { get; }
        public bool ShiftDown { get; set; }
        public DispatcherTimer Timer { get; }

        private const int BaseGameSpeed = 750;
        private const int MaxGameSpeed = 250;
        private const int GameSpeedIncrementor = 25;

        private TextBlock Level { get; }
        private TextBlock Lines { get; }
        private TextBlock Score { get; }
        private Button StartButton { get; }
        private Button StopButton { get; }
        private int _previousLevel = 1;

        public GameController(
            TetrisControl tetrisControl,
            Grid previewGrid,
            (TextBlock, TextBlock, TextBlock) blocks,
            (Button, Button) buttons,
            int seed = 0
        )
        {
            Game = new Game(tetrisControl.InnerGrid, previewGrid, seed);
            Control = tetrisControl;

            var (levelText, lineText, scoreText) = blocks;
            Level = levelText;
            Lines = lineText;
            Score = scoreText;

            var (startButton, stopButton) = buttons;
            StartButton = startButton;
            StopButton = stopButton;

            Timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 700)
            };
            Timer.Tick += On_TimerTick;
        }

        public void CleanUp()
        {
            Timer.Stop();
        }

        public void ClearFields()
        {
            Control.ShowPanel();

            Score.Text = "0";
            Level.Text = "10";
            Lines.Text = "0";

            StartButton.IsEnabled = true;
        }

        public void SetFields()
        {
            Level.Text = $"{Game.Level}";
            Lines.Text = $"{Game.Lines}";
            Score.Text = $"{Game.Score}";
        }

        public bool ShouldIncrementTickSpeed()
        {
            if (_previousLevel == Game.Level) return false;

            _previousLevel = Game.Level;
            return true;
        }

        public void IncrementTickSpeed()
        {
            var gameSpeed = BaseGameSpeed - GameSpeedIncrementor * (Game.Level - 1);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, gameSpeed < MaxGameSpeed ? MaxGameSpeed : gameSpeed);
        }

        public void On_KeyDown(KeyEventArgs e)
        {
            if (Game.State != GameState.Started) return;
            if (e.Key == Key.LeftShift)
            {
                ShiftDown = true;
                return;
            }

            switch (e.Key)
            {
                case Key.R:
                    Game.RotateCurrentShape(ShiftDown ? Direction.Left : Direction.Right);
                    break;
                case Key.Down:
                    Game.MoveCurrentShape(Direction.Down);
                    Game.DrawShapes();
                    Game.IncrementScore();
                    break;
                case Key.Left:
                    Game.MoveCurrentShape(Direction.Left);
                    break;
                case Key.Right:
                    Game.MoveCurrentShape(Direction.Right);
                    break;
                case Key.Space:
                    Game.HardDrop();
                    break;
            }
        }

        public void On_KeyUp(KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift) return;

            ShiftDown = false;
        }

        public void On_StartBtnClick()
        {
            Control.HidePanel();

            Game.ResetScore();
            Game.StartGame();
            Game.DrawShapes();
            Timer.Start();

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        public void On_StopBtnClick()
        {
            Control.ShowPanel();

            Game.EndGame();
            Timer.Stop();

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        public void On_TimerTick(object sender, EventArgs e)
        {
            switch (Game.State)
            {
                case GameState.Waiting:
                    break;
                case GameState.Ended:
                    ClearFields();
                    Timer.Stop();
                    break;
                case GameState.Started:
                    Game.MoveCurrentShape(Direction.Down);
                    Game.DrawShapes();
                    SetFields();
                    break;
            }

            if (ShouldIncrementTickSpeed())
            {
                IncrementTickSpeed();
            }

            SetFields();
        }
    }
}