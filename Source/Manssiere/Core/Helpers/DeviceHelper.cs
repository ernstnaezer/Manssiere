namespace Manssiere.Core.Helpers
{
    using System.Windows.Controls;

    /// <summary>
    /// Device helper functions
    /// </summary>
    internal static class DeviceHelper
    {
        // Methods
        public static int PixelsPerInch(Orientation orientation)
        {
            var nIndex = (orientation == Orientation.Horizontal) ? 0x58 : 90;
            using (var handle = UnsafeNativeMethods.CreateDc("DISPLAY"))
            {
                return (handle.IsInvalid ? 0x60 : UnsafeNativeMethods.GetDeviceCaps(handle, nIndex));
            }
        }
    }
}