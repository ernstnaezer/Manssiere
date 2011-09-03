namespace Manssiere.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Core.Animation;

    /// <summary>
    ///   Interaction logic for ScotchYokeUserControl.xaml
    /// </summary>
    public partial class ScotchYokeUserControl
    {
        private readonly Storyboard _storyBoard = new Storyboard();

        public ScotchYokeUserControl()
        {
            InitializeComponent();

            InitializeRotations();

            GotFocus += (e, s) => _storyBoard.Begin();
        }

        private void InitializeRotations()
        {
            _storyBoard.RepeatBehavior = RepeatBehavior.Forever;
            var duration = TimeSpan.FromSeconds(10);

            var animationRotation = new DoubleAnimationUsingKeyFrames
                                        {
                                            Duration = new Duration(duration),
                                            RepeatBehavior = RepeatBehavior.Forever
                                        };

            var animationTranslate = new PennerDoubleAnimation
                                         {
                                             Equation = PennerDoubleAnimation.Equations.QuadEaseInOut,
                                             Duration = TimeSpan.FromSeconds(5),
                                             RepeatBehavior = RepeatBehavior.Forever,
                                             From = 0,
                                             To = 610,
                                             AutoReverse = true
                                         };

            animationRotation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromPercent(0)));
            animationRotation.KeyFrames.Add(new LinearDoubleKeyFrame(360, KeyTime.FromPercent(1)));
           
            var rotateTransform = new RotateTransform();
            PullPoint.RenderTransform = rotateTransform;
            PullPoint.RenderTransformOrigin = new Point(0.5, 0.5);

            var translateTransform = new TranslateTransform();
            Yoke.RenderTransform = translateTransform;
            Yoke.RenderTransformOrigin = new Point(0.5,0.5);

            Storyboard.SetTarget(animationRotation, rotateTransform);
            Storyboard.SetTargetProperty(animationRotation, new PropertyPath(RotateTransform.AngleProperty));

            Storyboard.SetTarget(animationTranslate, translateTransform);
            Storyboard.SetTargetProperty(animationTranslate, new PropertyPath(TranslateTransform.XProperty));

            animationTranslate.Freeze();
            animationRotation.Freeze();

            _storyBoard.Children.Add(animationTranslate);
            _storyBoard.Children.Add(animationRotation);
        }
    }
}