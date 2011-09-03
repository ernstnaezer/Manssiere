namespace Manssiere.Core.Graphics.Transition
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class FadeIn : ITransition
    {
        private readonly Duration _fadeLength = new Duration(TimeSpan.FromSeconds(2));

        #region ITransition Members

        /// <summary>
        /// Gets a value indicating whether the new content is required topmost.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the new content is required topmost; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresNewContentTopmost
        {
            get { return true; }
        }

        /// <summary>
        /// Begins the transition.
        /// </summary>
        /// <param name="transitionElement">The transition element.</param>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        public void BeginTransition(TransitionPresenter transitionElement, UIElement oldContent, UIElement newContent)
        {
            transitionElement.Children.Clear();
            transitionElement.Children.Add(newContent);

            var sb = new Storyboard();
            var animation = CreateFadeInAnimation(newContent);

            sb.Children.Add(animation);
            sb.Duration = _fadeLength;

            newContent.Opacity = 0;

            sb.Completed +=
                (s, e) =>
                    {
                        sb.Stop();
                        newContent.Opacity = 1;
                    };

            sb.Begin();
        }

        #endregion

        /// <summary>
        /// Creates the fade in animation.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private DoubleAnimation CreateFadeInAnimation(DependencyObject target)
        {
            var animation = new DoubleAnimation();

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));

            animation.To = 1.0;
            animation.Duration = _fadeLength;

            return animation;
        }
    }
}