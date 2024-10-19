using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using TetrisClient.Controller;
using TetrisClient.Logic;

namespace TetrisClient.Ui
{
    public partial class MultiplayerWindow
    {
        private const string LocalEnvironmentUrl = "http://10.0.1.167:5000/TetrisHub";
        private readonly HubConnection _connection;

        private IGameController _p1Controller;

        // Player 2 doesn't need a controller, only a Game object
        private Game _p2Game;

        private bool IsOtherPlayerReady { get; set; }
        private bool IsReady { get; set; }
        private int PlayerOneSeed { get; set; }
        private int PlayerTwoSeed { get; set; }

        public MultiplayerWindow()
        {
            InitializeComponent();

            _connection = new HubConnectionBuilder()
                .WithUrl(LocalEnvironmentUrl)
                .Build();

            KeyDown += MultiplayerWindow_KeyDown;
            KeyUp += MultiplayerWindow_KeyUp;

            _connection.On<int>("ReadyUp", seed =>
            {
                IsOtherPlayerReady = true;
                PlayerTwoSeed = seed;

                if (IsReady)
                {
                    Dispatcher.BeginInvoke(new Action(StartGames));
                    return;
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    PlayerTwoTetrisControl.ShowPanel("READY");
                }));
            });

            _connection.On("DropShape", () =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _p2Game.MoveCurrentShape(Direction.Down);
                    _p2Game.DrawShapes();
                }));
            });
            
            _connection.On("HardDrop", () =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _p2Game.HardDrop();
                }));
            });

            _connection.On<string>("RotateShape", rotation =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _p2Game.RotateCurrentShape(rotation == "Left" ? Direction.Left : Direction.Right);
                }));
            });

            _connection.On<string>("MoveShape",
                direction =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _p2Game.MoveCurrentShape(direction == "Left" ? Direction.Left : Direction.Right);
                    }));
                });

            _connection.On("GameOver",
                () =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        PlayerTwoTetrisControl.ShowPanel("GAME OVER");
                    }));
                });
            
            _connection.On("QuitMatch",
                () =>
                {
                    IsOtherPlayerReady = false;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        PlayerTwoTetrisControl.ShowPanel("QUIT");
                    }));
                });

            Task.Run(async () => await _connection.StartAsync());
        }

        private void MultiplayerWindow_KeyDown(object sender, KeyEventArgs e) => _p1Controller.On_KeyDown(e);

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // We don't want double the events...
            if (e.Key != Key.Space) return;

            // The spacebar key is not registered by the KeyDown event
            // https://stackoverflow.com/a/17628655
            _p1Controller.On_KeyDown(e);
        }

        private void MultiplayerWindow_KeyUp(object sender, KeyEventArgs e) => _p1Controller.On_KeyUp(e);

        private void StartGames()
        {
            // Wrap the normal GameController with the Multiplayer Controller,
            // so commands can be sent to the server
            _p1Controller = new MultiplayerGameController(
                new GameController(
                    PlayerOneTetrisControl,
                    Preview,
                    (LevelText, LinesText, ScoreText),
                    (StartButton, StopButton),
                    PlayerOneSeed),
                _connection
            );
            _p1Controller.On_StartBtnClick();
            
            PlayerOneTetrisControl.HidePanel();
            PlayerTwoTetrisControl.HidePanel();

            _p2Game = new Game(PlayerTwoTetrisControl.InnerGrid, null, PlayerTwoSeed);
            _p2Game.StartGame();
            _p2Game.DrawShapes();

            IsReady = false;
            IsOtherPlayerReady = false;
        }

        private async void StartGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            IsReady = true;
            StartButton.IsEnabled = false;
            PlayerOneSeed = Guid.NewGuid().GetHashCode();
            
            await _connection.InvokeAsync("ReadyUp", PlayerOneSeed);

            // Only if the other player is ready, start the game
            if (IsOtherPlayerReady)
            {
                StartGames();
                return;
            }
            
            PlayerOneTetrisControl.ShowPanel("READY");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // Dispose of the connection when closing the window
            Task.Run(async() => await _connection.DisposeAsync());
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await _connection.InvokeAsync("QuitMatch");

                Dispatcher.Invoke(Close);
            });
        }
    }
}
