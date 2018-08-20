using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace dfbanka.gui.components
{
    public partial class TestPage : Grid
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

            WebRequest request = WebRequest.Create(url);
            request.PreAuthenticate = true;
            var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            string responseStr = string.Empty;

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                responseStr = reader.ReadToEnd();

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
    }
}