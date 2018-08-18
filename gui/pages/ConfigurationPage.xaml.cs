using System.Windows.Controls;

namespace dfbanka.gui.pages
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
                TokenFioBanka = fioTokenTxBx.Text.Trim(),
                UsernameWordPress = wpNameTxBx.Text.Trim(),
                PasswordWordPress = wpPsswdTxBx.Text.Trim()
            };

            MyWindow.Appka.SaveConfiguration(configuration);
        }
    }
}