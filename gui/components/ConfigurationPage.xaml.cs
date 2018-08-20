using System.Windows.Controls;

namespace dfbanka.gui.components
{
    public partial class ConfigurationPage : StackPanel
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var configuration = new Configuration()
            {
                BankaUrl = fioUrlTxBx.Text.Trim(),
                BankaToken = fioTokenTxBx.Text.Trim(),
                WordpressUrl = wpUrlTxBx.Text.Trim(),
                WordpressUsername = wpNameTxBx.Text.Trim(),
                WordpressPassword = wpPsswdTxBx.Text.Trim()
            };

            MyWindow.Appka.SaveConfiguration(configuration);
        }
    }
}