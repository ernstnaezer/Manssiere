namespace Manssiere.Core.Graphics
{
    using System;
    using System.Windows;
    using OpenTK.Graphics.OpenGL;

    /// <summary>
    ///   Opengl framebuffer.
    /// </summary>
    public class Framebuffer : FrameworkElement, ITexture
    {
        private uint _depthHandle;
        private uint _fboHandle;
        private uint _textureHandle;

        /// <summary>
        ///   Create a new framebuffer.
        /// </summary>
        /// <param name = "width">The width of the framebuffer</param>
        /// <param name = "height">The width of the framebuffer</param>
        public Framebuffer(int width, int height)
        {
            if (!GL.GetString(StringName.Extensions).Contains("EXT_framebuffer_object"))
            {
                throw new Exception(
                    "Your video card does not support Framebuffer Objects. Please update your drivers.");
            }

            RenderSize = new Size(width, height);

            GL.GenTextures(1, out _textureHandle);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                          (int) RenderSize.Width,
                          (int) RenderSize.Height, 0,
                          PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMinFilter,
                            (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureMagFilter,
                            (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapS,
                            (int) TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D,
                            TextureParameterName.TextureWrapT,
                            (int) TextureWrapMode.ClampToBorder);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.GenTextures(1, out _depthHandle);
            GL.BindTexture(TextureTarget.Texture2D, _depthHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat) All.DepthComponent24,
                          (int) RenderSize.Width,
                          (int) RenderSize.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                            (int) TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                            (int) TextureWrapMode.ClampToBorder);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Ext.GenFramebuffers(1, out _fboHandle);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fboHandle);

            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt,
                                        FramebufferAttachment.DepthAttachmentExt,
                                        TextureTarget.Texture2D, _depthHandle, 0);

            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt,
                            FramebufferAttachment.ColorAttachment0Ext,
                            TextureTarget.Texture2D, _textureHandle, 0);

            var errorCode = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (errorCode != FramebufferErrorCode.FramebufferCompleteExt)
            {
                throw new Exception(string.Format("Framebuffer construction failed with error: {0}", errorCode));
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        /// <summary>
        ///   Gets the texture handle.
        /// </summary>
        public uint TextureHandle
        {
            get { return _textureHandle; }
        }

        /// <summary>
        /// Gets the framebuffer handle
        /// </summary>
        public uint Handle
        {
            get { return _fboHandle; }
        }

        ///<summary>
        ///  Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_textureHandle != 0)
                GL.DeleteTextures(1, ref _textureHandle);

            // Clean up what we allocated before exiting
            if (_fboHandle != 0)
                GL.DeleteFramebuffers(1, ref _fboHandle);

            if (_depthHandle != 0)
                GL.DeleteTextures(1, ref _depthHandle);
        }

        /// <summary>
        ///   Helper to activate the framebuffer in a using construction.
        /// </summary>
        /// <returns>The framebuffer dispose helper.</returns>
        public IDisposable PushFramebuffer()
        {
            var restore = Configuration.ActiveFramebuffer;
            Configuration.ActiveFramebuffer = this;

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fboHandle);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            return new DisposableAction(() =>
                                            {
                                                var handle = restore != null ? restore.Handle : 0;
                                                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, handle);
                                                Configuration.ActiveFramebuffer = restore;
                                            });
        }
    }
}