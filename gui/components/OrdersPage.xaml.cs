using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Windows.Controls;

namespace dfbanka.gui.components
{
    /// <summary>
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Border
    {
        public OrdersPage()
        {
            this.DataContext = MyWindow.Appka;

            InitializeComponent();
        }

        public class Order
        {
            private readonly JToken store = null;

            public long Id { get { return this.GetValue<long>("id"); } }

            public string Status { get { return this.GetValue<string>("status"); } }

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

            protected object GetValue(string name)
            {
                return (this.store[name] as JValue).Value;
            }

            protected T GetValue<T>(string name)
            {
                return ((T)this.GetValue(name));
            }
        }
    }
}