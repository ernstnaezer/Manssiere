namespace Manssiere.Core
{
    using System;

    /// <summary>
    /// Generic disposable action
    /// </summary>
    /// <typeparam name="T">Action type.</typeparam>
    public class DisposableAction<T> : IDisposable
    {
        private readonly Action<T> _action;
        private readonly T _val;

        /// <summary>
        /// Create a new disposable action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="val">The tag value.</param>
        public DisposableAction(Action<T> action, T val)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
            _val = val;
        }

        /// <summary>
        /// The tag.
        /// </summary>
        public T Value
        {
            get { return _val; }
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose action.
        /// </summary>
        public void Dispose()
        {
            _action(_val);
        }

        #endregion
    }

    /// <summary>
    /// Disposable action.
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private readonly Action _action;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="action">The action to execute on dispose.</param>
        public DisposableAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _action = action;
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose action.
        /// </summary>
        public void Dispose()
        {
            _action();
        }

        #endregion
    }
}