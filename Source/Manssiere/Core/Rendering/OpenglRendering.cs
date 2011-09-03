namespace Manssiere.Core.Rendering
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Components;
    using DomainEvents.Events;
    using Graphics;
    using Graphics.Effects;
    using Graphics.Xaml;
    using Helpers;
    using Microsoft.Practices.ServiceLocation;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Input;
    using Configuration = Core.Configuration;
    using Size = System.Drawing.Size;

    public class OpenglRendering : GameWindow
    {
        private readonly XamlRenderer _xamlRenderer = new XamlRenderer();
        private PresenterWindow _presenterWindow;

        private bool _holdDispatcherEvent;
        private Framebuffer _framebuffer;

        public OpenglRendering()
            : base(
              Configuration.DisplayResolution.Width,
              Configuration.DisplayResolution.Height,
              new GraphicsMode(new ColorFormat(32), 0, 0, 0),
              Configuration.WindowTitle//,GameWindowFlags.Fullscreen
            )
        {
            KeyPress += OpenglRendering_KeyPress;
        }

        void OpenglRendering_KeyPress(object sender, KeyPressEventArgs e)
        {
            var pressedKeys = from key in Enum.GetValues(typeof (Key)).Cast<Key>()
                              where Keyboard[key]
                              select key;

            DomainEvents.DomainEvents.Raise(new KeyPressedEvent(pressedKeys));
        }

        #region Move & resize code

        protected override void OnMove(EventArgs e)
        {
            _holdDispatcherEvent = true;
            base.OnMove(e);
            _holdDispatcherEvent = false;
        }

        protected override void OnResize(EventArgs e)
        {
            _holdDispatcherEvent = true;
            base.OnResize(e);

            if (_presenterWindow != null)
            {
                Configuration.DisplayResolution = new Size(Width, Height);
                _presenterWindow.SetupFinalRenderTransform();
            }

            _holdDispatcherEvent = false;
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // configurate opengl
            var openglInit = new InitializeOpenGl();
            openglInit.Execute();

            // preform a load and init of all the scene's and components
            //ServiceLocator.Current.GetAllInstances<FrameworkElement>().ToArray();
            
            _presenterWindow = ServiceLocator.Current.GetInstance<PresenterWindow>();

            var refreshRate = DisplayDevice.Default.RefreshRate;

            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = Convert.ToInt32(refreshRate) }
                );

            _framebuffer = new Framebuffer(Configuration.InternalResolution.Width,
                                           Configuration.InternalResolution.Height) {Name = "Framebuffer"};

            // by default the presenter window stands in WPF rendering mode.
            // this meens there is a layout root (canvas) that has the aspect correction
            // render transform applied to it and contains a transition presenter.
            // The WPF engine takes care of framebuffers and all the interal stuff.
            // In the opengl rendering we need to manage this.

            // first we clean the transition presenter from the rendering queue
            // The transition presenter is passed as a seperate instance toafter we've pushed
            // the rendering framebuffer.
            _presenterWindow.LayoutRoot.Children.Clear();

            // we replace the transition presenter with the framebuffer object
            _presenterWindow.LayoutRoot.Children.Add(_framebuffer);

       }

        /// <summary>
        ///   Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name = "e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (PresenterWindow.DemoFlow.CurrentScene == null) Exit();

            if (_holdDispatcherEvent) return;

            // update total demo time
            Configuration.RunTime += e.Time;

            // trigger the animations
            DispatcherHelper.DoEvents();
        }

        /// <summary>
        ///   Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name = "e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (PresenterWindow.DemoFlow.CurrentScene == null) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Viewport(0, 0, Configuration.InternalResolution.Width, Configuration.InternalResolution.Height);
          //  GL.Viewport(0, 0, Configuration.DisplayResolution.Width, Configuration.DisplayResolution.Height);
            using (_framebuffer.PushFramebuffer())
            {
                _xamlRenderer.Draw(_presenterWindow.TransitionPresenter);
            }

            _xamlRenderer.Draw(_presenterWindow);

            SwapBuffers();
        }
    }
}