namespace Manssiere.Core.Graphics.Xaml
{
    using System.Drawing;
    using Helpers;
    using OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Renders a framebuffer to a quad.
    /// </summary>
    public class FramebufferRenderer : ShapeRenderer<Framebuffer>
    {
        /// <summary>
        ///   Shape specific render code.
        /// </summary>
        /// <param name="shape">The shape to render.</param>
        /// <param name="shapeOpacity">The shape opacity.</param>
        protected override void DrawShape(Framebuffer shape, float shapeOpacity)
        {
            // we draw the framebuffer without blending
            // to prevent a double alpha function resulting in a dark border
            GL.Disable(EnableCap.Blend);
            {
                using (GlHelper.PushTextureMapping())
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, shape.TextureHandle);

                    GL.Color3(Color.Transparent);
                    var height = shape.RenderSize.Height;
                    var width = shape.RenderSize.Width;

                    GL.Begin(BeginMode.Quads);
                    {
                        GL.TexCoord2(0.0f, 1.0f);
                        GL.Vertex2(0, 0);

                        GL.TexCoord2(0.0f, 0.0f);
                        GL.Vertex2(0, height);

                        GL.TexCoord2(1.0f, 0.0f);
                        GL.Vertex2(width, height);

                        GL.TexCoord2(1.0f, 1.0f);
                        GL.Vertex2(width, 0);
                    }
                    GL.End();
                }
            }
            GlHelper.EnableBlending();

        }


    }
}