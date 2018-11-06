using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using dfbanka.gui.api;
using dfbanka.gui.components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dfbanka.gui.core
{
    internal class Job
    {
        public TimeSpan Interval { get; } = TimeSpan.FromSeconds(120);
        public DateTime LastRunUtc { get; set; } = DateTime.MinValue;

        private readonly ILog log = null;

        public Job(ILog log)
        {
            this.log = log;
        }

        public async Task Run()
        {
            var config = Files.Load<Configuration>(Files.Paths.ConfigXml);

            if (config == null)
            {
                this.log.Print("No configuration loaded");
                return;
            }

            await this.DownloadOrders(config);

            await this.DownloadBank(config);

            // send emails
        }

        private async Task DownloadOrders(Configuration config)
        {
            if (string.IsNullOrWhiteSpace(config.WordpressUrl))
            {
                this.log.Print("Missing wordpress url");
                return;
            }

            string username = config.WordpressUsername;
            string password = config.WordpressPassword;
            string url = $"{config.WordpressUrl}/index.php/wp-json/wc/v2/orders";

            string responseStr = await WordPress.Get(username, password, url);
#if DEBUG
            Files.Save(Files.Paths.OrdersJson, responseStr);
#endif

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
        }

        private async Task DownloadBank(Configuration config)
        {
            if (string.IsNullOrWhiteSpace(config.BankaToken))
            {
                this.log.Print("Missing bank token");
                return;
            }

            var now = DateTime.Now;
            string url = config.BankaUrl;
            string from = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
            string to = new DateTime(now.Year, now.Month, 1).AddMonths(1).ToString("yyyy-MM-dd");
            string data = null;

            using (var client = new WebClient())
            {
                data = await client.DownloadStringTaskAsync(new Uri($"{url}/ib_api/rest/periods/{config.BankaToken}/{from}/{to}/transactions.xml"));
            }

#if DEBUG
            Files.Save(Files.Paths.BankXml, data);
#endif

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(data);

            var transactions = xDoc.SelectNodes("/AccountStatement/TransactionList/Transaction");

            foreach (XmlNode transaction in transactions)
            {
                var variableSymbol = transaction.SelectSingleNode("column_5");

                if (variableSymbol != null && !string.IsNullOrWhiteSpace(variableSymbol.InnerXml))
                {
                    var order = MyWindow.Appka.Orders.FirstOrDefault(f => string.Equals(f.InvoiceNr, variableSymbol.InnerXml, StringComparison.OrdinalIgnoreCase));

                    if (order == null)
                        continue;

                    var volume = transaction.SelectSingleNode("column_1");
                    if (volume != null && !string.IsNullOrWhiteSpace(volume.InnerXml))
                    {
                        if (string.Equals(order.Total, volume.InnerXml) && order.Status.Id == OrdersPage.Status_Pending)
                            await MyWindow.Appka.UpdateOrder(OrdersPage.Status_Processing, order.Id);
                    }
                }
            }
        }
    }
}