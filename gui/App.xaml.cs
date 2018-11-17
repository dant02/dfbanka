using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using dfbanka.gui.api;
using dfbanka.gui.core;
using Newtonsoft.Json;
using static dfbanka.gui.components.OrdersPage;

namespace dfbanka.gui
{
    public partial class App : Application
    {
        private static Mutex mutex = new Mutex(true, "B8EE3594-F46F-4063-BE1F-5C311AD3A58E");
        private bool hasAquiredMutext = false;

        public App() : base()
        {
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Runner.Instance.Dispose();

            if (this.hasAquiredMutext)
                mutex.ReleaseMutex();

            mutex.Dispose();
            base.OnExit(e);
        }

        public ObservableCollection<Order> Orders { get; private set; }

        public ListCollectionView IncompleteOrders = null;
        public ListCollectionView InactiveOrders = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // https://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-wpf-application
            this.hasAquiredMutext = mutex.WaitOne(TimeSpan.Zero, true);

            if (!this.hasAquiredMutext)
            {
                MessageBox.Show("One instance is already running");
                Current.Shutdown(0);
            }

            this.Orders = new ObservableCollection<Order>();
            this.IncompleteOrders = new ListCollectionView(this.Orders)
            {
                Filter = (arg) =>
                {
                    var order = arg as Order;
                    return order.Status.Id != "completed" &&
                    order.Status.Id != "refunded" &&
                    order.Status.Id != "cancelled" &&
                    order.Status.Id != "failed";
                },
                IsLiveFiltering = true,
            };
            this.IncompleteOrders.LiveFilteringProperties.Add("Status");

            this.InactiveOrders = new ListCollectionView(this.Orders)
            {
                Filter = (arg) =>
                {
                    var order = arg as Order;
                    return order.Status.Id == "completed" ||
                    order.Status.Id == "refunded" ||
                    order.Status.Id == "cancelled" ||
                    order.Status.Id == "failed";
                },
                IsLiveFiltering = true
            };
            this.InactiveOrders.LiveFilteringProperties.Add("Status");

            MyWindow.Instance.Show();

            Runner.Instance.Start(MyWindow.Instance.Log);
        }

        public async Task UpdateOrder(string newStatus, long idOrder)
        {
            var config = Files.Load<Configuration>(Files.Paths.ConfigXml);

            string username = config.WordpressUsername;
            string password = config.WordpressPassword;
            string url = $"{config.WordpressUrl}/index.php/wp-json/wc/v2/orders/{idOrder}";

            string payload = JsonConvert.SerializeObject(new { status = newStatus });

            await WordPress.Put(username, password, url, payload);
        }
    }
}