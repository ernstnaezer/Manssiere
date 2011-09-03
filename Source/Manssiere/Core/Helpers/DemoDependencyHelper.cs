namespace Manssiere.Core.Helpers
{
    using System.Windows;
    using Graphics;

    /// <summary>
    /// Provides getter and setter functions for various demo runtime objects.
    /// </summary>
    internal sealed class DemoDependencyHelper : DependencyObject
    {
        private static readonly FrameworkElement EventDispatcher = new FrameworkElement();

        private static readonly
            DependencyProperty TextureProperty
                = DependencyProperty
                    .Register("Texture",
                              typeof (Texture),
                              typeof (DemoDependencyHelper),
                              new FrameworkPropertyMetadata(default(Texture))
                    );

        private static readonly
            DependencyProperty FramebufferProperty
                = DependencyProperty
                    .Register("Framebuffer",
                              typeof (Framebuffer),
                              typeof (DemoDependencyHelper),
                              new FrameworkPropertyMetadata(default(Texture))
                    );

        /// <summary>
        /// Gets the texture from the given object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Texture GetTexture(DependencyObject instance)
        {
            return (Texture) instance.GetValue(TextureProperty);
        }

        /// <summary>
        /// Sets the texture at a given instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        public static void SetTexture(DependencyObject instance, Texture value)
        {
            instance.SetValue(TextureProperty, value);
        }

        /// <summary>
        /// Gets the Framebuffer from the given object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Framebuffer GetFramebuffer(DependencyObject instance)
        {
            return (Framebuffer)instance.GetValue(FramebufferProperty);
        }

        /// <summary>
        /// Sets the Framebuffer at a given instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        public static void SetFramebuffer(DependencyObject instance, Framebuffer value)
        {
            instance.SetValue(FramebufferProperty, value);
        }

    }
}