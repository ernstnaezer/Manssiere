namespace Manssiere.Core.Graphics.Xaml
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using Castle.Core;
    using Effects;

    /// <summary>
    ///   Opengl Xaml Rendering engine. This is where most of the magic happens.
    /// </summary>
    public class XamlRenderer : IXamlRenderer<FrameworkElement>
    {
        private readonly CanvasRenderer _canvasRenderer = new CanvasRenderer();
        private readonly ImageRenderer _imageRenderer = new ImageRenderer();
        private readonly PathRenderer _pathRenderer = new PathRenderer();
        private readonly Viewport3DRenderer _viewport3DRenderer = new Viewport3DRenderer();
        private readonly ControlRenderer _controlRenderer = new ControlRenderer();
        private readonly FramebufferRenderer _framebufferRenderer = new FramebufferRenderer();

        /// <summary>
        ///   Draws the specified element.
        /// </summary>
        /// <param name = "element">The element.</param>
        public void Draw(FrameworkElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (element is Window == false && element.Visibility != Visibility.Visible) return;

            // update the internal WPF state
            element.Arrange(new Rect(0,0,Configuration.InternalResolution.Width, Configuration.InternalResolution.Height));

            // excecute any pre render action (Effects)
            PreRenderAction(element);

            if (element is Window)
            {
                var window = (Window) element;
                _controlRenderer.Draw(window);
                Draw((FrameworkElement) window.Content);
            }
            else if (element is Canvas)
            {
                var canvas = (Canvas) element;
                _canvasRenderer.Draw(canvas);
                canvas.Children.Cast<FrameworkElement>().ForEach(Draw);
            }
            else if (element is Framebuffer)
            {
                _framebufferRenderer.Draw((Framebuffer) element);
            }
            else if (element is Shape)
            {
                _pathRenderer.Draw((Shape)element);
            }
            else if (element is Image)
            {
                _imageRenderer.Draw((Image) element);
            }
            else if (element is Viewport3D)
            {
                _viewport3DRenderer.Draw((Viewport3D) element);
            }
            else if (element is UserControl)
            {
                _controlRenderer.Draw((UserControl) element);
                Draw((FrameworkElement) ((UserControl) element).Content);
            }

            // cleanup & merge effects
            PostRenderAction(element);
        }

        /// <summary>
        ///   Executes actions needed before an rendering takes place
        /// </summary>
        /// <param name = "element"></param>
        private static void PreRenderAction(UIElement element)
        {
            if (!(element.Effect is IEffect)) return;
            var effect = (IEffect) element.Effect;

            if (!effect.IsInitialized)
                effect.Initialize();

            effect.PreRenderAction(element);
        }

        /// <summary>
        ///   Executes actions needed to process and effect
        /// </summary>
        /// <param name = "element"></param>
        private static void PostRenderAction(UIElement element)
        {
            if (!(element.Effect is IEffect)) return;
            var effect = (IEffect) element.Effect;
            effect.PostRenderAction(element);
        }
    }
}