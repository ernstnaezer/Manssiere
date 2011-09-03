namespace Manssiere.Core.Helpers
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Native win32 unsafe / unmanaged functions
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        // Methods
        public static DcSafeHandle CreateDc(string lpszDriver)
        {
            return IntCreateDC(lpszDriver, null, null, IntPtr.Zero);
        }

        /// <summary>
        /// Sets the device gamma ramp.
        /// </summary>
        /// <param name="hDc">The h dc.</param>
        /// <param name="lpRamp">The lp ramp.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDc, ref MonitorHelper.Ramp lpRamp);

        /// <summary>
        /// Gets the device gamma ramp.
        /// </summary>
        /// <param name="hDc">The h dc.</param>
        /// <param name="lpRamp">The lp ramp.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        public static extern bool GetDeviceGammaRamp(IntPtr hDc, out MonitorHelper.Ramp lpRamp);

        /// <summary>
        /// Gets the device context.
        /// </summary>
        /// <param name="hWnd">The handle.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Deletes the device context.
        /// </summary>
        /// <param name="hDc">The h dc.</param>
        /// <returns></returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool DeleteDC(IntPtr hDc);

        /// <summary>
        /// Gets the device caps.
        /// </summary>
        /// <param name="hDc">The h dc.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDeviceCaps(DcSafeHandle hDc, int nIndex);

        /// <summary>
        /// Creates a new device context.
        /// </summary>
        /// <param name="lpszDriver">The LPSZ driver.</param>
        /// <param name="lpszDeviceName">Name of the LPSZ device.</param>
        /// <param name="lpszOutput">The LPSZ output.</param>
        /// <param name="devMode">The dev mode.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateDC", CharSet = CharSet.Auto)]
        public static extern DcSafeHandle IntCreateDC(string lpszDriver, string lpszDeviceName, string lpszOutput, IntPtr devMode);

        // To get a handle to the specified monitor
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        // To get the working area of the specified monitor
        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MonitorHelper.Monitorinfoex monitorInfo);
    }
}