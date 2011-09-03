namespace Manssiere.Core.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Xaml switch to fullscreen helper
    /// </summary>
    /// <remarks> http://www.swart.ws/2009/03/kiosk-full-screen-wpf-applications.html </remarks>
    public static class FullScreenHelper
    {
        /// <summary>
        /// Switch the window to fullscreen
        /// </summary>
        /// <param name="window"></param>
        public static void GoFullscreen(this Window window)
        {
            // Make window borderless
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;

            // Get handle for nearest monitor to this window
            var wih = new WindowInteropHelper(window);
            var hMonitor = UnsafeNativeMethods.MonitorFromWindow(wih.Handle, MonitorDefaulttonearest);

            // Get monitor info
            var monitorInfo = new MonitorHelper.Monitorinfoex();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            UnsafeNativeMethods.GetMonitorInfo(new HandleRef(window, hMonitor), monitorInfo);

            // Create working area dimensions, converted to DPI-independent values
            var source = HwndSource.FromHwnd(wih.Handle);
            if (source == null) return; // Should never be null
            if (source.CompositionTarget == null) return; // Should never be null
            var matrix = source.CompositionTarget.TransformFromDevice;
            var workingArea = monitorInfo.rcMonitor;
            var dpiIndependentSize =
                matrix.Transform(
                new Point(
                    workingArea.Right - workingArea.Left,
                    workingArea.Bottom - workingArea.Top));

            // Maximize the window to the device-independent working area ie
            // the area without the taskbar.
            // NOTE - window state must be set to Maximized as this adds certain
            // maximized behaviors eg you can't move a window while it is maximized,
            // such as by calling Window.DragMove
            window.MaxWidth = dpiIndependentSize.X;
            window.MaxHeight = dpiIndependentSize.Y;
            window.WindowState = WindowState.Maximized;
        }

        // Nearest monitor to window
        const int MonitorDefaulttonearest = 2;
    }
}