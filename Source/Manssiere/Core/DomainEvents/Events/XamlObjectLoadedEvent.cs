namespace Manssiere.Core.DomainEvents.Events
{
    using System.Windows;

    /// <summary>
    /// Event raised when an object is loaded by the xaml loader
    /// </summary>
    public class XamlObjectLoadedEvent : IDomainEvent
    {
        public FrameworkElement FrameworkElement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlObjectLoadedEvent"/> class.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        public XamlObjectLoadedEvent(FrameworkElement frameworkElement)
        {
            FrameworkElement = frameworkElement;
        }
    }
}