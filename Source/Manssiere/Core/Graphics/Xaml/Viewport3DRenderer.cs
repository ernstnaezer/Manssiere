namespace Manssiere.Core.Graphics.Xaml
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using Castle.Core;
    using Helpers;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using VertexArayUtility;
    using VertexArayUtility.Buffers;

    /// <summary>
    /// Viewport 3d renderer.
    /// </summary>
    public class Viewport3DRenderer : ShapeRenderer<Viewport3D>
    {
        private static readonly DependencyProperty VertexArrayProperty = DependencyProperty.Register(
            "VertexArray",
            typeof (VertexArray),
            typeof (Viewport3DRenderer),
            new FrameworkPropertyMetadata(default(VertexArray))
            );

        private int _lightCount;

        /// <summary>
        /// Draws the shape.
        /// </summary>
        /// <param name="viewport3D">The viewport3 D.</param>
        /// <param name="shapeOpacity">The shape opacity.</param>
        protected override void DrawShape(Viewport3D viewport3D, float shapeOpacity)
        {
            if (viewport3D == null) throw new ArgumentNullException("viewport3D");
            if (viewport3D.Camera as PerspectiveCamera == null)
                throw new ArgumentException("Viewport has no perspective camera.");

            GL.ClearDepth(1f);
            GL.DepthRange(0f, 1f);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);

            GL.PushAttrib(AttribMask.LightingBit);
            {
                GL.Enable(EnableCap.Lighting);
                using (GlHelper.PushPerspectiveCameraMatrix((PerspectiveCamera) viewport3D.Camera))
                {
                    _lightCount = 0;
                    viewport3D.Children.ForEach(Render);
                }
            }
            GL.PopAttrib();
        }

        /// <summary>
        /// Renders visual 3d content
        /// </summary>
        /// <param name="visual"></param>
        private void Render(Visual3D visual)
        {
            if (!(visual is ModelVisual3D)) return;
            Render(((ModelVisual3D)visual).Content, visual.Transform.Value);
        }

        /// <summary>
        /// Renders 3d model.
        /// </summary>
        /// <param name="visual">The model</param>
        /// <param name="parentTransform">The parent matrix</param>
        private void Render(Model3D visual, Matrix3D parentTransform)
        {
            if (visual is Model3DGroup)
            {
                ((Model3DGroup)visual).Children.ForEach(child => Render(child, visual.Transform.Value * parentTransform));
            }
            else if (visual is GeometryModel3D)
            {
                var geometryModel = (GeometryModel3D)visual;
                var model = (MeshGeometry3D)((GeometryModel3D)visual).Geometry;

                if (model.GetValue(VertexArrayProperty) == null)
                {
                    LoadModelToVertextArray(model);
                }

                GL.PushAttrib(AttribMask.TextureBit);
                {
                    // reset materials
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse,
                                new[] {0.0f, 0.0f, 0.0f, 1.0f});
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new[] {0.0f, 0.0f, 0.0f, 1.0f});
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new[] {0.0f, 0.0f, 0.0f, 1.0f});
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 0.0f);

                    LoadMaterial(MaterialFace.Front, geometryModel.Material);
                    LoadMaterial(MaterialFace.Back, geometryModel.BackMaterial);

                    GL.PushMatrix();
                    {
                        var transform = visual.Transform.Value * parentTransform;
                        var matrix = GlHelper.ConvertToOpenGlMatrix(transform);
                        GL.MultMatrix(ref matrix);

                        var vertexArray = (VertexArray) model.GetValue(VertexArrayProperty);

                        vertexArray.Render();
                    }
                    GL.PopMatrix();
                }
                // texture bit
                GL.PopAttrib();
            }
            else if (visual is AmbientLight)
            {
                var lightColors = GlHelper.GetLightColorValues(((Light)visual).Color, 1.0f);

                GL.LightModel(LightModelParameter.LightModelAmbient, lightColors);
            }
            else if (visual is DirectionalLight)
            {
                var lightName = LightName.Light0 + _lightCount;

                var light = (DirectionalLight)visual;

                var lightColors = GlHelper.GetLightColorValues(light.Color, 1.0f);
                var transform = light.Transform.Value * parentTransform;
                var direction = transform.Transform(light.Direction);

                GL.Light(lightName, LightParameter.Position, new[]
                                                                 {
                                                                     -(float) direction.X,
                                                                     -(float) direction.Y,
                                                                     -(float) direction.Z,
                                                                     0.0f                       // 0 = direction, 1 = position
                                                                 });
                GL.Light(lightName, LightParameter.Diffuse, lightColors);
                GL.Light(lightName, LightParameter.Specular, lightColors);
                GL.Enable(EnableCap.Light0 + _lightCount);

                _lightCount++;
            }
            else if (visual is SpotLight)
            {
                // todo
            }
        }

        /// <summary>
        /// Configure the opengl material.
        /// </summary>
        /// <param name="face"></param>
        /// <param name="material"></param>
        private static void LoadMaterial(MaterialFace face, Material material)
        {
            if (material is MaterialGroup)
            {
                ((MaterialGroup)material).Children.ForEach(m => LoadMaterial(face, m));
            }
            else if (material is DiffuseMaterial)
            {
                var diffuseMaterial = (DiffuseMaterial)material;
                if (diffuseMaterial.Brush is SolidColorBrush)
                {
                    var solidColorBrush = (SolidColorBrush) diffuseMaterial.Brush;

                    GL.Material(face, MaterialParameter.Diffuse,
                                GlHelper.GetLightColorValues(solidColorBrush.Color, (float) solidColorBrush.Opacity));
                }
                else if (diffuseMaterial.Brush is ImageBrush)
                {
                    var texture = DemoDependencyHelper.GetTexture(diffuseMaterial.Brush);

                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, texture.TextureHandle);
                }
            }
            else if (material is SpecularMaterial)
            {
                var specularMaterial = (SpecularMaterial)material;
                if (specularMaterial.Brush as SolidColorBrush == null) return;
                var solidColorBrush = (SolidColorBrush)specularMaterial.Brush;

                var specularPower = (float)specularMaterial.SpecularPower;
                GL.Material(face, MaterialParameter.Shininess, specularPower > 100 ? 100 : specularPower);
                GL.Material(face, MaterialParameter.Specular, GlHelper.GetLightColorValues(solidColorBrush.Color, (float)solidColorBrush.Opacity));
            }
        }

        /// <summary>
        /// Creates a new vertex array and copies the model vector elements.
        /// </summary>
        /// <param name="mesh"></param>
        private static void LoadModelToVertextArray(MeshGeometry3D mesh)
        {
            var vertexArray = new VertexArray
                                  {
                                      Vertices = new VertexBuffer(),
                                      Elements = new ElementBuffer(),
                                      Normals = new NormalBuffer(),
                                      TexCoords = new TexCoordBuffer()
                                  };

            mesh.Positions.ForEach(p => vertexArray.Vertices.Add(new Vector3((float)p.X, (float)p.Y, (float)p.Z)));
            mesh.Normals.ForEach(p => vertexArray.Normals.Add(new Vector3((float)p.X, (float)p.Y, (float)p.Z)));
            mesh.TriangleIndices.ForEach(i => vertexArray.Elements.Add(Convert.ToUInt32(i)));
            mesh.TextureCoordinates.ForEach(t => vertexArray.TexCoords.Add(new Vector2((float) t.X, (float) t.Y)));

            mesh.SetValue(VertexArrayProperty, vertexArray);
        }
    }
}