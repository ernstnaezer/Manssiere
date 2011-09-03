namespace Manssiere.Core.Rendering
{
    using System;
    using Helpers;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;

    public class InitializeOpenGl 
    {
        /// <summary>
        /// Runs the task
        /// </summary>
        /// <remarks>Opengl has been intialized when this method is called.</remarks>
        public void Execute()
        {
            var version = GL.GetString(StringName.Version);
            var major = (int) version[0];
            var minor = (int) version[2];
            if (major <= 2 && minor < 5)
            {
                throw new ApplicationException("You need at least OpenGL 1.5 to run this demo.");
            }

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.PolygonSmooth);

            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
            GL.LightModel(LightModelParameter.LightModelColorControl, (int) LightModelColorControl.SeparateSpecularColor);

            GL.Disable(EnableCap.DepthTest);           
            GL.Disable(EnableCap.CullFace);

            //GL.ClearDepth(1f);5
            //GL.DepthRange(0f, 1f);
            //GL.DepthFunc(DepthFunction.Lequal);
            //GL.Enable(EnableCap.DepthTest);
            
            GlHelper.EnableBlending();

            GL.ClearColor(new Color4(0, 0, 0, 0));

            GL.Clear(ClearBufferMask.AccumBufferBit |
                     ClearBufferMask.ColorBufferBit |
                     ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);
        }

    }
}