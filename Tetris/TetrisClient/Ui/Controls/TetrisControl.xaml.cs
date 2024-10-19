using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace TetrisClient.Ui.Controls
{
    public partial class TetrisControl
    {
        public TetrisControl()
        {
            InitializeComponent();
        }

        [Description("Property for setting the inner grid margin")]
        public Thickness GridMargin
        {
            set => InnerGrid.Margin = value;
        }
        
        
        [Description("Property for setting the inner grid margin")]
        public Thickness PanelMargin
        {
            set => TetrisTextPanel.Margin = value;
        }

        public void HidePanel() => TetrisTextPanel.Visibility = Visibility.Hidden;

        public void ShowPanel(string newText)
        {
            TetrisTextPanelText.Text = newText;
            ShowPanel();
        }
        
        public void ShowPanel() => TetrisTextPanel.Visibility = Visibility.Visible;
    }
}
