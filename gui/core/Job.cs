using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using dfbanka.gui.api;
using dfbanka.gui.components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dfbanka.gui.core
{
    internal class Job
    {
        public TimeSpan Interval { get; } = TimeSpan.FromSeconds(10);
        public DateTime LastRunUtc { get; set; } = DateTime.MinValue;

        private readonly ILog log = null;

        public Job(ILog log)
        {
            this.log = log;
        }

        public async Task Run()
        {
            var config = Files.Load<Configuration>(Files.Paths.ConfigXml);

            await this.DownloadOrders(config);

            //await this.DownloadBank(config);

            // update orders from bank info

            // send emails
        }

        private async Task DownloadOrders(Configuration config)
        {
            if (string.IsNullOrWhiteSpace(config.WordpressUrl))
                return;

            string username = config.WordpressUsername;
            string password = config.WordpressPassword;
            string url = $"{config.WordpressUrl}/index.php/wp-json/wc/v2/orders";

            string responseStr = await WordPress.Get(username, password, url);

            Files.Save(Files.Paths.OrdersJson, responseStr);

            if (JsonConvert.DeserializeObject(responseStr) is JArray obj)
                foreach (var tkn in obj)
                {
                    var order = new OrdersPage.Order(tkn);
                    var currentOrder = MyWindow.Appka.Orders.FirstOrDefault(f => f.Id == order.Id);
                    if (currentOrder != null)
                    {
                        currentOrder.UpdateStore(tkn);
                    }
                    else
                        App.Current.Dispatcher.Invoke(() => { MyWindow.Appka.Orders.Add(order); });
                }

            //MyWindow.Appka.IncompleteOrders.c
        }

        private async Task DownloadBank(Configuration config)
        {
            if (string.IsNullOrWhiteSpace(config.BankaToken))
                return;

            string url = config.BankaUrl;
            string from = "2018-08-01";
            string to = "2018-08-31";
            string data = null;

            using (var client = new WebClient())
            {
                data = await client.DownloadStringTaskAsync(new Uri($"{url}/ib_api/rest/periods/{config.BankaToken}/{from}/{to}/transactions.xml"));
            }

            Files.Save(Files.Paths.BankXml, data);
        }
    }
}