using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using dfbanka.gui.api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dfbanka.gui.components
{
    /// <summary>
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Border
    {
        // https://docs.woocommerce.com/document/managing-orders/
        public static Dictionary<string, string> Statuses { get; } = new Dictionary<string, string>() {
                { "pending", "Čeká na platbu" }, // Order received, no payment initiated.Awaiting payment (unpaid).
                { "failed", "Selhalo" }, // Payment failed or was declined (unpaid). Note that this status may not show immediately and instead show as Pending until verified (e.g., PayPal).
                { "processing", "Zpracovává se" }, // Payment received (paid) and stock has been reduced; order is awaiting fulfillment.All product orders require processing, except those that only contain products which are both Virtual and Downloadable.
                { "completed", "Dokončeno" }, // Order fulfilled and complete – requires no further action.
                { "on-hold", "Čeká na vyřízení"}, // Awaiting payment – stock is reduced, but you need to confirm payment.
                { "cancelled", "Zrušena" }, // Cancelled by an admin or the customer – stock is increased, no further action required.
                { "refunded", "Vráceno"  } // Refunded by an admin – no further action required.
            };

        private static Lazy<OrdersPage> lazy = new Lazy<OrdersPage>(() => new OrdersPage());

        public static OrdersPage Instance { get { return lazy.Value; } }

        private OrdersPage()
        {
            this.DataContext = MyWindow.Appka;

            InitializeComponent();
        }

        public class Order : INotifyPropertyChanged
        {
            private JToken store = null;

            public event PropertyChangedEventHandler PropertyChanged;

            public long Id { get { return this.GetValue<long>("id"); } }

            public OrderStatus Status
            {
                get
                {
                    return new OrderStatus(this.GetValue<string>("status"));
                }
                set
                {
                    this.SetValue("status", value.Id);
                }
            }

            public string PaymentMethod { get { return this.GetValue<string>("payment_method_title"); } }

            public object Total { get { return this.GetValue("total"); } }

            public DateTime CreatedOn { get { return this.GetValue<DateTime>("date_created_gmt").ToLocalTime(); } }

            protected JArray MetaData { get { return this.store["meta_data"] as JArray; } }

            public string InvoiceNr
            {
                get
                {
                    //var jval = this.MetaData["1548"] as JValue;

                    var jVal = this.MetaData.FirstOrDefault(f =>
                    {
                        var val = f as JToken;

                        if ((val.Value<string>("key")) == "_wcpdf_invoice_number")
                            return true;

                        return false;
                    });

                    if (jVal != null)
                        return (jVal as JToken).Value<string>("value");

                    return null;
                }
            }

            public Order(JToken data)
            {
                store = data;
            }

            public void UpdateStore(JToken data)
            {
                store = data;
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
            }

            protected object GetValue(string name)
            {
                return (this.store[name] as JValue).Value;
            }

            protected T GetValue<T>(string name)
            {
                return ((T)this.GetValue(name));
            }

            protected void SetValue(string name, object value)
            {
                (this.store[name] as JValue).Value = value;
            }

            public class OrderStatus
            {
                public string Id { get; set; }

                public string DisplayText
                {
                    get
                    {
                        return Statuses[this.Id];
                    }
                }

                public OrderStatus(string id)
                {
                    this.Id = id;
                }

                public override string ToString()
                {
                    return this.DisplayText;
                }
            }
        }

        private async void ContextMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;

            var contextMenu = (ContextMenu)sender;

            if (contextMenu.PlacementTarget is DataGridRow row && row.DataContext is Order order && menuItem.DataContext is KeyValuePair<string, string> pair)
            {
                var config = Files.Load<Configuration>(Files.Paths.ConfigXml);

                string username = config.WordpressUsername;
                string password = config.WordpressPassword;
                string url = $"{config.WordpressUrl}/index.php/wp-json/wc/v2/orders/{order.Id}";

                string payload = JsonConvert.SerializeObject(new { status = pair.Key });

                await WordPress.Put(username, password, url, payload);
            }
        }
    }
}