using System;
using System.Windows;
using System.Windows.Forms;

namespace dfbanka.gui
{
    public class WindowBase : Window
    {
        public WindowBase()
        {
            var ni = new NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name),
                Visible = true
            };
            ni.DoubleClick += (src, arg) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }
    }
}