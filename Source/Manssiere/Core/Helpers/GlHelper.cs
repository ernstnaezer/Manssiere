namespace Manssiere.Core.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using Manssiere.Core;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using Color = System.Windows.Media.Color;
    using Configuration = Core.Configuration;
    using PixelFormat = System.Drawing.Imaging.PixelFormat;

    /// <summary>
    /// Helper class for ortho (2d) view.
    /// </summary>
    public static class GlHelper
    {
        /// <summary>
        ///   Grabs the screenshot.
        /// </summary>
        /// <returns></returns>
        public static Bitmap GrabScreenshot()
        {
            var bounds = new Rectangle(new Point(0, 0),
                                       new Size(Configuration.DisplayResolution.Width,
                                                Configuration.DisplayResolution.Height));

            var bmp = new Bitmap(Configuration.DisplayResolution.Width, Configuration.DisplayResolution.Height);
            var data = bmp.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, Configuration.DisplayResolution.Width,
                          Configuration.DisplayResolution.Height,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        /// <summary>
        /// Enable blending using one minus src alpha.
        /// </summary>
        public static void EnableBlending()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public static Matrix4 ConvertToOpenGlMatrix(Matrix localTransform)
        {
            return new Matrix4((float)localTransform.M11, (float)localTransform.M12, 0, 0,
                               (float)localTransform.M21, (float)localTransform.M22, 0, 0,
                               0, 0, 1, 0,
                               (float)localTransform.OffsetX, (float)localTransform.OffsetY, 0, 1);
        }

        /// <summary>
        /// Convert a WPF matrix to an opengl edition
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Matrix4 ConvertToOpenGlMatrix(Matrix3D transform)
        {
            return new Matrix4((float)transform.M11, (float)transform.M12, (float)transform.M13,
                               (float)transform.M14,
                               (float)transform.M21, (float)transform.M22, (float)transform.M23,
                               (float)transform.M24,
                               (float)transform.M31, (float)transform.M32, (float)transform.M33,
                               (float)transform.M34,
                               (float)transform.OffsetX, (float)transform.OffsetY,
                               (float)transform.OffsetZ,
                               (float)transform.M44);
        }

        /// <summary>
        /// Set the projection matrix to an orthographic matrix at screensize.
        /// The restoring is done using a disposable action.
        /// </summary>
        /// <returns>The restore action.</returns>
        public static IDisposable PushOrthoMatrix()
        {
            CreateOrthoMatrix(0,
                              Configuration.InternalResolution.Width,
                              Configuration.InternalResolution.Height,
                              0);

            return new DisposableAction(RestoreMatrix);
        }

        /// <summary>
        /// Set the projection matrix to an orthographic matrix at the given size.
        /// The restoring is done using a disposable action.
        /// </summary>
        /// <returns>The restore action.</returns>
        public static IDisposable PushOrthoMatrix(float left, float right, float bottom, float top)
        {
            CreateOrthoMatrix(left, right, bottom, top);

            return new DisposableAction(RestoreMatrix);
        }
      
        /// <summary>
        /// Enables texturemapping with alpha channel support.
        /// </summary>
        /// <returns>A disposable action that disables texturemapping.</returns>
        public static IDisposable PushTextureMapping()
        {
            GL.PushAttrib(AttribMask.TextureBit);
            GL.Enable(EnableCap.Texture2D);

            return new DisposableAction(GL.PopAttrib);
        }

        /// <summary>
        /// Save the current projection matrix and switch to ortho view.
        /// </summary>
        private static void CreateOrthoMatrix(Single left, Single right, Single bottom, Single top)
        {
            GL.MatrixMode(MatrixMode.Projection);

            // save the current matrix
            GL.PushMatrix();

            // setup a orthographic view
            GL.LoadIdentity();
            GL.Ortho(left, right, bottom, top, -100, 100);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            GL.LoadIdentity();
        }

        /// <summary>
        /// Restore the saved projection matrix.
        /// </summary>
        private static void RestoreMatrix()
        {
            // restore the model view matrix
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            // restore the projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
        }

        /// <summary>
        /// Load a project and view matrix based on the give perspective camera.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static DisposableAction PushPerspectiveCameraMatrix(PerspectiveCamera camera)
        {
            var fov = (float) Math.Tan(OpenTK.MathHelper.DegreesToRadians((float) camera.FieldOfView));

            var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fov,               
                                                                        1.7777f,
                                                                        (float) camera.NearPlaneDistance,
                                                                        (float) camera.FarPlaneDistance);

            GL.MatrixMode(MatrixMode.Projection);
            // save the current matrix
            GL.PushMatrix();

            GL.LoadMatrix(ref projectionMatrix);

            var lookat = Matrix4.LookAt(
                                           (float) camera.Position.X,
                                           (float) camera.Position.Y,
                                           (float) camera.Position.Z,
                                           (float) camera.Position.X + (float) camera.LookDirection.X,
                                           (float) camera.Position.Y + (float) camera.LookDirection.Y,
                                           (float) camera.Position.Z + (float) camera.LookDirection.Z,
                                           (float) camera.UpDirection.X,
                                           (float) camera.UpDirection.Y,
                                           (float) camera.UpDirection.Z
                );

            GL.MatrixMode(MatrixMode.Modelview);
            // save the current matrix
            GL.PushMatrix();

            GL.LoadMatrix(ref lookat);

            return new DisposableAction(RestoreMatrix);
        }

        /// <summary>
        /// Convert a Color to a float array for opengl.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="opacity"></param>
        /// <returns></returns>
        public static float[] GetLightColorValues(Color color, float opacity)
        {
            return new[]
                       {
                           opacity*(((float) color.R)/byte.MaxValue),
                           opacity*(((float) color.G)/byte.MaxValue),
                           opacity*(((float) color.B)/byte.MaxValue),
                           opacity*(((float) color.A)/byte.MaxValue),
                       };
        }
    }
}