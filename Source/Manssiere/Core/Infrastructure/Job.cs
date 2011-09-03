namespace Manssiere.Core.Infrastructure
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// Helper class for <see cref="DelayExecuter"/>.
    /// </summary>
    internal sealed class Job
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="args">The args.</param>
        /// <param name="priority">The priority.</param>
        public Job(Delegate callback, object[] args, DispatcherPriority priority)
        {
            Callback = callback;
            Args = args;
            Priority = priority;
        }

        /// <summary>
        /// Gets or sets the callback.
        /// </summary>
        /// <value>The callback.</value>
        public Delegate Callback { get; private set; }

        /// <summary>
        /// Gets or sets the args.
        /// </summary>
        /// <value>The args.</value>
        public object[] Args { get; private set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public DispatcherPriority Priority { get; private set; }
    }
}