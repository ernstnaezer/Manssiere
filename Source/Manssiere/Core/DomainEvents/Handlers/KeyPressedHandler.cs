namespace Manssiere.Core.DomainEvents.Handlers
{
    using System.Linq;
    using System.Windows;
    using Components;
    using Events;
    using Helpers;
    using OpenTK.Input;

    public class KeyPressedHandler 
        : IHandles<KeyPressedEvent>
    {
        /// <summary>
        ///   Handles the specified @event.
        /// </summary>
        /// <param name = "event">The @event.</param>
        public void Handle(KeyPressedEvent @event)
        {
            if (@event.PressedKeys.Contains(Key.Escape))
            {
                Application.Current.Shutdown();
            }

            if (@event.PressedKeys.Contains(Key.P))
            {
                ScreenshotHelper.SaveScreenshot();
            }

            if (@event.PressedKeys.Contains(Key.Back) || @event.PressedKeys.Contains(Key.Left))
            {
                PresenterWindow.DemoFlow.PreviousScene();
            }

            if (@event.PressedKeys.Contains(Key.Space) || @event.PressedKeys.Contains(Key.Right))
            {
                PresenterWindow.DemoFlow.NextScene();
            }
        }
    }
}