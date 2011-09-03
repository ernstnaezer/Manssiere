namespace Manssiere.Core.Graphics.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;
    using Helpers;
    using OpenTK.Graphics.OpenGL;

    public class BlurEffect : ShaderEffect, IEffect
    {
        private static Shader _poisionDiskShader;
        private static Shader _gaussianShader;
        private readonly BlendEffect _blendEffect = new BlendEffect();

        #region Shadercode

        private const string VertexShaderProg =
            @"varying vec2 texCoord;

            void main(void)
            {
               gl_Position = vec4( gl_Vertex.xy, 0.0, 1.0 );
               gl_Position = sign( gl_Position );
               
               // Texture coordinate for screen aligned (in correct range):
               texCoord = (vec2( gl_Position.x,  gl_Position.y ) + vec2( 1.0 ) ) / vec2( 2.0 );   
            }";

        private const string GaussianFragementShaderProg =
            @"
                uniform sampler2D Texture0;
                uniform float radius;
                varying vec2 texCoord;

                void main(void) {
                	vec2 kernel[16];
                	kernel[0] = vec2( -0.627955, -0.192227 );
                	kernel[1] = vec2( -0.102321, -0.052551 );
                	kernel[2] = vec2( 0.031971, -0.833506 );
                	kernel[3] = vec2( 0.964006, 0.939178 );
                	kernel[4] = vec2( -0.532050, 0.780129 );
                	kernel[5] = vec2( 0.752531, -0.377451 );
                	kernel[6] = vec2( 0.694767, 0.337059 );
                	kernel[7] = vec2( 0.244456, 0.581036 );
                	kernel[8] = vec2( 0.615339, -0.863584 );
                	kernel[9] = vec2( -0.950707, -0.674839 );
                	kernel[10] = vec2( -0.935346, 0.447946 );
                	kernel[11] = vec2( -0.444211, -0.732845 );
                	kernel[12] = vec2( 0.401385, -0.044408 );
                	kernel[13] = vec2( -0.997905, 0.940210 );
                	kernel[14] = vec2( -0.042914, 0.994714 );
                	kernel[15] = vec2( -0.462629, 0.301998 );
                	
                	vec4 color = vec4(0);

                	for (int i = 0; i < 16; i++) {
                		color += texture2D(Texture0, texCoord+(kernel[i]*vec2(radius / 100.0)));
                	}

                	color /= vec4(16);

                	gl_FragColor = color;
                }
        ";

        private const string PoisionDiskFragmentShaderProg =
            @"
            uniform sampler2D Texture0;
            uniform float radius;

            varying vec2 texCoord;

            void main(void)
            {
               vec2 poisson[12];
               poisson[0] = vec2(-0.326212, -0.40581);
               poisson[1] = vec2(-0.840144, -0.07358);
               poisson[2] = vec2(-0.695914, 0.457137);
               poisson[3] = vec2(-0.203345, 0.620716);
               poisson[4] = vec2(0.96234, -0.194983);
               poisson[5] = vec2(0.473434, -0.480026);
               poisson[6] = vec2(0.519456, 0.767022);
               poisson[7] = vec2(0.185461, -0.893124);
               poisson[8] = vec2(0.507431, 0.064425);   
               poisson[9] = vec2(0.89642, 0.412458);
               poisson[10] = vec2(-0.32194, -0.932615);
               poisson[11] = vec2(-0.791559, -0.59771);

               vec4 color = vec4(0);

               for(int i = 0; i < 12; i++)
               {
                  color +=  texture2D( Texture0, texCoord + (poisson[i] * radius) * 0.01);
               }

               gl_FragColor = color / 12.0;;
            }";

        #endregion

        public BlurEffect()
        {
            PixelShader = new PixelShader();
        }

        /// <summary>
        ///   Gets or sets the shader type
        /// </summary>
        public BlurShaderType BlurShaderType { get; set; }

        public float Radius { get; set; }

        /// <summary>
        ///   Initiaze the effect
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized) return;

            _poisionDiskShader = Shader.Load(VertexShaderProg, PoisionDiskFragmentShaderProg);
            _gaussianShader = Shader.Load(VertexShaderProg, GaussianFragementShaderProg);

            _blendEffect.BlendMode = BlendMode.Normal;
            _blendEffect.Initialize();

            IsInitialized = true;
        }

        /// <summary>
        ///   Indicates wheter this shader is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///   Event triggered before the xaml rendering
        /// </summary>
        /// <param name = "element"></param>
        public void PreRenderAction(UIElement element)
        {
            _blendEffect.PreRenderAction(element);
        }

        /// <summary>
        ///   Event triggered after the xaml rendering
        /// </summary>
        /// <param name = "element"></param>
        public void PostRenderAction(UIElement element)
        {
            var framebuffer = Configuration.ActiveFramebuffer;
            if (framebuffer == null) return;

            var shader = GetShader();

            using (GlHelper.PushTextureMapping())
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _blendEffect.Framebuffer.TextureHandle);

                using (GlHelper.PushOrthoMatrix(-1, 1, 1, -1))
                {
                    using (shader.PushShader())
                    {
                        shader.SetUniform("radius", 0.5);
                        shader.SetUniform("Texture0", 0);

                        GL.Begin(BeginMode.Quads);
                        {
                            GL.TexCoord2(0, 1);
                            GL.Vertex2(-1.0, -1.0);

                            GL.TexCoord2(1, 1);
                            GL.Vertex2(1.0, -1.0);

                            GL.TexCoord2(1, 0);
                            GL.Vertex2(1.0, 1.0);

                            GL.TexCoord2(0, 0);
                            GL.Vertex2(-1.0, 1.0);
                        }
                        GL.End();
                    }
                }
            }

            _blendEffect.PostRenderAction(element);
        }

        /// <summary>
        ///   Gets the blurshader based on the
        ///   <see cref = "BlurShaderType" />
        ///   property.
        /// </summary>
        /// <returns></returns>
        private Shader GetShader()
        {
            switch (BlurShaderType)
            {
                case BlurShaderType.PoisionDisk:
                    return _poisionDiskShader;
                case BlurShaderType.Gaussian:
                    return _gaussianShader;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}