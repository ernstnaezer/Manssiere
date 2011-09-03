namespace Manssiere.Core.Graphics.Xaml
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Helpers;

    /// <summary>
    /// Render a xaml image in opengl.
    /// </summary>
    public class ImageRenderer : ShapeRenderer<Image>
    {
        /// <summary>
        /// Shape specific render code.
        /// </summary>
        /// <param name="shape">The shape to render.</param>
        /// <param name="shapeOpacity">The shape opacity.</param>
        protected override void DrawShape(Image shape, Single shapeOpacity)
        {
            if (shapeOpacity == 0 || shape.Source == null) return;

            // test both the image and the source for a texture property.
            var texture = DemoDependencyHelper.GetTexture(shape) ??
                          DemoDependencyHelper.GetTexture(shape.Source);

            if (texture == null) return;

            SetBrush(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), shapeOpacity);
            texture.Render((float)shape.RenderSize.Width, (float)shape.RenderSize.Height);
        }
    }
}