namespace Manssiere.Core.Infrastructure
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Castle.Core;
    using Manssiere.Core.DomainEvents.Events;

    /// <summary>
    /// Strong type xaml loader and initializer
    /// </summary>
    public static class XamlLoader
    {
        private static readonly RoutedEventArgs LoadedEventArgs = new RoutedEventArgs(FrameworkElement.LoadedEvent);

        /// <summary>
        /// Initialized an element by calling the Arrange function and raising the load event.
        /// </summary>
        /// <param name="element"></param>
        public static void InitializeFrameworkElement(FrameworkElement element)
        {
            ArrangeCanvasElements(element);
            RaiseLoadedEvent(element);
            RaiseLoadedDomainEvent(element);           
        }

        /// <summary>
        /// Calls the Arrange function on child canvas elements
        /// </summary>
        /// <param name="xamlObject">The xaml object.</param>
        private static void ArrangeCanvasElements(FrameworkElement xamlObject)
        {
            if (xamlObject is Canvas)
                xamlObject.Arrange(new Rect());

            LogicalTreeHelper
                .GetChildren(xamlObject)
                .Cast<FrameworkElement>()
                .ForEach(ArrangeCanvasElements);
        }

        /// <summary>
        /// Raises the loaded event.
        /// </summary>
        /// <param name="xamlObject">The xaml object.</param>
        private static void RaiseLoadedEvent(FrameworkElement xamlObject)
        {
            xamlObject.RaiseEvent(LoadedEventArgs);

            LogicalTreeHelper
                .GetChildren(xamlObject)
                .Cast<FrameworkElement>()
                .ForEach(RaiseLoadedEvent);
        }        

        /// <summary>
        /// Raises the xaml objecgt loaded domainevent.
        /// </summary>
        /// <param name="xamlObject">The xaml object.</param>
        private static void RaiseLoadedDomainEvent(FrameworkElement xamlObject) {
            DomainEvents.DomainEvents.Raise(new XamlObjectLoadedEvent(xamlObject));

            LogicalTreeHelper
                .GetChildren(xamlObject)
                .Cast<FrameworkElement>()
                .ForEach(RaiseLoadedDomainEvent);
        }
    }
}