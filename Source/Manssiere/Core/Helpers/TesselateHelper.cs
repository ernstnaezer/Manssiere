namespace Manssiere.Core.Graphics.Tesselation
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using Castle.Core;

    /// <summary>
    /// Tessellate definitions for 2d tessellation.
    /// </summary>
    public class TesselateHelper
    {
        private readonly Tesselator.Tesselator _tesselator = new Tesselator.Tesselator();
        private readonly List<Vector3> _vertexList = new List<Vector3>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TesselateHelper"/> class.
        /// </summary>
        public TesselateHelper()
        {   
            _tesselator.callBegin += TesselatorCallBegin;
            _tesselator.callEnd += TesselatorCallEnd;
            _tesselator.callVertex += TesselatorCallVertex;
            _tesselator.callCombine += TesselatorCallCombine;
        }

        /// <summary>
        /// Tessellates the path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void TessellatePath(PathGeometry path)
        {
            _vertexList.Clear();
            _tesselator.EmptyCache();
            _tesselator.BeginPolygon();
            
            switch(path.FillRule)
            {
                case FillRule.EvenOdd:
                    _tesselator.WindingRule = Tesselator.Tesselator.WindingRuleType.Odd;
                    break;
                case FillRule.Nonzero:
                    _tesselator.WindingRule = Tesselator.Tesselator.WindingRuleType.NonZero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var figure in path.Figures)
            {
                _tesselator.BeginContour();
                
                foreach (var seg in figure.Segments)
                {
                    if (seg is PolyLineSegment)
                    {
                        var polyline = seg as PolyLineSegment;
                        polyline.Points.ForEach(AddPoint);
                    }
                    else if (seg is LineSegment)
                    {
                        AddPoint((seg as LineSegment).Point);
                    }
                    else if (seg is PolyBezierSegment)
                    {
                        var polybezier = (PolyBezierSegment) seg;

                        var bezierCurve = new BezierCurve(polybezier.Points.Select(p => new Vector2((float) p.X, (float) p.Y)));

                        Enumerable
                            .Range(1, 10)
                            .Select(i => bezierCurve.CalculatePoint(1.0f/i))
                            .ForEach(p => AddPoint(new Point(p.X, p.Y)));
                    }
                    else if (seg is BezierSegment)
                    {
                        var polybezier = (BezierSegment)seg;

                        var bezierCurve = new BezierCurveCubic(
                            LastPoint(), 
                            new Vector2((float) polybezier.Point3.X, (float) polybezier.Point3.Y),
                            new Vector2((float) polybezier.Point1.X, (float) polybezier.Point1.Y),
                            new Vector2((float) polybezier.Point2.X, (float) polybezier.Point2.Y)
                            );

                        Enumerable
                            .Range(1, 10)
                            .Select(i => bezierCurve.CalculatePoint(1.0f / i))
                            .ForEach(p => AddPoint(new Point(p.X, p.Y)));
                    }
                    else
                    {
                        throw new ApplicationException(string.Format("segment type {0} not supported.", seg));
                    }
                }
                _tesselator.EndContour();
            }
            _tesselator.EndPolygon();
        }

        private Vector2 LastPoint()
        {
            var l = _vertexList.Any() ? _vertexList.Last() : new Vector3();
            return new Vector2(l.X, l.Y); 
        }

        private void AddPoint(Point t)
        {
            var point = new[] {t.X, t.Y, 1d};

            _tesselator.AddVertex(point, _vertexList.Count);
            _vertexList.Add(new Vector3((float) point[0], (float) point[1], (float) point[2]));
        }

        #region Tesselator callbacks

        private void TesselatorCallCombine(double[] coords3, int[] data4, double[] weight4, out int outData)
        {
            outData = _vertexList.Count;
            _vertexList.Add(new Vector3((float) coords3[0], (float) coords3[1], (float) coords3[2]));
        }

        private void TesselatorCallVertex(int data)
        {
            GL.Vertex3(_vertexList[data]);
        }

        private static void TesselatorCallEnd()
        {
            GL.End();
        }

        private static void TesselatorCallBegin(Tesselator.Tesselator.TriangleListType type)
        {
            BeginMode beginMode;

            switch (type)
            {
                case Tesselator.Tesselator.TriangleListType.LineLoop:
                    beginMode = BeginMode.LineLoop;
                    break;
                case Tesselator.Tesselator.TriangleListType.Triangles:
                    beginMode = BeginMode.Triangles;
                    break;
                case Tesselator.Tesselator.TriangleListType.TriangleStrip:
                    beginMode = BeginMode.TriangleStrip;
                    break;
                case Tesselator.Tesselator.TriangleListType.TriangleFan:
                    beginMode = BeginMode.TriangleFan;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            GL.Begin(beginMode);
        }

        #endregion
    }
}