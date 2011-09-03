namespace Manssiere.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Point = System.Windows.Point;
    using Size = System.Windows.Size;

    public static class XamlHelper
    {
        private static readonly RoutedEventArgs GotFocusEventArgs = new RoutedEventArgs(UIElement.GotFocusEvent);

        /// <summary>
        /// Raises the got fouces event on the given element.
        /// </summary>
        /// <param name="element"></param>
        public static void RaiseGotFocusEvent(UIElement element)
        {
            element.RaiseEvent(GotFocusEventArgs);        
        }

        /// <summary>
        /// Get the shape final transformation matrix with all the parent transformations applied to it.
        /// </summary>
        /// <param name="element">The shape to get the transformation matrix for.</param>
        /// <returns>The shape transformation matrix.</returns>
        public static Matrix GetTransformationMatrix(FrameworkElement element)
        {
            var localTransform = Matrix.Identity;

            while (element != null)
            {
                var renderTransform = element.RenderTransform.Value;

                var renderTransformOrigin = new Point(element.RenderSize.Width * element.RenderTransformOrigin.X,
                                                      element.RenderSize.Height * element.RenderTransformOrigin.Y);

                var pleft = Canvas.GetLeft(element);
                var ptop = Canvas.GetTop(element);
             
                if (MathHelper.IsValid(pleft) == false)
                {
                    pleft = 0;
                }
                if (MathHelper.IsValid(ptop) == false)
                {
                    ptop = 0;
                }

                localTransform.Translate(-renderTransformOrigin.X, -renderTransformOrigin.Y);

                localTransform *= renderTransform;
                localTransform *= new TranslateTransform(pleft, ptop).Value;

                localTransform.Translate(renderTransformOrigin.X, renderTransformOrigin.Y);

                element = (FrameworkElement)LogicalTreeHelper.GetParent(element);
            }
            return localTransform;
        }

        /// <summary>
        /// Gets the childeren.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IEnumerable<FrameworkElement> GetChilderen(FrameworkElement element)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (var i = 0; i < childCount; i++)
            {
                yield return (FrameworkElement)VisualTreeHelper.GetChild(element, i);
            }
        }

        /// <summary>
        /// Gets the specified transform from a transform group.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transformGroup"></param>
        /// <returns></returns>
        public static T GetTransform<T>(Transform transformGroup)
            where T : Transform
        {
            var @group = (TransformGroup) transformGroup;
            return @group.Children.OfType<T>().First();
        }

        /// <summary>
        /// Creates a transformgroup containing a 
        /// <see cref="ScaleTransform"/>, 
        /// <see cref="SkewTransform"/>, 
        /// <see cref="RotateTransform"/> and 
        /// <see cref="TranslateTransform"/>
        /// </summary>
        /// <returns></returns>
        public static TransformGroup CreateDefaultTransformGroup()
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform());
            transformGroup.Children.Add(new SkewTransform());
            transformGroup.Children.Add(new RotateTransform());
            transformGroup.Children.Add(new TranslateTransform());

            return transformGroup;
        }

        /// <summary>
        ///   Converts a WPF bitmap to a System.Drawing.Bitmap
        /// </summary>
        /// <param name = "wpfBitmap">BitmapSource to convert</param>
        /// <returns>A GDI Bitmap</returns>
        public static Bitmap GdiBitmapFromWpfBitmap(BitmapSource wpfBitmap)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(wpfBitmap));

            using (var imageStream = new MemoryStream())
            {
                encoder.Save(imageStream);
                return new Bitmap(imageStream);
            }
        }

        /// <summary>
        /// Creates the bitmap from visual.
        /// </summary>
        /// <param name="visualToRender">The visual to render.</param>
        /// <returns></returns>
        public static BitmapSource CreateBitmapFromVisual(UIElement visualToRender)
        {
            return CreateBitmapFromVisual(visualToRender, false);
        }

        /// <summary>
        /// Creates the bitmap from visual.
        /// </summary>
        /// <param name="visualToRender">The visual to render.</param>
        /// <param name="undoTransformation">if set to <c>true</c> [undo transformation].</param>
        /// <returns></returns>
        private static BitmapSource CreateBitmapFromVisual(UIElement visualToRender, bool undoTransformation)
        {
            return visualToRender == null 
                ? null 
                : CreateBitmapFromVisual(visualToRender.RenderSize.Width, visualToRender.RenderSize.Height, visualToRender, undoTransformation);
        }

        /// <summary>
        /// Creates the bitmap from visual.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="visualToRender">The visual to render.</param>
        /// <param name="undoTransformation">if set to <c>true</c> [undo transformation].</param>
        /// <returns></returns>
        private static BitmapSource CreateBitmapFromVisual(double width, double height, Visual visualToRender, bool undoTransformation)
        {
            if (visualToRender == null)
            {
                return null;
            }

            var bitmap = new RenderTargetBitmap((int)Math.Ceiling(width), (int)Math.Ceiling(height),
                                                DeviceHelper.PixelsPerInch(Orientation.Horizontal),
                                                DeviceHelper.PixelsPerInch(Orientation.Vertical),
                                                PixelFormats.Pbgra32);
            if (undoTransformation)
            {
                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    var brush = new VisualBrush(visualToRender);
                    context.DrawRectangle(brush, null, new Rect(new Point(), new Size(width, height)));
                }
                bitmap.Render(visual);
                return bitmap;
            }
            bitmap.Render(visualToRender);
            return bitmap;
        }
    }
}