using System.Windows;

namespace TetrisClient.Ui
{
    public partial class MenuWindow
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void StartSinglePlayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new SinglePlayerWindow().ShowDialog();
            Show();
        }

        private void StartMultiplayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new MultiplayerWindow().ShowDialog();
            Show();
        }
    }
}
