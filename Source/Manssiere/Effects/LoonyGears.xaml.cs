namespace Manssiere.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
    ///   Interaction logic for LoonyGears.xaml
    /// </summary>
    public partial class LoonyGears
    {
        private readonly Storyboard _storyBoard = new Storyboard();

        public LoonyGears()
        {
            InitializeComponent();
            InitializeRotations();

            GotFocus += (e, s) => _storyBoard.Begin();
        }

        private void InitializeRotations()
        {
            const double biggest = 36.0;

            _storyBoard.RepeatBehavior = RepeatBehavior.Forever;

            // SetupAnimationForGear(GearBox33, 1, SweepDirection.Counterclockwise);
            SetupAnimationForGear(GearBox19, 19/biggest, SweepDirection.Counterclockwise);
            SetupAnimationForGear(InternalGearBox, 1, SweepDirection.Clockwise);
            SetupAnimationForGear(GearBox11, 11/biggest, SweepDirection.Clockwise);
            SetupAnimationForGear(GearBox13, 13/biggest, SweepDirection.Counterclockwise);
            SetupAnimationForGear(GearBox7, 7/biggest, SweepDirection.Counterclockwise);
        }

        private void SetupAnimationForGear(Canvas gearBox, double ratio, SweepDirection direction)
        {
            var duration = TimeSpan.FromMilliseconds(30000*ratio);

            var animationRotation = new DoubleAnimationUsingKeyFrames
                                        {
                                            Duration = new Duration(duration),
                                            RepeatBehavior = RepeatBehavior.Forever
                                        };

            animationRotation.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromPercent(0)));
            animationRotation.KeyFrames.Add(new LinearDoubleKeyFrame(
                                                direction == SweepDirection.Clockwise ? 360 : -360,
                                                KeyTime.FromPercent(1)));

            var rotateTransform = new RotateTransform();
            gearBox.RenderTransform = rotateTransform;
            gearBox.RenderTransformOrigin = new Point(0.5, 0.5);

            Storyboard.SetTarget(animationRotation, rotateTransform);
            Storyboard.SetTargetProperty(animationRotation, new PropertyPath(RotateTransform.AngleProperty));

            animationRotation.Freeze();

            _storyBoard.Children.Add(animationRotation);
        }
    }
}