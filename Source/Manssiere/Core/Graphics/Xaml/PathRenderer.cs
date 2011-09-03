namespace Manssiere.Core.Graphics.Xaml
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Helpers;
    using OpenTK.Graphics.OpenGL;
    using Tesselation;

    /// <summary>
    /// Render a xaml ellipse in opengl.
    /// </summary>
    public class PathRenderer : ShapeRenderer<Shape>
    {
        private static readonly DependencyProperty ShapeDisplayListProperty = DependencyProperty.Register(
            "ShapeDisplayList",
            typeof (int),
            typeof (PathRenderer),
            new FrameworkPropertyMetadata(default(int))
            );

        private static readonly DependencyProperty StrokeDisplayListProperty = DependencyProperty.Register(
            "StrokeDisplayList",
            typeof (int),
            typeof (PathRenderer),
            new FrameworkPropertyMetadata(default(int))
            );

        private readonly TesselateHelper _tesselateHelper = new TesselateHelper();

        /// <summary>
        /// Clears the display lists
        /// </summary>
        /// <param name="dependencyObject"></param>
        public static void ResetDisplayLists(DependencyObject dependencyObject)
        {
            // free up memory
            var shapeList = (int) dependencyObject.GetValue(ShapeDisplayListProperty);
            if(shapeList > 0)
            {
                GL.DeleteLists(shapeList, 1);
            }

            var strokeList = (int)dependencyObject.GetValue(StrokeDisplayListProperty);
            if (strokeList > 0)
            {
                GL.DeleteLists(strokeList, 1);
            }

            dependencyObject.SetValue(ShapeDisplayListProperty, 0);
            dependencyObject.SetValue(StrokeDisplayListProperty, 0);
        }

        /// <summary>
        /// Shape specific render code.
        /// </summary>
        /// <param name="shape">The shape to render.</param>
        /// <param name="shapeOpacity">The shape opacity.</param>
        /// <exception cref="ApplicationException">When an image brush doens't contain an opengl texture.</exception>
        protected override void DrawShape(Shape shape, Single shapeOpacity)
        {
            if (shapeOpacity == 0) return;

            // allow drop resets the display list each frame.
            if (shape.AllowDrop)
                ResetDisplayLists(shape);

            var shapeDisplayList = (int)shape.GetValue(ShapeDisplayListProperty);
            var strokeDisplayList = (int)shape.GetValue(StrokeDisplayListProperty);

            if (shapeDisplayList == 0)
            {
                shapeDisplayList = TesselateShapeDisplayList(shape);
                shape.SetValue(ShapeDisplayListProperty, shapeDisplayList);
            }

            if (strokeDisplayList == 0)
            {
                strokeDisplayList = TesselateStrokeDisplayList(shape);
                shape.SetValue(StrokeDisplayListProperty, strokeDisplayList);
            }

            if (shape.Stroke != null)
            {
                SetBrush(shape.Stroke, shapeOpacity);
                if (shape.Stroke is SolidColorBrush)
                {
                    GL.CallList(strokeDisplayList);
                }
            }

            if (shape.Fill != null)
            {
                if (shape.Fill is ImageBrush)
                {
                    SetBrush(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), shapeOpacity);
                    
                    // yup... very lame :)
                    var texture = DemoDependencyHelper.GetTexture(shape.Fill);
                    if(texture == null) throw new ApplicationException(string.Format(@"Failed loading the texture for {0}", shape.Name));

                    texture.Render((float) shape.ActualWidth, (float) shape.ActualHeight);
                }
                else if(shape.Fill is SolidColorBrush)
                {
                    SetBrush(shape.Fill, shapeOpacity);
                    GL.CallList(shapeDisplayList);
                }
            }
        }

        /// <summary>
        /// Tesselates the shape to a display list.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        private int TesselateShapeDisplayList(Shape shape)
        {
            var list = GL.GenLists(1);
            GL.NewList(list, ListMode.Compile);
            {
                var shapePath = shape.RenderedGeometry.GetFlattenedPathGeometry();

                if (!shapePath.IsEmpty())
                {
                    _tesselateHelper.TessellatePath(shapePath);
                }
            }
            GL.EndList();

            return list;
        }

        /// <summary>
        /// Tesselates the stroke to a display list.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        private int TesselateStrokeDisplayList(Shape shape) {
            
            var pen = new Pen
                          {
                              Thickness = shape.StrokeThickness,
                              Brush = shape.Stroke,
                          };

            var list = GL.GenLists(1);
            GL.NewList(list, ListMode.Compile);
            {
                var strokePath = shape.RenderedGeometry.GetWidenedPathGeometry(pen);

                if (!strokePath.IsEmpty())
                {
                    _tesselateHelper.TessellatePath(strokePath);
                }
            }
            GL.EndList();

            return list;
        }
    }
}