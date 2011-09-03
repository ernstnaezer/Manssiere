namespace Manssiere.Core
{
    using Graphics;
    using OpenTK.Input;
    using Size = System.Drawing.Size;

    /// <summary>
    ///   Demo config
    /// </summary>
    public static class Configuration
    {
        private static readonly Size Internalsize = new Size(1920, 1080);

        static Configuration()
        {
            DisplayResolution = new Size(800, 600);
            WindowTitle = "some dumbass forgot to set the title";
        }

        /// <summary>
        ///   Gets or sets the keyboard device
        /// </summary>
        public static KeyboardDevice Keyboard { get; set; }

        /// <summary>
        ///   Gets or sets the active framebuffer
        /// </summary>
        public static Framebuffer ActiveFramebuffer { get; set; }

        /// <summary>
        ///   Gets or sets the demo runtime
        /// </summary>
        public static double RunTime { get; set; }

        /// <summary>
        ///   Internal demo size
        /// </summary>
        public static Size InternalResolution
        {
            get { return Internalsize; }
        }

        /// <summary>
        ///   Gets or sets the display resolution.
        /// </summary>
        /// <value>The display resolution.</value>
        public static Size DisplayResolution { get; set; }


        /// <summary>
        /// Gets or set the demo titles 
        /// </summary>
        public static string WindowTitle { get; set; }
    }
}