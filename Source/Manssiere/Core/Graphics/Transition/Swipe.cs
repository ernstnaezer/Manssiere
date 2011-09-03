namespace Manssiere.Core.Graphics.Transition
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    public class Swipe : ITransition
    {
        private readonly Duration _swipLength = new Duration(TimeSpan.FromSeconds(1));

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
            var sb = new Storyboard
                         {
                             Duration = _swipLength
                         };

            Canvas.SetLeft(newContent, Configuration.InternalResolution.Width);

            var animation = CreateSwipAnimation(newContent);
            sb.Children.Add(animation);

            sb.Completed +=
                (s, e) =>
                    {
                        sb.Stop();
                        transitionElement.TransitionEnded(oldContent);
                        Canvas.SetLeft(newContent, 0);
                    };

            sb.Begin();
        }

        #endregion

        /// <summary>
        /// Creates the swip animation.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private AnimationTimeline CreateSwipAnimation(DependencyObject target)
        {           
            var animation = new DoubleAnimation
                                {
                                    To = 0, 
                                    Duration = _swipLength
                                };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.LeftProperty));

            return animation;
        }
    }
}