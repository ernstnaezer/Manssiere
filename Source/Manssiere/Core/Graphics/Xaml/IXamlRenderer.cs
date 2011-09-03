namespace Manssiere.Core.Graphics.Xaml
{
    using System.Windows;

    public interface IXamlRenderer<T> 
        where T : FrameworkElement
    {
        /// <summary>
        /// Draws the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        void Draw(T element);
    }
}