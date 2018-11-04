using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace dfbanka.gui.core
{
    internal class Runner : IDisposable
    {
        // [ms]
        private const int Interval = 1000;

        private static Lazy<Runner> lazy = new Lazy<Runner>(() => new Runner());

        public static Runner Instance { get { return lazy.Value; } }

        private Timer timer = null;

        private ILog log = null;

        private List<Job> jobs = new List<Job>();

        private Runner()
        {
        }

        public void Start(ILog log)
        {
            this.log = log;
            timer = new Timer(this.Callback, null, Interval, Timeout.Infinite);

            jobs.Add(new Job(this.log));
        }

        private async void Callback(object state)
        {
            foreach (var job in jobs.Where(f => f.LastRunUtc + f.Interval < DateTime.UtcNow).OrderBy(f => f.LastRunUtc))
            {
                var sw = Stopwatch.StartNew();

                try
                {
                    await job.Run();
                }
                catch (Exception ex)
                {
                    log.Print(ex.Message);
                    log.Print(ex.StackTrace);
                }
                finally
                {
                    job.LastRunUtc = DateTime.UtcNow;
                    this.log.Print($"job work: " + sw.Elapsed);
                }
            }

            timer.Change(Interval, Timeout.Infinite);
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}