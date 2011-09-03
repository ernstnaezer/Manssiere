namespace Manssiere.Core.Graphics.Xaml
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Helpers;
    using OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Render a UI element.
    /// </summary>
    /// <typeparam name="T">The uielement type.</typeparam>
    public abstract class ShapeRenderer<T> : DependencyObject, IXamlRenderer<T>
        where T : FrameworkElement
    {
        /// <summary>
        /// Calculate the final shape opacity.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>The final opacity.</returns>
        private static Single GetShapeOpacity(T shape)
        {
            var opacity = (float)shape.Opacity;

            // travel up the group tree and apply the parent opacity.
            var parent = shape.Parent;
            while (parent != null)
            {
                opacity *= Convert.ToSingle(parent.GetValue(UIElement.OpacityProperty));
                parent = LogicalTreeHelper.GetParent(parent);
            }

            return opacity;
        }

        /// <summary>
        /// Apply an opengl color with alpha opacity.
        /// </summary>
        /// <param name="brush">The solid color brush to load.</param>
        /// <param name="opacity">The shape opacity.</param>
        protected static void SetBrush(Brush brush, Single opacity)
        {
            if (brush == null) throw new ArgumentNullException("brush");

            if (!(brush is SolidColorBrush))
            {
                GL.Color3(System.Drawing.Color.Transparent);
                return;
            }

            GL.Color4(GlHelper.GetLightColorValues((Color)brush.GetValue(SolidColorBrush.ColorProperty), opacity));
        }

        /// <summary>
        /// Draw the given shape.
        /// </summary>
        /// <param name="shape">The shape to draw.</param>
        public void Draw(T shape)
        {
            if (shape == null) throw new ArgumentNullException("shape");
            
            // by default our engine disables depth test
            // depth is controlled by the ordering in the XAML file
            // Rendering functions that need a depth test (Viewport3d) 
            // need to enabled them.
            GL.Disable(EnableCap.DepthTest);

            using (GlHelper.PushOrthoMatrix())
            {
                GL.PushMatrix();
                {
                    var transformationMatrix = XamlHelper.GetTransformationMatrix(shape);
                    var glMatrix = GlHelper.ConvertToOpenGlMatrix(transformationMatrix);
                    GL.MultMatrix(ref glMatrix);

                    var shapeOpacity = GetShapeOpacity(shape);

                   DrawShape(shape, shapeOpacity);
                }
                GL.PopMatrix();
            }
        }

        /// <summary>
        /// Shape specific render code.
        /// </summary>
        /// <param name="shape">The shape to render.</param>
        /// <param name="shapeOpacity">The shape opacity.</param>
        protected abstract void DrawShape(T shape, Single shapeOpacity);
    }
}
