/*
** License Applicability. Except to the extent portions of this file are
** made subject to an alternative license as permitted in the SGI Free
** Software License B, Version 1.1 (the "License"), the contents of this
** file are subject only to the provisions of the License. You may not use
** this file except in compliance with the License. You may obtain a copy
** of the License at Silicon Graphics, Inc., attn: Legal Services, 1600
** Amphitheatre Parkway, Mountain View, CA 94043-1351, or at:
** 
** http://oss.sgi.com/projects/FreeB
** 
** Note that, as provided in the License, the Software is distributed on an
** "AS IS" basis, with ALL EXPRESS AND IMPLIED WARRANTIES AND CONDITIONS
** DISCLAIMED, INCLUDING, WITHOUT LIMITATION, ANY IMPLIED WARRANTIES AND
** CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY, FITNESS FOR A
** PARTICULAR PURPOSE, AND NON-INFRINGEMENT.
** 
** Original Code. The Original Code is: OpenGL Sample Implementation,
** Version 1.2.1, released January 26, 2000, developed by Silicon Graphics,
** Inc. The Original Code is Copyright (c) 1991-2000 Silicon Graphics, Inc.
** Copyright in any portions created by third parties is as indicated
** elsewhere herein. All Rights Reserved.
** 
** Additional Notice Provisions: The application programming interfaces
** established by SGI in conjunction with the Original Code are The
** OpenGL(R) Graphics System: A Specification (Version 1.2.1), released
** April 1, 1999; The OpenGL(R) Graphics System Utility Library (Version
** 1.3), released November 4, 1998; and OpenGL(R) Graphics with the X
** Window System(R) (Version 1.3), released October 19, 1998. This software
** was created using the OpenGL(R) version 1.2.1 Sample Implementation
** published by SGI, but has not been independently verified as being
** compliant with the OpenGL(R) version 1.2.1 Specification.
**
*/
/*
** Author: Eric Veach, July 1994.
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
**
*/

namespace Manssiere.Core.Graphics.Tesselation.Tesselator
{
    using System;
    using C5;

    public class Tesselator
    {
        /* The begin/end calls must be properly nested.  We keep track of
        * the current state to enforce the ordering.
        */

        #region Delegates

        public delegate void CallBeginDelegate(TriangleListType type);

        public delegate void CallCombineDelegate(double[] coords3, int[] data4,
                                                 double[] weight4, out int outData);

        public delegate void CallEdgeFlagDelegate(bool boundaryEdge);

        public delegate void CallEndDelegate();

        public delegate void CallMeshDelegate(Mesh mesh);

        public delegate void CallVertexDelegate(int data);

        #endregion

        #region TriangleListType enum

        public enum TriangleListType
        {
            LineLoop,
            Triangles,
            TriangleStrip,
            TriangleFan
        }

        #endregion

        #region WindingRuleType enum

        public enum WindingRuleType
        {
            Odd,
            NonZero,
            Positive,
            Negative,
            ABS_GEQ_Two,
        }

        #endregion

        private const int MAX_CACHE_SIZE = 100;
        public const double MAX_COORD = 1.0e150;
        private bool boundaryOnly; /* Extract contours, not triangles */
        public int cacheCount; /* number of cached vertices */
        public ContourVertex currentSweepVertex; /* current sweep event being processed */
        public Dictionary edgeDictionary; /* edge dictionary for sweep line */
        private bool emptyCache; /* empty cache on next vertex() call */

        private HalfEdge lastHalfEdge; /* lastEdge.Org is the most recent vertex */
        public Face lonelyTriList;
        public Mesh mesh; /* stores the input contours, and eventually the tessellation itself */
        private ProcessingState processingState; /* what begin/end calls have we seen? */
        public VertexAndIndex[] simpleVertexCache = new VertexAndIndex[MAX_CACHE_SIZE]; /* the vertex data */

        public IntervalHeap<ContourVertex> vertexPriorityQue = new IntervalHeap<ContourVertex>();
        public WindingRuleType windingRule; // rule for determining polygon interior

        public Tesselator()
        {
            /* Only initialize fields which can be changed by the api.  Other fields
            * are initialized where they are used.
            */

            processingState = ProcessingState.Dormant;

            windingRule = WindingRuleType.NonZero;
            boundaryOnly = false;
        }

        public bool EdgeCallBackSet
        {
            get { return callEdgeFlag != null; }
        }

        public WindingRuleType WindingRule
        {
            get { return windingRule; }
            set { windingRule = value; }
        }

        public bool BoundaryOnly
        {
            get { return boundaryOnly; }
            set { boundaryOnly = value; }
        }

        public event CallCombineDelegate callCombine;

        /*** state needed for rendering callbacks (see render.c) ***/

        public event CallBeginDelegate callBegin;

        public event CallEdgeFlagDelegate callEdgeFlag;

        public event CallVertexDelegate callVertex;

        public event CallEndDelegate callEnd;

        public event CallMeshDelegate callMesh;


        /*** state needed to cache single-contour polygons for renderCache() */

        ~Tesselator()
        {
            RequireState(ProcessingState.Dormant);
        }

        public bool IsWindingInside(int numCrossings)
        {
            switch (windingRule)
            {
                case WindingRuleType.Odd:
                    return (numCrossings & 1) != 0;

                case WindingRuleType.NonZero:
                    return (numCrossings != 0);

                case WindingRuleType.Positive:
                    return (numCrossings > 0);

                case WindingRuleType.Negative:
                    return (numCrossings < 0);

                case WindingRuleType.ABS_GEQ_Two:
                    return (numCrossings >= 2) || (numCrossings <= -2);
            }

            throw new Exception();
        }

        public void CallBegin(TriangleListType triangleType)
        {
            if (callBegin != null)
            {
                callBegin(triangleType);
            }
        }

        public void CallVertex(int vertexData)
        {
            if (callVertex != null)
            {
                callVertex(vertexData);
            }
        }

        public void CallEdegFlag(bool edgeState)
        {
            if (callEdgeFlag != null)
            {
                callEdgeFlag(edgeState);
            }
        }

        public void CallEnd()
        {
            if (callEnd != null)
            {
                callEnd();
            }
        }

        public void CallCombine(double[] coords3, int[] data4,
                                double[] weight4, out int outData)
        {
            outData = 0;
            if (callCombine != null)
            {
                callCombine(coords3, data4, weight4, out outData);
            }
        }

        private void GotoState(ProcessingState newProcessingState)
        {
            while (processingState != newProcessingState)
            {
                /* We change the current state one level at a time, to get to
                * the desired state.
                */
                if (processingState < newProcessingState)
                {
                    switch (processingState)
                    {
                        case ProcessingState.Dormant:
                            throw new Exception("MISSING_BEGIN_POLYGON");

                        case ProcessingState.InPolygon:
                            throw new Exception("MISSING_BEGIN_CONTOUR");

                        default:
                            break;
                    }
                }
                else
                {
                    switch (processingState)
                    {
                        case ProcessingState.InContour:
                            throw new Exception("MISSING_END_CONTOUR");

                        case ProcessingState.InPolygon:
                            throw new Exception("MISSING_END_POLYGON");

                        default:
                            break;
                    }
                }
            }
        }

        private void RequireState(ProcessingState state)
        {
            if (processingState != state)
            {
                GotoState(state);
            }
        }

        public virtual void BeginPolygon()
        {
            RequireState(ProcessingState.Dormant);

            processingState = ProcessingState.InPolygon;
            cacheCount = 0;
            emptyCache = false;
            mesh = null;
        }

        public void BeginContour()
        {
            RequireState(ProcessingState.InPolygon);

            processingState = ProcessingState.InContour;
            lastHalfEdge = null;
            if (cacheCount > 0)
            {
                // Just set a flag so we don't get confused by empty contours
                emptyCache = true;
            }
        }

        private bool AddVertex(double x, double y, int data)
        {
            HalfEdge e;

            e = lastHalfEdge;
            if (e == null)
            {
                /* Make a self-loop (one vertex, one edge). */
                e = mesh.MakeEdge();
                Mesh.meshSplice(e, e.otherHalfOfThisEdge);
            }
            else
            {
                /* Create a new vertex and edge which immediately follow e
                * in the ordering around the left face.
                */
                if (Mesh.meshSplitEdge(e) == null)
                {
                    return false;
                }
                e = e.nextEdgeCCWAroundLeftFace;
            }

            /* The new vertex is now e.Org. */
            e.originVertex.clientIndex = data;
            e.originVertex.coords[0] = x;
            e.originVertex.coords[1] = y;

            /* The winding of an edge says how the winding number changes as we
            * cross from the edge''s right face to its left face.  We add the
            * vertices in such an order that a CCW contour will add +1 to
            * the winding number of the region inside the contour.
            */
            e.winding = 1;
            e.otherHalfOfThisEdge.winding = -1;

            lastHalfEdge = e;

            return true;
        }

        public void EmptyCache()
        {
            VertexAndIndex[] v = simpleVertexCache;

            mesh = new Mesh();

            for (int i = 0; i < cacheCount; i++)
            {
                AddVertex(v[i].x, v[i].y, v[i].vertexIndex);
            }
            cacheCount = 0;
            emptyCache = false;
        }

        private void CacheVertex(double[] coords3, int data)
        {
            simpleVertexCache[cacheCount].vertexIndex = data;
            simpleVertexCache[cacheCount].x = coords3[0];
            simpleVertexCache[cacheCount].y = coords3[1];
            ++cacheCount;
        }

        public void AddVertex(double[] coords3, int data)
        {
            int i;
            double x;
            var clamped = new double[3];

            RequireState(ProcessingState.InContour);

            if (emptyCache)
            {
                EmptyCache();
                lastHalfEdge = null;
            }

            for (i = 0; i < 3; ++i)
            {
                x = coords3[i];
                if (x < -MAX_COORD)
                {
                    throw new Exception("Your coordinate exceeded -" + MAX_COORD + ".");
                }
                if (x > MAX_COORD)
                {
                    throw new Exception("Your coordinate exceeded " + MAX_COORD + ".");
                }
                clamped[i] = x;
            }

            if (mesh == null)
            {
                if (cacheCount < MAX_CACHE_SIZE)
                {
                    CacheVertex(clamped, data);
                    return;
                }
                EmptyCache();
            }

            AddVertex(clamped[0], clamped[1], data);
        }

        public void EndContour()
        {
            RequireState(ProcessingState.InContour);
            processingState = ProcessingState.InPolygon;
        }

        private void CheckOrientation()
        {
            double area;
            Face curFace, faceHead = mesh.faceHead;
            ContourVertex vHead = mesh.vertexHead;
            HalfEdge curHalfEdge;

            /* When we compute the normal automatically, we choose the orientation
             * so that the the sum of the signed areas of all contours is non-negative.
             */
            area = 0;
            for (curFace = faceHead.nextFace; curFace != faceHead; curFace = curFace.nextFace)
            {
                curHalfEdge = curFace.halfEdgeThisIsLeftFaceOf;
                if (curHalfEdge.winding <= 0)
                {
                    continue;
                }

                do
                {
                    area += (curHalfEdge.originVertex.x - curHalfEdge.directionVertex.x)
                            *(curHalfEdge.originVertex.y + curHalfEdge.directionVertex.y);
                    curHalfEdge = curHalfEdge.nextEdgeCCWAroundLeftFace;
                } while (curHalfEdge != curFace.halfEdgeThisIsLeftFaceOf);
            }

            if (area < 0)
            {
                /* Reverse the orientation by flipping all the t-coordinates */
                for (ContourVertex curVertex = vHead.nextVertex; curVertex != vHead; curVertex = curVertex.nextVertex)
                {
                    curVertex.y = -curVertex.y;
                }
            }
        }

        private void ProjectPolygon()
        {
            ContourVertex v, vHead = mesh.vertexHead;

            // Project the vertices onto the sweep plane
            for (v = vHead.nextVertex; v != vHead; v = v.nextVertex)
            {
                v.x = v.coords[0];
                v.y = -v.coords[1];
            }

            CheckOrientation();
        }

        public void EndPolygon()
        {
            RequireState(ProcessingState.InPolygon);
            processingState = ProcessingState.Dormant;

            if (mesh == null)
            {
                if (!EdgeCallBackSet && callMesh == null)
                {
                    /* Try some special code to make the easy cases go quickly
                    * (eg. convex polygons).  This code does NOT handle multiple contours,
                    * intersections, edge flags, and of course it does not generate
                    * an explicit mesh either.
                    */
                    if (RenderCache())
                    {
                        return;
                    }
                }

                EmptyCache(); /* could've used a label*/
            }

            /* Determine the polygon normal and project vertices onto the plane
            * of the polygon.
            */
            ProjectPolygon();

            /* __gl_computeInterior( this ) computes the planar arrangement specified
            * by the given contours, and further subdivides this arrangement
            * into regions.  Each region is marked "inside" if it belongs
            * to the polygon, according to the rule given by this.windingRule.
            * Each interior region is guaranteed to be monotone.
            */
            ActiveRegion.ComputeInterior(this);

            bool rc = true;

            /* If the user wants only the boundary contours, we throw away all edges
            * except those which separate the interior from the exterior.
            * Otherwise we tessellate all the regions marked "inside".
            */
            if (boundaryOnly)
            {
                rc = mesh.SetWindingNumber(1, true);
            }
            else
            {
                rc = mesh.TessellateInterior();
            }

            mesh.CheckMesh();

            if (callBegin != null || callEnd != null
                || callVertex != null || callEdgeFlag != null)
            {
                if (boundaryOnly)
                {
                    RenderBoundary(mesh); /* output boundary contours */
                }
                else
                {
                    RenderMesh(mesh); /* output strips and fans */
                }
            }
            if (callMesh != null)
            {
                /* Throw away the exterior faces, so that all faces are interior.
                * This way the user doesn't have to check the "inside" flag,
                * and we don't need to even reveal its existence.  It also leaves
                * the freedom for an implementation to not generate the exterior
                * faces in the first place.
                */
                mesh.DiscardExterior();
                callMesh(mesh); /* user wants the mesh itself */
                mesh = null;
                return;
            }
            mesh = null;
        }

        #region CodeFromRender

        /************************ Strips and Fans decomposition ******************/

        /* __gl_renderMesh( tess, mesh ) takes a mesh and breaks it into triangle
        * fans, strips, and separate triangles.  A substantial effort is made
        * to use as few rendering primitives as possible (ie. to make the fans
        * and strips as large as possible).
        *
        * The rendering output is provided as callbacks (see the api).
        */

        private const int SIGN_INCONSISTENT = 2;

        public void RenderMesh(Mesh mesh)
        {
            Face f;

            /* Make a list of separate triangles so we can render them all at once */
            lonelyTriList = null;

            for (f = mesh.faceHead.nextFace; f != mesh.faceHead; f = f.nextFace)
            {
                f.marked = false;
            }
            for (f = mesh.faceHead.nextFace; f != mesh.faceHead; f = f.nextFace)
            {
                /* We examine all faces in an arbitrary order.  Whenever we find
                * an unprocessed face F, we output a group of faces including F
                * whose size is maximum.
                */
                if (f.isInterior && !f.marked)
                {
                    RenderMaximumFaceGroup(f);
                    if (!f.marked)
                    {
                        throw new Exception();
                    }
                }
            }
            if (lonelyTriList != null)
            {
                RenderLonelyTriangles(lonelyTriList);
                lonelyTriList = null;
            }
        }


        private void RenderMaximumFaceGroup(Face fOrig)
        {
            /* We want to find the largest triangle fan or strip of unmarked faces
            * which includes the given face fOrig.  There are 3 possible fans
            * passing through fOrig (one centered at each vertex), and 3 possible
            * strips (one for each CCW permutation of the vertices).  Our strategy
            * is to try all of these, and take the primitive which uses the most
            * triangles (a greedy approach).
            */
            HalfEdge e = fOrig.halfEdgeThisIsLeftFaceOf;
            var max = new FaceCount(1, e, RenderTriangle);
            FaceCount newFace;

            max.size = 1;
            max.eStart = e;

            if (!EdgeCallBackSet)
            {
                newFace = MaximumFan(e);
                if (newFace.size > max.size)
                {
                    max = newFace;
                }
                newFace = MaximumFan(e.nextEdgeCCWAroundLeftFace);
                if (newFace.size > max.size)
                {
                    max = newFace;
                }
                newFace = MaximumFan(e.Lprev);
                if (newFace.size > max.size)
                {
                    max = newFace;
                }

                newFace = MaximumStrip(e);
                if (newFace.size > max.size)
                {
                    max = newFace;
                }
                newFace = MaximumStrip(e.nextEdgeCCWAroundLeftFace);
                if (newFace.size > max.size)
                {
                    max = newFace;
                }
                newFace = MaximumStrip(e.Lprev);
                if (newFace.size > max.size)
                {
                    max = newFace;
                }
            }

            max.CallRender(this, max.eStart, max.size);
        }

        private FaceCount MaximumFan(HalfEdge eOrig)
        {
            /* eOrig.Lface is the face we want to render.  We want to find the size
            * of a maximal fan around eOrig.Org.  To do this we just walk around
            * the origin vertex as far as possible in both directions.
            */
            var newFace = new FaceCount(0, null, RenderFan);
            Face trail = null;
            HalfEdge e;

            for (e = eOrig; !e.leftFace.Marked(); e = e.nextEdgeCCWAroundOrigin)
            {
                Face.AddToTrail(ref e.leftFace, ref trail);
                ++newFace.size;
            }
            for (e = eOrig; !e.rightFace.Marked(); e = e.Oprev)
            {
                Face f = e.rightFace;
                Face.AddToTrail(ref f, ref trail);
                e.rightFace = f;
                ++newFace.size;
            }
            newFace.eStart = e;

            Face.FreeTrail(ref trail);
            return newFace;
        }


        private bool IsEven(int n)
        {
            return (((n) & 1) == 0);
        }

        private FaceCount MaximumStrip(HalfEdge eOrig)
        {
            /* Here we are looking for a maximal strip that contains the vertices
            * eOrig.Org, eOrig.Dst, eOrig.Lnext.Dst (in that order or the
            * reverse, such that all triangles are oriented CCW).
            *
            * Again we walk forward and backward as far as possible.  However for
            * strips there is a twist: to get CCW orientations, there must be
            * an *even* number of triangles in the strip on one side of eOrig.
            * We walk the strip starting on a side with an even number of triangles;
            * if both side have an odd number, we are forced to shorten one side.
            */
            var newFace = new FaceCount(0, null, RenderStrip);
            int headSize = 0, tailSize = 0;
            Face trail = null;
            HalfEdge e, eTail, eHead;

            for (e = eOrig; !e.leftFace.Marked(); ++tailSize, e = e.nextEdgeCCWAroundOrigin)
            {
                Face.AddToTrail(ref e.leftFace, ref trail);
                ++tailSize;
                e = e.Dprev;
                if (e.leftFace.Marked()) break;
                Face.AddToTrail(ref e.leftFace, ref trail);
            }
            eTail = e;

            for (e = eOrig; !e.rightFace.Marked(); ++headSize, e = e.Dnext)
            {
                Face f = e.rightFace;
                Face.AddToTrail(ref f, ref trail);
                e.rightFace = f;
                ++headSize;
                e = e.Oprev;
                if (e.rightFace.Marked()) break;
                f = e.rightFace;
                Face.AddToTrail(ref f, ref trail);
                e.rightFace = f;
            }
            eHead = e;

            newFace.size = tailSize + headSize;
            if (IsEven(tailSize))
            {
                newFace.eStart = eTail.otherHalfOfThisEdge;
            }
            else if (IsEven(headSize))
            {
                newFace.eStart = eHead;
            }
            else
            {
                /* Both sides have odd length, we must shorten one of them.  In fact,
                * we must start from eHead to guarantee inclusion of eOrig.Lface.
                */
                --newFace.size;
                newFace.eStart = eHead.nextEdgeCCWAroundOrigin;
            }

            Face.FreeTrail(ref trail);
            return newFace;
        }


        private void RenderTriangle(Tesselator tess, HalfEdge e, int size)
        {
            /* Just add the triangle to a triangle list, so we can render all
            * the separate triangles at once.
            */
            if (size != 1)
            {
                throw new Exception();
            }
            Face.AddToTrail(ref e.leftFace, ref lonelyTriList);
        }


        private void RenderLonelyTriangles(Face f)
        {
            /* Now we render all the separate triangles which could not be
            * grouped into a triangle fan or strip.
            */
            HalfEdge e;
            bool newState = false;
            bool edgeState = false; /* force edge state output for first vertex */
            bool sentFirstEdge = false;

            CallBegin(TriangleListType.Triangles);

            for (; f != null; f = f.trail)
            {
                /* Loop once for each edge (there will always be 3 edges) */

                e = f.halfEdgeThisIsLeftFaceOf;
                do
                {
                    if (EdgeCallBackSet)
                    {
                        /* Set the "edge state" to TRUE just before we output the
                        * first vertex of each edge on the polygon boundary.
                        */
                        newState = !e.rightFace.isInterior;
                        if (edgeState != newState || !sentFirstEdge)
                        {
                            sentFirstEdge = true;
                            edgeState = newState;
                            CallEdegFlag(edgeState);
                        }
                    }

                    CallVertex(e.originVertex.clientIndex);

                    e = e.nextEdgeCCWAroundLeftFace;
                } while (e != f.halfEdgeThisIsLeftFaceOf);
            }

            CallEnd();
        }


        private static void RenderFan(Tesselator tess, HalfEdge e, int size)
        {
            /* Render as many CCW triangles as possible in a fan starting from
            * edge "e".  The fan *should* contain exactly "size" triangles
            * (otherwise we've goofed up somewhere).
            */
            tess.CallBegin(TriangleListType.TriangleFan);
            tess.CallVertex(e.originVertex.clientIndex);
            tess.CallVertex(e.directionVertex.clientIndex);

            while (!e.leftFace.Marked())
            {
                e.leftFace.marked = true;
                --size;
                e = e.nextEdgeCCWAroundOrigin;
                tess.CallVertex(e.directionVertex.clientIndex);
            }

            if (size != 0)
            {
                throw new Exception();
            }
            tess.CallEnd();
        }


        private static void RenderStrip(Tesselator tess, HalfEdge halfEdge, int size)
        {
            /* Render as many CCW triangles as possible in a strip starting from
            * edge "e".  The strip *should* contain exactly "size" triangles
            * (otherwise we've goofed up somewhere).
            */
            tess.CallBegin(TriangleListType.TriangleStrip);
            tess.CallVertex(halfEdge.originVertex.clientIndex);
            tess.CallVertex(halfEdge.directionVertex.clientIndex);

            while (!halfEdge.leftFace.Marked())
            {
                halfEdge.leftFace.marked = true;
                --size;
                halfEdge = halfEdge.Dprev;
                tess.CallVertex(halfEdge.originVertex.clientIndex);
                if (halfEdge.leftFace.Marked()) break;

                halfEdge.leftFace.marked = true;
                --size;
                halfEdge = halfEdge.nextEdgeCCWAroundOrigin;
                tess.CallVertex(halfEdge.directionVertex.clientIndex);
            }

            if (size != 0)
            {
                throw new Exception();
            }
            tess.CallEnd();
        }


        /************************ Boundary contour decomposition ******************/

        /* Takes a mesh, and outputs one
        * contour for each face marked "inside".  The rendering output is
        * provided as callbacks.
        */

        public void RenderBoundary(Mesh mesh)
        {
            for (Face curFace = mesh.faceHead.nextFace; curFace != mesh.faceHead; curFace = curFace.nextFace)
            {
                if (curFace.isInterior)
                {
                    CallBegin(TriangleListType.LineLoop);
                    HalfEdge curHalfEdge = curFace.halfEdgeThisIsLeftFaceOf;
                    do
                    {
                        CallVertex(curHalfEdge.originVertex.clientIndex);
                        curHalfEdge = curHalfEdge.nextEdgeCCWAroundLeftFace;
                    } while (curHalfEdge != curFace.halfEdgeThisIsLeftFaceOf);
                    CallEnd();
                }
            }
        }


        /************************ Quick-and-dirty decomposition ******************/

        private int ComputeNormal(double[] norm3)
            /*
        * Check that each triangle in the fan from v0 has a
        * consistent orientation with respect to norm3[].  If triangles are
        * consistently oriented CCW, return 1; if CW, return -1; if all triangles
        * are degenerate return 0; otherwise (no consistent orientation) return
        * SIGN_INCONSISTENT.
        */
        {
            VertexAndIndex[] vCache = simpleVertexCache;
            VertexAndIndex v0 = vCache[0];
            int vcIndex;
            double dot, xc, yc, xp, yp;
            var n = new double[3];
            int sign = 0;

            /* Find the polygon normal.  It is important to get a reasonable
            * normal even when the polygon is self-intersecting (eg. a bowtie).
            * Otherwise, the computed normal could be very tiny, but perpendicular
            * to the true plane of the polygon due to numerical noise.  Then all
            * the triangles would appear to be degenerate and we would incorrectly
            * decompose the polygon as a fan (or simply not render it at all).
            *
            * We use a sum-of-triangles normal algorithm rather than the more
            * efficient sum-of-trapezoids method (used in CheckOrientation()
            * in normal.c).  This lets us explicitly reverse the signed area
            * of some triangles to get a reasonable normal in the self-intersecting
            * case.
            */
            vcIndex = 1;
            xc = vCache[vcIndex].x - v0.x;
            yc = vCache[vcIndex].y - v0.y;
            while (++vcIndex < cacheCount)
            {
                xp = xc;
                yp = yc;
                xc = vCache[vcIndex].x - v0.x;
                yc = vCache[vcIndex].y - v0.y;

                /* Compute (vp - v0) cross (vc - v0) */
                n[0] = 0;
                n[1] = 0;
                n[2] = xp*yc - yp*xc;

                dot = n[0]*norm3[0] + n[1]*norm3[1] + n[2]*norm3[2];
                if (dot != 0)
                {
                    /* Check the new orientation for consistency with previous triangles */
                    if (dot > 0)
                    {
                        if (sign < 0)
                        {
                            return SIGN_INCONSISTENT;
                        }
                        sign = 1;
                    }
                    else
                    {
                        if (sign > 0)
                        {
                            return SIGN_INCONSISTENT;
                        }
                        sign = -1;
                    }
                }
            }

            return sign;
        }

        /* Takes a single contour and tries to render it
        * as a triangle fan.  This handles convex polygons, as well as some
        * non-convex polygons if we get lucky.
        *
        * Returns TRUE if the polygon was successfully rendered.  The rendering
        * output is provided as callbacks (see the api).
        */

        public bool RenderCache()
        {
            VertexAndIndex[] vCache = simpleVertexCache;
            VertexAndIndex v0 = vCache[0];
            var norm3 = new double[3];
            int sign;

            if (cacheCount < 3)
            {
                /* Degenerate contour -- no output */
                return true;
            }

            norm3[0] = 0;
            norm3[1] = 0;
            norm3[2] = 1;

            sign = ComputeNormal(norm3);
            if (sign == SIGN_INCONSISTENT)
            {
                // Fan triangles did not have a consistent orientation
                return false;
            }
            if (sign == 0)
            {
                // All triangles were degenerate
                return true;
            }

            /* Make sure we do the right thing for each winding rule */
            switch (windingRule)
            {
                case WindingRuleType.Odd:
                case WindingRuleType.NonZero:
                    break;
                case WindingRuleType.Positive:
                    if (sign < 0) return true;
                    break;
                case WindingRuleType.Negative:
                    if (sign > 0) return true;
                    break;
                case WindingRuleType.ABS_GEQ_Two:
                    return true;
            }

            CallBegin(BoundaryOnly
                          ? TriangleListType.LineLoop
                          : (cacheCount > 3)
                                ? TriangleListType.TriangleFan
                                : TriangleListType.Triangles);

            CallVertex(v0.vertexIndex);
            if (sign > 0)
            {
                for (int vcIndex = 1; vcIndex < cacheCount; ++vcIndex)
                {
                    CallVertex(vCache[vcIndex].vertexIndex);
                }
            }
            else
            {
                for (int vcIndex = cacheCount - 1; vcIndex > 0; --vcIndex)
                {
                    CallVertex(vCache[vcIndex].vertexIndex);
                }
            }
            CallEnd();
            return true;
        }

        private class FaceCount
        {
            #region Delegates

            public delegate void RenderDelegate(Tesselator tess, HalfEdge edge, int data);

            #endregion

            public HalfEdge eStart; /* edge where this primitive starts */
            public int size; /* number of triangles used */

            public FaceCount(int _size, HalfEdge _eStart, RenderDelegate _render)
            {
                size = _size;
                eStart = _eStart;
                render = _render;
            }

            private event RenderDelegate render;
            // routine to render this primitive

            public void CallRender(Tesselator tess, HalfEdge edge, int data)
            {
                render(tess, edge, data);
            }
        } ;

        #endregion

        #region Nested type: ProcessingState

        private enum ProcessingState
        {
            Dormant,
            InPolygon,
            InContour
        } ;

        #endregion

        #region Nested type: Vertex

        public struct Vertex
        {
            public double x;
            public double y;
        }

        #endregion

        #region Nested type: VertexAndIndex

        public struct VertexAndIndex
        {
            public int vertexIndex;
            public double x;
            public double y;
        }

        #endregion
    } ;
}