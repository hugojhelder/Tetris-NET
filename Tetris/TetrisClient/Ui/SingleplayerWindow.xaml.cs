using System.Windows;
using System.Windows.Input;
using TetrisClient.Controller;

namespace TetrisClient.Ui
{
    /// <summary>
    /// Interaction logic for SinglePlayerWindow.xaml
    /// </summary>
    public partial class SinglePlayerWindow
    {
        private readonly IGameController _controller;

        public SinglePlayerWindow()
        {
            InitializeComponent();

            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyUp;

            _controller = new GameController(
                TetrisGrid,
                Preview,
                (LevelText, LinesText, ScoreText),
                (StartButton, StopButton)
            );
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e) => _controller.On_KeyDown(e);

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // We don't want double the events...
            if (e.Key != Key.Space) return;

            // The spacebar key is not registered by the KeyDown event
            // https://stackoverflow.com/a/17628655
            _controller.On_KeyDown(e);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e) => _controller.On_KeyUp(e);

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            _controller.On_StartBtnClick();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e) => _controller.On_StopBtnClick();
    }
}
