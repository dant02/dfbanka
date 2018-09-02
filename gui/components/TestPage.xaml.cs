using System;
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

            config = MyWindow.Appka.LoadConfiguration();

            if (config == null)
                throw new InvalidOperationException("Nepodařilo se nahrát konfiguraci");
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

                    if (!MyWindow.Appka.Orders.ContainsKey(order.Id))
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            MyWindow.Appka.Orders.Add(order.Id, order);
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
    }
}