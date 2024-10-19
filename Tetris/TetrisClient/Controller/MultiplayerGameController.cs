using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using TetrisClient.Logic;
using TetrisClient.Ui.Controls;

namespace TetrisClient.Controller
{
    public class MultiplayerGameController : IGameController
    {
        private readonly HubConnection _connection;
        private readonly IGameController _controller;

        public TetrisControl Control { get; }
        public Game Game { get; }
        public bool ShiftDown { get; set; }
        public DispatcherTimer Timer { get; }

        public MultiplayerGameController(IGameController controller, HubConnection connection)
        {
            _connection = connection;
            _controller = controller;

            Control = _controller.Control;
            Game = _controller.Game;
            ShiftDown = _controller.ShiftDown;
            Timer = _controller.Timer;
            Timer.Tick += On_TimerTick;
        }

        public void CleanUp() => _controller.CleanUp();

        public void ClearFields() => _controller.ClearFields();

        public void SetFields() => _controller.SetFields();

        public bool ShouldIncrementTickSpeed() => _controller.ShouldIncrementTickSpeed();

        public void IncrementTickSpeed() => _controller.IncrementTickSpeed();

        public void On_KeyDown(KeyEventArgs e)
        {
            _controller.On_KeyDown(e);

            // Invokes the correct method on the servers' hub
            switch (e.Key)
            {
                case Key.R:
                    Task.Run(async () => await _connection.InvokeAsync(
                        "RotateShape", 
                        ShiftDown ? Direction.Left.ToString() : Direction.Right.ToString())
                    );
                    break;
                case Key.Down:
                    Task.Run(async () => await _connection.InvokeAsync("DropShape"));
                    break;
                case Key.Left:
                    Task.Run(async () => await _connection.InvokeAsync("MoveShape", Direction.Left.ToString()));
                    break;
                case Key.Right:
                    Task.Run(async () => await _connection.InvokeAsync("MoveShape", Direction.Right.ToString()));
                    break;
                case Key.Space:
                    Task.Run(async () => await _connection.InvokeAsync("HardDrop"));
                    break;
            }
        }

        public void On_KeyUp(KeyEventArgs e) => _controller.On_KeyUp(e);

        public void On_StartBtnClick() => _controller.On_StartBtnClick();

        public void On_StopBtnClick() => _controller.On_StopBtnClick();

        public void On_TimerTick(object sender, EventArgs e)
        {
            switch (Game.State)
            {
                // Only send events when the game has started
                case GameState.Started:
                    Task.Run(async () => await _connection.InvokeAsync("DropShape"));
                    break;
                case GameState.Ended:
                    Control.ShowPanel("GAME OVER");
                    Task.Run(async () => await _connection.InvokeAsync("GameOver"));
                    break;
            }
        }
    }
}
