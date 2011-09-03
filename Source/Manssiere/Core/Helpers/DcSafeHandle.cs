namespace Manssiere.Core.Helpers
{
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// Save device context handle
    /// </summary>
    internal sealed class DcSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DcSafeHandle"/> class.
        /// </summary>
        private DcSafeHandle()
            : base(true)
        {
        }

        /// <summary>
        /// When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            return UnsafeNativeMethods.DeleteDC(base.handle);
        }
    }
}