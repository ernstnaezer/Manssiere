namespace Manssiere.Core.Graphics.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;
    using Helpers;
    using OpenTK.Graphics.OpenGL;

    /// <summary>
    /// Blending effect
    /// </summary>
    public class BlendEffect : ShaderEffect, IEffect
    {
        public static readonly
            DependencyProperty BlendModeProperty
                = DependencyProperty
                    .Register("BlendMode",
                              typeof(BlendMode),
                              typeof (DemoDependencyHelper),
                              new FrameworkPropertyMetadata(BlendMode.Screen));

        private Shader _shaderAdd;
        private Shader _shaderAverage;
        private Shader _shaderBehind;
        private Shader _shaderClear;
        private Shader _shaderColorBurn;
        private Shader _shaderColorDodge;
        private Shader _shaderDarken;
        private Shader _shaderDifference;
        private Shader _shaderExclusion;
        private Shader _shaderHardLight;
        private Shader _shaderInverseDifference;
        private Shader _shaderLighten;
        private Shader _shaderMultiply;
        private Shader _shaderNormal;
        private Shader _shaderScreen;
        private Shader _shaderSoftLight;
        private Shader _shaderSubstract;

        private IDisposable _removeFramebufferAction;

        public BlendEffect()
        {
            PixelShader = new PixelShader();
        }

        /// <summary>
        /// Gets or sets the framebuffer
        /// </summary>
        public Framebuffer Framebuffer { get; set; }

        /// <summary>
        /// Gets or sets the blendmode
        /// </summary>
        public BlendMode BlendMode
        {
            get { return (BlendMode) GetValue(BlendModeProperty); }
            set { SetValue(BlendModeProperty, value); }
        }

        /// <summary>
        ///   Event triggered before the xaml rendering
        /// </summary>
        /// <param name = "element"></param>
        public void PreRenderAction(UIElement element)
        {
            _removeFramebufferAction = Framebuffer.PushFramebuffer();
        }

        /// <summary>
        ///   Event triggered after the xaml rendering
        /// </summary>
        /// <param name = "element"></param>
        public void PostRenderAction(UIElement element)
        {
            var shader = GetShader();

            if (shader == null || Configuration.ActiveFramebuffer == null || _removeFramebufferAction == null)
                return;

            _removeFramebufferAction.Dispose();

            using (GlHelper.PushTextureMapping())
            {
                var textureHandle0 = Framebuffer.TextureHandle;
                var textureHandle1 = Configuration.ActiveFramebuffer.TextureHandle;
                
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureHandle0);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, textureHandle1);

                using (GlHelper.PushOrthoMatrix(-1, 1, 1, -1))
                {
                    GL.PushAttrib(AttribMask.ViewportBit);
                    {
                        using (shader.PushShader())
                        {
                            shader.SetUniform("opacity", element.Opacity);
                            shader.SetUniform("baseTex", 0);
                            shader.SetUniform("blendTex", 1);

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
                    GL.PopAttrib();
                }

                GL.ActiveTexture(TextureUnit.Texture0);
            }
        }

        /// <summary>
        ///   Runs the task
        /// </summary>
        /// <remarks>
        ///   Opengl has been intialized when this method is called.
        /// </remarks>
        public void Initialize()
        {
            if (IsInitialized) return;
            
            InitializeShaders();
            InitializeFramebuffer();

            IsInitialized = true;
        }

        /// <summary>
        /// Indicates wheter this shader is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get; private set;
        }

        /// <summary>
        ///   Select the shader based on the config
        /// </summary>
        /// <returns></returns>
        private Shader GetShader()
        {
            switch (BlendMode)
            {
                case BlendMode.Normal:
                    return _shaderNormal;
                case BlendMode.Average:
                    return _shaderAverage;
                case BlendMode.Behind:
                    return _shaderBehind;
                case BlendMode.Clear:
                    return _shaderClear;
                case BlendMode.Darken:
                    return _shaderDarken;
                case BlendMode.Lighten:
                    return _shaderLighten;
                case BlendMode.Multiply:
                    return _shaderMultiply;
                case BlendMode.Screen:
                    return _shaderScreen;
                case BlendMode.ColorBurn:
                    return _shaderColorBurn;
                case BlendMode.ColorDodge:
                    return _shaderColorDodge;
                case BlendMode.SoftLight:
                    return _shaderSoftLight;
                case BlendMode.HardLight:
                    return _shaderHardLight;
                case BlendMode.Add:
                    return _shaderAdd;
                case BlendMode.Substract:
                    return _shaderSubstract;
                case BlendMode.Difference:
                    return _shaderDifference;
                case BlendMode.InverseDifference:
                    return _shaderInverseDifference;
                case BlendMode.Exclusion:
                    return _shaderExclusion;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///   Create a new internal framebuffer the xaml tree is rendered to.
        /// </summary>
        private void InitializeFramebuffer()
        {
            Framebuffer = new Framebuffer(Configuration.InternalResolution.Width,
                                           Configuration.InternalResolution.Height);
        }

        /// <summary>
        ///   Initialize the shaders
        /// </summary>
        private void InitializeShaders()
        {
            _shaderAdd = Shader.Load(VertexShaderProg, ShaderAddProg);
            _shaderAverage = Shader.Load(VertexShaderProg, ShaderAverageProg);
            _shaderBehind = Shader.Load(VertexShaderProg, ShaderBehindProg);
            _shaderClear = Shader.Load(VertexShaderProg, ShaderClearProg);
            _shaderColorBurn = Shader.Load(VertexShaderProg, ShaderColorBurnProg);
            _shaderColorDodge = Shader.Load(VertexShaderProg, ShaderColorDodgeProg);
            _shaderDarken = Shader.Load(VertexShaderProg, ShaderDarkenProg);
            _shaderDifference = Shader.Load(VertexShaderProg, ShaderDifferenceProg);
            _shaderExclusion = Shader.Load(VertexShaderProg, ShaderExclusionProg);
            _shaderHardLight = Shader.Load(VertexShaderProg, ShaderHardLightProg);
            _shaderInverseDifference = Shader.Load(VertexShaderProg, ShaderInverseDifferenceProg);
            _shaderLighten = Shader.Load(VertexShaderProg, ShaderLightenProg);
            _shaderMultiply = Shader.Load(VertexShaderProg, ShaderMultiplyProg);
            _shaderNormal = Shader.Load(VertexShaderProg, ShaderNormalProg);
            _shaderScreen = Shader.Load(VertexShaderProg, ShaderScreenProg);
            _shaderSoftLight = Shader.Load(VertexShaderProg, ShaderSoftLightProg);
            _shaderSubstract = Shader.Load(VertexShaderProg, ShaderSubstractProg);
        }

        private const string VertexShaderProg =
            @"
			void main(void) {
				gl_Position = vec4( gl_Vertex.xy, 0.0, 1.0 );
				gl_Position = sign( gl_Position );

				vec4 tc;
				tc.zw = vec2(0,0);
				tc.xy = (vec2( gl_Position.x, gl_Position.y ) + vec2( 1.0 ) ) / vec2( 2.0 );
				tc = gl_TextureMatrix[0] * tc;
				gl_TexCoord[0].xy = tc.xy;
			}
			";

        #region fragment shader code

        private const string ShaderAverageProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity; 
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = (base + blend) * 0.5;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderBehindProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;  
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = (base.a == 0.0) ? blend : base;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderClearProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = vec4(blend.rgb, 0.0);
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderColorBurnProg =
            @"
			vec4 white = vec4(1.0, 1.0, 1.0, 1.0);
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = white - (white - base) / blend;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderColorDodgeProg =
            @"
			vec4 white = vec4(1.0, 1.0, 1.0, 1.0);
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = base / (white - blend);
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderDarkenProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = min(blend, base);
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderDifferenceProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = abs(blend - base);
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderExclusionProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = base + blend - (2.0 * base * blend);
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderHardLightProg =
            @"
			vec4 white = vec4(1.0, 1.0, 1.0, 1.0);
			vec4 lumCoeff = vec4(0.2125, 0.7154, 0.0721, 1.0);
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result;
				float lum = dot(blend, lumCoeff);

				if (lum < 0.45) result = 2.0 * blend * base;
				else if (lum > 0.55) result = white - 2.0 * ((white - blend) * (white - base));
				else {
					vec4 result1 = 2.0 * blend * base;
					vec4 result2 = white - 2.0 * ((white - blend) * (white - base));
					result = mix( result1, result2, (lum - 0.45) * 10.0);
				}
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderInverseDifferenceProg =
            @"
			vec4 white = vec4(1.0, 1.0, 1.0, 1.0);
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = white - abs(white - blend - base);
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderLightenProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = max(blend, base);
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderMultiplyProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = blend * base;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderNormalProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity; 
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = blend;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderScreenProg =
            @"
			vec4 white = vec4(1.0, 1.0, 1.0, 1.0);
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = white - ((white - blend) * (white - base));
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderSoftLightProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = 2.0 * base * blend + base * base - 2.0 + base * base * blend;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderSubstractProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = blend - base;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        private const string ShaderAddProg =
            @"
			uniform sampler2D baseTex;
			uniform sampler2D blendTex;
			uniform float opacity;
			void main() {
				vec4 base = texture2D(baseTex, gl_TexCoord[0].xy);
				vec4 blend = texture2D(blendTex, gl_TexCoord[0].xy);
				vec4 result = blend + base;
				gl_FragColor = mix(base, result, opacity);
			}
		";

        #endregion
    }
}