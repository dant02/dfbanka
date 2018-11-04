using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using dfbanka.gui.api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dfbanka.gui.components
{
    public partial class TestPage : StackPanel
    {
        private Configuration config = null;

        public TestPage()
        {
            InitializeComponent();

            config = Files.Load<Configuration>(Files.Paths.ConfigXml);

            if (config == null)
                config = new Configuration();
        }

        private async void BtnGetXml_Click(object sender, RoutedEventArgs e)
        {
            string url = this.config.BankaUrl;
            string token = this.config.BankaToken;
            string from = "2018-08-01";
            string to = "2018-08-31";
            byte[] data = null;

            using (var client = new WebClient())
            {
                data = await client.DownloadDataTaskAsync(new Uri($"{url}/ib_api/rest/periods/{token}/{from}/{to}/transactions.xml"));
            }

            if (data != null)
            { }
        }

        /// <summary>
        /// https://github.com/woocommerce/woocommerce/wiki/Getting-started-with-the-REST-API
        /// </summary>
        private async void BtnGetJson_Click(object sender, RoutedEventArgs e)
        {
            string username = config.WordpressUsername;
            string password = config.WordpressPassword;
            string url = $"{config.WordpressUrl}/index.php/wp-json/wc/v2/orders";

            string responseStr = await WordPress.Get(username, password, url);

            if (JsonConvert.DeserializeObject(responseStr) is JArray obj)
                foreach (var tkn in obj)
                {
                    var order = new OrdersPage.Order(tkn);

                    var currentOrder = MyWindow.Appka.Orders.FirstOrDefault(f => f.Id == order.Id);
                    if (currentOrder == null)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            MyWindow.Appka.Orders.Add(order);
                        });
                    }
                }
        }

        private async void BtnPutOrder_Click(object sender, RoutedEventArgs e)
        {
            string username = config.WordpressUsername;
            string password = config.WordpressPassword;
            int idOrder = 167;
            string url = $"{config.WordpressUrl}/index.php/wp-json/wc/v2/orders/{idOrder}";

            string payload = JsonConvert.SerializeObject(new { status = "completed" });

            await WordPress.Put(username, password, url, payload);
        }

        private void BtnMail_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(recipentTxBx.Text))
            {
                MessageBox.Show("Please fill recipent of test email");
                return;
            }

            Mail.Send(config.MailServerAddress, config.MailServerPort, config.MailUsername, config.MailPassword, recipentTxBx.Text.Trim());
        }
    }
}