namespace Manssiere.Core.Components
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using DemoFlow;
    using Graphics.Transition;
    using Helpers;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    ///   Interaction logic for PresenterWindow.xaml
    /// </summary>
    public partial class PresenterWindow : Window
    {
        private readonly Random _shakeRandomizer = new Random();

        /// <summary>
        ///   Gets or sets the demo flow.
        /// </summary>
        /// <value>The demo flow.</value>
        public static AbstractDemoFlow DemoFlow
        {
            get { return ServiceLocator.Current.GetInstance<AbstractDemoFlow>(); }
        }

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref = "PresenterWindow" />
        ///   class.
        /// </summary>
        public PresenterWindow()
        {
            InitializeComponent();

            Title = Configuration.WindowTitle;
            Loaded += PresenterWindowLoaded;
        }

        /// <summary>
        ///   Presenters the window loaded.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The
        ///   <see cref = "System.Windows.RoutedEventArgs" />
        ///   instance containing the event data.</param>
        private void PresenterWindowLoaded(object sender, RoutedEventArgs e)
        {
            SetupFinalRenderTransform();

            if (DemoFlow == null || DemoFlow.HasScenes == false)
                throw new ApplicationException("No scenes defined in the demoflow.");

            UpdateCurrentSceneInTransitionPresenter(DemoFlow.CurrentScene);
            DemoFlow.StateChanged += (cs, ce) => UpdateCurrentSceneInTransitionPresenter(ce.ControlDefinition);
        }

        /// <summary>
        ///   Updates the current scene in transition presenter.
        /// </summary>
        /// <param name = "controlDefinition">The control definition.</param>
        private void UpdateCurrentSceneInTransitionPresenter(AbstractDemoFlow.ControlDefinition controlDefinition)
        {
            if (controlDefinition == null) throw new ArgumentNullException("controlDefinition");

            // exit the demo if there is no scene left
            if (DemoFlow.CurrentScene == null)
            {
                Close();
                return;
            }

            if (controlDefinition.TransitionType == null)
            {
                // replace the current controls if the new state h
                if (controlDefinition.ControlType != null)
                {
                    TransitionPresenter.Children.Clear();
                    var newContent = (UIElement) ServiceLocator.Current.GetInstance(controlDefinition.ControlType);

                    // a control is brought into view, send it a notification 
                    // so it can start its animations
                    XamlHelper.RaiseGotFocusEvent(newContent);

                    TransitionPresenter.Children.Add(newContent);
                }
                return;
            }

            var previousScene = DemoFlow.PreviousState;
            var currentScene = DemoFlow.CurrentScene;

            var previousControlType = previousScene != null ? previousScene.ControlType : null;
            var currentControlType = currentScene.ControlType;

            var previousControl = previousControlType != null
                                      ? (UIElement) ServiceLocator.Current.GetInstance(previousControlType)
                                      : new Canvas();
            var currentControl = currentControlType != null
                                     ? (UIElement) ServiceLocator.Current.GetInstance(currentControlType)
                                     : new Canvas();

            var transition = (ITransition) ServiceLocator.Current.GetInstance(currentScene.TransitionType);

            // a control is brought into view, send it a notification 
            // so it can start its animations
            XamlHelper.RaiseGotFocusEvent(currentControl);

            TransitionPresenter.DoTransition(previousControl, currentControl, transition);
        }

        /// <summary>
        ///   Setups the final render transform.
        /// </summary>
        public void SetupFinalRenderTransform()
        {
            LayoutRoot.Width = Configuration.InternalResolution.Width;
            LayoutRoot.Height = Configuration.InternalResolution.Height;

            LayoutRoot.RenderTransformOrigin = new Point(0, 1);

            var scaleFactor = Convert.ToDouble(Configuration.DisplayResolution.Width)/
                              Convert.ToDouble(Configuration.InternalResolution.Width);

            var scaleTransform = XamlHelper.GetTransform<ScaleTransform>(LayoutRoot.RenderTransform);
            scaleTransform.ScaleX = scaleFactor;
            scaleTransform.ScaleY = scaleFactor;

            var translateTransform = XamlHelper.GetTransform<TranslateTransform>(LayoutRoot.RenderTransform);
            translateTransform.Y =
                - (Configuration.DisplayResolution.Height - (Configuration.InternalResolution.Height*scaleFactor))/2.0;
        }
    }
}