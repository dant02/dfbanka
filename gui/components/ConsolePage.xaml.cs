using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using dfbanka.gui.core;

namespace dfbanka.gui.components
{
    public partial class ConsolePage : Border, ILog
    {
        private static Lazy<ConsolePage> lazy = new Lazy<ConsolePage>(() => new ConsolePage());

        public static ConsolePage Instance { get { return lazy.Value; } }

        private Paragraph paragraph = new Paragraph() { FontFamily = new FontFamily("Consolas"), FontSize = 11 };

        private ScrollViewer scrollViewer;

        private ConsolePage()
        {
            InitializeComponent();

            txt.Document = new FlowDocument(paragraph);

            this.Print("started");
        }

        public ScrollViewer ScrollViewer
        {
            get
            {
                if (this.scrollViewer == null)
                {
                    DependencyObject obj = this;

                    do
                    {
                        if (VisualTreeHelper.GetChildrenCount(obj) > 0)
                            obj = VisualTreeHelper.GetChild(obj as Visual, 0);
                        else
                            return null;
                    }
                    while (!(obj is ScrollViewer));

                    this.scrollViewer = obj as ScrollViewer;
                }

                return this.scrollViewer;
            }
        }

        private bool hasText = false;

        public void Clear()
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                this.paragraph.Inlines.Clear();
                hasText = false;
            });
        }

        public void Print(string text)
        {
            App.Current.Dispatcher.InvokeAsync(() =>
            {
                if (hasText)
                    paragraph.Inlines.Add(new LineBreak());

                paragraph.Inlines.Add(new Run(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")) { Background = Brushes.LightGreen });
                paragraph.Inlines.Add(" " + text);

                hasText = true;

                this.ScrollViewer?.ScrollToEnd();
            });
        }
    }
}