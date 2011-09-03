namespace Manssiere.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Core.Helpers;

    /// <summary>
    ///   Interaction logic for StamperScene.xaml
    /// </summary>
    public partial class StamperScene
    {
        public StamperScene()
        {
            InitializeComponent();

           // GotFocus += (s, e) => Crawler.BeginAnimation();
            //LostFocus += (s, e) => Crawler.StopAnimation();

           // GotFocus += (s, e) =>
            {
                CompositionTarget.Rendering += CompositionTargetRendering;
            };

            LostFocus += (s, e) =>
            {
                CompositionTarget.Rendering -= CompositionTargetRendering;
            };

            InitializeStamper(Stamp1);
        }

        void CompositionTargetRendering(object sender, EventArgs e)
        {
            UpdateStamper(Stamp1);
        }

        private static void InitializeStamper(Panel stampertGroup)
        {
            stampertGroup.Children[1].RenderTransform = XamlHelper.CreateDefaultTransformGroup();
            stampertGroup.Children[2].RenderTransform = XamlHelper.CreateDefaultTransformGroup();
        }

        private void UpdateStamper(Panel stampertGroup)
        {
            var crank = stampertGroup.Children[0];
            var rodOne = (FrameworkElement)stampertGroup.Children[1];
            var rodTwo = stampertGroup.Children[2];

            // http://en.wikipedia.org/wiki/Piston_motion_equations
            // x = r cos(a)-sqrt(r^2 cos^2(a)+l^2-r^2)
            var r = Convert.ToSingle(540/2.0);
            var l = Convert.ToSingle(rodOne.Height) - 40;
            var angle = Convert.ToSingle(XamlHelper.GetTransform<RotateTransform>(crank.RenderTransform).Angle) % 360;
            var aRad = OpenTK.MathHelper.DegreesToRadians(angle);

            var rPow = Math.Pow(r, 2);
            var lPow = Math.Pow(l, 2);
            var cosA = Math.Cos(aRad);
            var x = r*cosA - Math.Sqrt((rPow*Math.Pow(cosA, 2)) + lPow - rPow);

            var pistonOffset = Math.Abs(x);
            XamlHelper.GetTransform<TranslateTransform>(rodOne.RenderTransform).Y = pistonOffset - 440;
            XamlHelper.GetTransform<TranslateTransform>(rodTwo.RenderTransform).Y = pistonOffset - 440;

            // http://en.wikipedia.org/wiki/Triangle#Trigonometric_ratios_in_right_triangles
            var rotateTransform = XamlHelper.GetTransform<RotateTransform>(rodOne.RenderTransform);
            rotateTransform.Angle =
                OpenTK.MathHelper.RadiansToDegrees(
                    (float) Math.Acos(((lPow + Math.Pow(pistonOffset, 2) - rPow)/(2*l*pistonOffset))));
            rotateTransform.Angle *= (angle >= 0.0f && angle <= 180.0f) ? 1 : -1;
        }
    }
}