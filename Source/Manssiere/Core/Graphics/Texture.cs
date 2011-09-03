namespace Manssiere.Core.Graphics
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Helpers;
    using OpenTK.Graphics.OpenGL;
    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    /// <summary>
    /// Opengl material binding.
    /// </summary>
    /// <remarks>textures must by of size 2n (16,32,64,256,512,etc.etc)</remarks>
    public class Texture : ITexture
    {
        private uint _textureHandle;

        /// <summary>
        /// Initialize code
        /// </summary>
        private Texture()
        {
            GL.GenTextures(1, out _textureHandle);

            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);

            // set texture properties
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,(int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,(int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
 
            // unbind the texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #region ITexture Members

        /// <summary> 
        /// Gets the texture handle.
        /// </summary>
        public uint TextureHandle
        {
            get { return _textureHandle; }
        }

        /// <summary>
        /// Render the texture to the active screen
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height..</param>
        public void Render(float width, float height)
        {
            using (GlHelper.PushTextureMapping())
            {
                using (PushTexture())
                {
                    GL.Begin(BeginMode.Quads);
                    {
                        GL.TexCoord2(0.0f, 0.0f);
                        GL.Vertex2(0, 0);

                        GL.TexCoord2(0.0f, 1.0f);
                        GL.Vertex2(0, height);

                        GL.TexCoord2(1.0f, 1.0f);
                        GL.Vertex2(width, height);

                        GL.TexCoord2(1.0f, 0.0f);
                        GL.Vertex2(width, 0);
                    }
                    GL.End();
                }
            }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_textureHandle != 0)
                GL.DeleteTextures(1, ref _textureHandle);
        }

        #endregion

        /// <summary>
        /// Pushes the texture into opengl.
        /// </summary>
        /// <returns></returns>
        private IDisposable PushTexture()
        {
            GL.PushAttrib(AttribMask.TextureBit);
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, _textureHandle);

                return new DisposableAction(() =>
                                                {
                                                    GL.PopAttrib();
                                                    GL.BindTexture(TextureTarget.Texture2D, 0);
                                                });
            }
        }

        /// <summary>
        /// Upload an image to opengl.
        /// </summary>
        /// <param name="bitmap">The image.</param>
        public static Texture FromBitmap(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException("bitmap");
           
            var size = new Size(bitmap.Width, bitmap.Height);

            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported pixelformat " + bitmap.PixelFormat);
            }

            var rect = new Rectangle(0, 0, size.Width, size.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var texture = new Texture();
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureHandle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, 
                           PixelInternalFormat.Rgba, size.Width, size.Height, 0,
                           GetPixelFormat(bitmap), PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);

            // unbind after loading
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return texture;
        }

        private static PixelFormat GetPixelFormat(Bitmap bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormat.Bgr;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return PixelFormat.Bgra;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormat.Bgra;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}