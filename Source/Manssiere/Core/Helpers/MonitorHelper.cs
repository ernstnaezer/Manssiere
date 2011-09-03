namespace Manssiere.Core.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using Size = System.Drawing.Size;

    /// <summary>
    /// Win32 specific gamma helper functions.
    /// </summary>
    public static class MonitorHelper
    {
        private static Ramp? _savedRamp;

        /// <summary>
        /// Save the current gamma ramp.
        /// </summary>
        /// <remarks>Call this funciton at startup</remarks>
        public static void SaveGamma()
        {
            Ramp savedRamp;
            UnsafeNativeMethods.GetDeviceGammaRamp(UnsafeNativeMethods.GetDC(IntPtr.Zero), out savedRamp);
            _savedRamp = savedRamp;
        }

        /// <summary>
        /// Restores the saved gamma ramp. 
        /// </summary>
        /// <remarks>Call this funciton at closedown</remarks>
        public static void RestoreGamma()
        {
            if(_savedRamp == null) return;

            var savedRamp = _savedRamp.Value;
            UnsafeNativeMethods.SetDeviceGammaRamp(UnsafeNativeMethods.GetDC(IntPtr.Zero), ref savedRamp);
        }

        /// <summary>
        /// Sets the gamma.
        /// </summary>
        /// <param name="gamma">The gamma. Gamma is a value between 3 and 44</param>
        public static void SetGamma(int gamma)
        {
            Ramp ramp;
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];

            for (int i = 1; i < 256; i++)
            {
                var v = (ushort) (Math.Min(65535, Math.Max(0, Math.Pow((i + 1)/256.0, gamma*0.1)*65535 + 0.5)));
                ramp.Red[i] = ramp.Green[i] = ramp.Blue[i] = v;
            }
            UnsafeNativeMethods.SetDeviceGammaRamp(UnsafeNativeMethods.GetDC(IntPtr.Zero), ref ramp);
        }

        /// <summary>
        /// Gets the size of the monitor holding the window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Size MonitorSize(Window window)
        {
            // Get handle for nearest monitor to this window
            var wih = new WindowInteropHelper(window);
            var hMonitor = UnsafeNativeMethods.MonitorFromWindow(wih.Handle, 2);

            // Get monitor info
            var monitorInfo = new Monitorinfoex();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            UnsafeNativeMethods.GetMonitorInfo(new HandleRef(window, hMonitor), monitorInfo);

            return new Size(monitorInfo.rcMonitor.Right, monitorInfo.rcMonitor.Bottom);
        }

        #region Nested type: Ramp

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Ramp
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public UInt16[] Blue;
        }

        #endregion

        // Rectangle (used by MONITORINFOEX)
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Monitor information (used by GetMonitorInfo())
        [StructLayout(LayoutKind.Sequential)]
        public class Monitorinfoex
        {
            public int cbSize;
            public RECT rcMonitor; // Total area
            public RECT rcWork; // Working area
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public char[] szDevice;
        }
    }
}
