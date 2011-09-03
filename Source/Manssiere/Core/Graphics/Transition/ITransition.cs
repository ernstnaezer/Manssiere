namespace Manssiere.Core.Graphics.Transition
{
    using System.Windows;

    public interface ITransition
    {
        /// <summary>
        /// Gets a value indicating whether the new content is required topmost.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the new content is required topmost; otherwise, <c>false</c>.
        /// </value>
        bool RequiresNewContentTopmost { get; }

        /// <summary>
        /// Begins the transition.
        /// </summary>
        /// <param name="transitionElement">The transition element.</param>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        void BeginTransition(TransitionPresenter transitionElement, UIElement oldContent, UIElement newContent);
    }
}