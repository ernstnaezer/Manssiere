namespace Manssiere.Core.Helpers
{
    using System;
    using System.Drawing;
    using Components;
    using Microsoft.Practices.ServiceLocation;
    using OpenTK.Graphics;

    public static class ScreenshotHelper
    {
        /// <summary>
        ///   Saves the screenshot.
        /// </summary>
        public static void SaveScreenshot()
        {
            Bitmap screenshot;
            if (GraphicsContext.CurrentContext == null)
            {
                screenshot = XamlHelper.GdiBitmapFromWpfBitmap(
                    XamlHelper.CreateBitmapFromVisual(
                        ServiceLocator.Current.GetInstance<PresenterWindow>().LayoutRoot));
            }
            else
            {
                screenshot = GlHelper.GrabScreenshot();
            }
            
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            screenshot.Save(string.Format(@"{0}\screenshot - {1}.png", path,
                                          DateTime.Now.ToString("yyyy'-'MM'-'dd HH'.'mm'.'ss")));
        }
    }
}