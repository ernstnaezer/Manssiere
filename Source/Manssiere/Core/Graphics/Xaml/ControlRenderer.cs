namespace Manssiere.Core.Graphics.Xaml
{
    using System;
    using System.Windows.Controls;
    using OpenTK.Graphics.OpenGL;

    public class ControlRenderer : ShapeRenderer<Control>
    {
        /// <summary>
        /// Shape specific render code.
        /// </summary>
        /// <param name="shape">The shape to render.</param>
        /// <param name="shapeOpacity">The shape opacity.</param>
        protected override void DrawShape(Control shape, float shapeOpacity)
        {
            if (shapeOpacity == 0 || shape.Background == null)
                return;

            SetBrush(shape.Background, shapeOpacity);

            DrawRectangle((float)shape.Width, (float)shape.Height, (int)shape.GetValue(Panel.ZIndexProperty));
        }

        /// <summary>
        /// Draw the rectangle.
        /// </summary>
        private static void DrawRectangle(Single width, Single height, Single depth)
        {
            GL.Begin(BeginMode.Quads);
            {
                GL.Vertex3(0, 0, depth);
                GL.Vertex3(width, 0, depth);
                GL.Vertex3(width, height, depth);
                GL.Vertex3(0, height, depth);
            }
            GL.End();
        }
    }
}