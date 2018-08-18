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

        public List<Panel> RegisteredPages { get; } = new List<Panel>() {
            new pages.ConfigurationPage(),
            new pages.TestPage()
        };

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(Panel), typeof(MyWindow));

        public Panel CurrentPage
        {
            get { return (Panel)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        public IEnumerable<dynamic> Pages { get { return RegisteredPages.Select(f => new { f.Name }); } }

        private MyWindow()
        {
            this.CurrentPage = RegisteredPages[0];

            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            var cnt = btn?.Content as string;

            var page = RegisteredPages.FirstOrDefault(f => f.Name == cnt);

            this.CurrentPage = page;
        }
    }
}