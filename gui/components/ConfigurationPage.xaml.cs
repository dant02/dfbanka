using System;
using System.Windows.Controls;

namespace dfbanka.gui.components
{
    public partial class ConfigurationPage : StackPanel
    {
        private static Lazy<ConfigurationPage> lazy = new Lazy<ConfigurationPage>(() => new ConfigurationPage());

        public static ConfigurationPage Instance { get { return lazy.Value; } }

        private ConfigurationPage()
        {
            InitializeComponent();
        }

        public void SetConfiguration(Configuration configuration)
        {
            fioUrlTxBx.Text = configuration.BankaUrl;
            fioTokenTxBx.Password = configuration.BankaToken;
            wpUrlTxBx.Text = configuration.WordpressUrl;
            wpNameTxBx.Text = configuration.WordpressUsername;
            wpPsswdTxBx.Password = configuration.WordpressPassword;

            mailServerAddressTxBx.Text = configuration.MailServerAddress;
            mailServerPortTxBx.Text = configuration.MailServerPort.ToString();
            mailUsernameTxBx.Text = configuration.MailUsername;
            mailPasswordTxBx.Password = configuration.MailPassword;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool hasPort = int.TryParse(mailServerPortTxBx.Text, out int port);

            var configuration = new Configuration()
            {
                BankaUrl = fioUrlTxBx.Text.Trim(),
                BankaToken = fioTokenTxBx.Password.Trim(),
                WordpressUrl = wpUrlTxBx.Text.Trim(),
                WordpressUsername = wpNameTxBx.Text.Trim(),
                WordpressPassword = wpPsswdTxBx.Password.Trim(),
                MailServerAddress = mailServerAddressTxBx.Text.Trim(),
                MailServerPort = hasPort ? port : 25,
                MailUsername = mailUsernameTxBx.Text.Trim(),
                MailPassword = mailPasswordTxBx.Password.Trim()
            };

            MyWindow.Appka.SaveConfiguration(configuration);
        }
    }
}