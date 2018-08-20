using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace dfbanka.gui
{
    public partial class MyWindow : WindowBase
    {
        private static Lazy<MyWindow> lazy = new Lazy<MyWindow>(() => new MyWindow());
        public static MyWindow Instance { get { return lazy.Value; } }

        public static App Appka { get { return App.Current as App; } }

        public List<FrameworkElement> RegisteredPages { get; } = new List<FrameworkElement>();

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(FrameworkElement), typeof(MyWindow));

        public FrameworkElement CurrentPage
        {
            get { return (Panel)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        public IEnumerable<dynamic> Pages { get { return RegisteredPages.Select(f => new { f.Name }); } }

        private MyWindow()
        {
            RegisteredPages.Add(new components.TestPage());
            RegisteredPages.Add(new components.OrdersPage());
            RegisteredPages.Add(new components.ConfigurationPage());

            this.CurrentPage = RegisteredPages[0];

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var cnt = btn?.Content as string;

            var page = RegisteredPages.FirstOrDefault(f => f.Name == cnt);

            this.CurrentPage = page;
        }
    }
}