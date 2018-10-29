using System;
using System.Threading;
using dfbanka.gui.components;

namespace dfbanka.gui.core
{
    internal class Runner : IDisposable
    {
        // [ms]
        private const int Interval = 1000;

        private static Lazy<Runner> lazy = new Lazy<Runner>(() => new Runner());

        public static Runner Instance { get { return lazy.Value; } }

        private Timer timer = null;

        internal Runner()
        {
        }

        public void Start()
        {
            timer = new Timer(this.Callback, null, Interval, Timeout.Infinite);
        }

        private void Callback(object state)
        {
            timer.Change(Interval, Timeout.Infinite);

            ConsolePage.Instance.Print("Timer signal");
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}