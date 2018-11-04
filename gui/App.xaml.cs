using System.Collections.ObjectModel;
using System.Windows;
using dfbanka.gui.core;
using static dfbanka.gui.components.OrdersPage;

namespace dfbanka.gui
{
    public partial class App : Application
    {
        public App() : base()
        {
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Runner.Instance.Dispose();

            base.OnExit(e);
        }

        public ObservableCollection<Order> Orders { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.Orders = new ObservableCollection<Order>();

            MyWindow.Instance.Show();

            Runner.Instance.Start(MyWindow.Instance.Log);
        }

        public void UpdateOrder()
        {
        }
    }
}