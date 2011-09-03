namespace Manssiere.Core.Graphics.Transition
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Transition between effects
    /// </summary>
    public class TransitionPresenter : Canvas
    {
        private static readonly RoutedEventArgs LostFocusEventArgs = new RoutedEventArgs(UIElement.LostFocusEvent);

        /// <summary>
        /// Starts the specified transition.
        /// </summary>
        /// <param name="previousContent"></param>
        /// <param name="newContent"></param>
        /// <param name="transition"></param>
        public void DoTransition(UIElement previousContent, UIElement newContent, ITransition transition)
        {
            Children.Clear();
            Children.Add(previousContent);

            if (transition.RequiresNewContentTopmost)
            {
                if (newContent != null) Children.Add(newContent);
                transition.BeginTransition(this, Children.Cast<UIElement>().First(), Children.Cast<UIElement>().Last());
            }
            else
            {
                if (newContent != null) Children.Insert(0, newContent);
                transition.BeginTransition(this, Children.Cast<UIElement>().Last(), Children.Cast<UIElement>().First());
            }
        }

        /// <summary>
        /// Removes the old content control from the visualizer
        /// </summary>
        /// <param name="oldContent"></param>
        public void TransitionEnded(UIElement oldContent)
        {
            if (oldContent == null) return;

            Children.Remove(oldContent);
            oldContent.RaiseEvent(LostFocusEventArgs);
        }
    }
}