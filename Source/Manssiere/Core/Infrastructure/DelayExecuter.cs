namespace Manssiere.Core.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Timers;
    using System.Windows.Threading;

    /// <summary>
    ///   Provides functionality for delaying the execution of a <see cref = "Delegate" />.
    /// </summary>
    /// <remarks>
    ///   This class will execute the <see cref = "Delegate" />s in the Thread associated with its <see cref = "Dispatcher" />.
    /// </remarks>
    public sealed class DelayExecuter
    {
        private readonly Dispatcher _dispatcher;
        private readonly SortedList<int, Job> _jobs;
        private readonly object _resourceLock;
        private readonly Timer _timer;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DelayExecuter" /> class.
        /// </summary>
        /// <remarks>
        ///   <see cref = "Dispatcher" /> is set to <see cref = "System.Windows.Threading.Dispatcher.CurrentDispatcher" />
        /// </remarks>
        public DelayExecuter()
            : this(Dispatcher.CurrentDispatcher)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DelayExecuter" /> class.
        /// </summary>
        /// <param name = "dispatcher">The dispatcher to use as <see cref = "Dispatcher" />.</param>
        public DelayExecuter(Dispatcher dispatcher)
        {
            _jobs = new SortedList<int, Job>();
            _resourceLock = new object();

            _dispatcher = dispatcher;

            _timer = new Timer {AutoReset = false};
            _timer.Elapsed += TimerElapsed;
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "DelayExecuter" /> has pending jobs.
        /// </summary>
        /// <value><c>true</c> if this instance has pending jobs; otherwise, <c>false</c>.</value>
        public bool HasJobs
        {
            get
            {
                lock (_resourceLock)
                {
                    return _jobs.Count != 0;
                }
            }
        }

        /// <summary>
        ///   Gets the dispatcher for this <see cref = "DelayExecuter" />.
        /// </summary>
        /// <value>The dispatcher.</value>
        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        ///   Executes the specified <see cref = "Delegate" /> after the specified delay.
        /// </summary>
        /// <param name = "delay">The delay to wait</param>
        /// <param name = "callback">The callback to execute.</param>
        /// <param name = "args">The optional args for the <paramref name = "callback" />.</param>
        public void Execute(TimeSpan delay, Delegate callback, params object[] args)
        {
            Execute(delay, DispatcherPriority.Normal, callback, args);
        }

        /// <summary>
        ///   Executes the specified <see cref = "Delegate" /> after the specified delay.
        /// </summary>
        /// <param name = "delay">The delay to wait</param>
        /// <param name = "priority">The <see cref = "DispatcherPriority" /> to use to invoke the <paramref name = "callback" />.</param>
        /// <param name = "callback">The callback to execute.</param>
        /// <param name = "args">The optional args for the <paramref name = "callback" />.</param>
        public void Execute(TimeSpan delay, DispatcherPriority priority, Delegate callback, params object[] args)
        {
            var job = new Job(callback, args, priority);

            lock (_resourceLock)
            {
                _timer.Stop();

                var executionTime = Environment.TickCount;
                unchecked
                {
                    executionTime += (int) delay.TotalMilliseconds;
                }

                _jobs.Add(executionTime, job);

                RefreshTimer();
            }
        }

        /// <summary>
        ///   Executes the specified <see cref = "Delegate" /> after the specified time.
        /// </summary>
        /// <param name = "executionTime">The time when to execute the <paramref name = "callback" />.</param>
        /// <param name = "callback">The callback to execute.</param>
        /// <param name = "args">The optional args for the <paramref name = "callback" />.</param>
        public void Execute(DateTime executionTime, Delegate callback, params object[] args)
        {
            Execute(executionTime, DispatcherPriority.Normal, callback, args);
        }

        /// <summary>
        ///   Executes the specified <see cref = "Delegate" /> after the specified time.
        /// </summary>
        /// <param name = "executionTime">The time when to execute the <paramref name = "callback" />.</param>
        /// <param name = "priority">The <see cref = "DispatcherPriority" /> to use to invoke the <paramref name = "callback" />.</param>
        /// <param name = "callback">The callback to execute.</param>
        /// <param name = "args">The optional args for the <paramref name = "callback" />.</param>
        public void Execute(DateTime executionTime, DispatcherPriority priority, Delegate callback, params object[] args)
        {
            Execute(executionTime.Subtract(DateTime.Now), priority, callback, args);
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            lock (_resourceLock)
            {
                //in ms - cycels between int.MinValue and int.MaxValue
                var tickCount = Environment.TickCount;
                var right = tickCount;
                var left = unchecked(tickCount - int.MaxValue);

                var min = Math.Min(right, left);
                var max = Math.Max(right, left);

                while (_jobs.Count != 0
                       && max >= _jobs.Keys[0]
                       && min <= _jobs.Keys[0])
                {
                    Job job = _jobs.Values[0];
                    Dispatcher.BeginInvoke(job.Callback, job.Priority, job.Args);
                    _jobs.RemoveAt(0);
                }

                RefreshTimer();
            }
        }

        private void RefreshTimer()
        {
            if (_jobs.Count == 0)
                return;

            var tickCount = Environment.TickCount;
            var timeToNext = Math.Abs(unchecked(_jobs.Keys[0] - tickCount));

            if (timeToNext == 0)
                timeToNext = 1;

            _timer.Interval = timeToNext;
            _timer.Start();
        }
    }
}