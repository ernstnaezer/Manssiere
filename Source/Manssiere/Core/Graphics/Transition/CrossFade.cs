namespace Manssiere.Core.Graphics.Transition
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class CrossFade : ITransition
    {
        protected readonly Duration FadeLength = new Duration(TimeSpan.FromSeconds(2));

        #region ITransition Members

        /// <summary>
        /// Gets a value indicating whether the new content is required topmost.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the new content is required topmost; otherwise, <c>false</c>.
        /// </value>
        public virtual bool RequiresNewContentTopmost
        {
            get { return false; }
        }

        /// <summary>
        /// Begins the transition.
        /// </summary>
        /// <param name="transitionElement">The transition element.</param>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        public virtual void BeginTransition(TransitionPresenter transitionElement, UIElement oldContent, UIElement newContent)
        {
            var sb = new Storyboard();
            var animation = CreateFadeOutAnimation(oldContent);

            sb.Children.Add(animation);
            sb.Duration = FadeLength;

            sb.Completed +=
                (s, e) =>
                    {
                        sb.Stop();
                        transitionElement.TransitionEnded(oldContent);
                    };

            sb.Begin();
        }

        #endregion

        /// <summary>
        /// Creates the fade out animation.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        protected DoubleAnimation CreateFadeOutAnimation(DependencyObject target)
        {
            var animation = new DoubleAnimation();
            
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));

            animation.To = 0.0;
            animation.Duration = FadeLength;

            return animation;
        }
    }
}