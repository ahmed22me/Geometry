using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{

    public enum VertexType
    {
        Convex,
        Concave
    }

    class SubtractingEars : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            ComputeTriangles(points);

        }

        public override string ToString()
        {
            return "Subtracting Ears";
        }

        private List<short> ComputeTriangles(List<CGUtilities.Point> points)
        {
            List<short> indices = new List<short>();
            List<VertexType> vertexTypes = new List<VertexType>();
            List<short> triangles = new List<short>();

            int vertexCount = points.Count;

            for (var i = 0; i < vertexCount; i++)
            {
                indices.Add((short)i);
            }

            if (AreVerticesClockwise(points))
            {
                // vertices are already in clockwise order
                // leave indices as they are
                
            }
            else
            {
                // reverse indices
                indices.Reverse();
            }

            vertexTypes.Capacity = vertexCount;
            for (int i = 0, n = vertexCount; i < n; ++i)
                vertexTypes.Add(ClassifyVertex(i, indices, points));

            // A polygon with n vertices has a triangulation of n-2 triangles.
            triangles.Capacity = Math.Max(0, vertexCount - 2) * 3;

            while (vertexCount > 3)
            {
                int earTipIndex = FindEarTip(vertexTypes,points);
                CutEarTip(earTipIndex, indices, vertexTypes, ref vertexCount);

                // The type of the two vertices adjacent to the clipped vertex may have changed.
                int previousIndex = PreviousIndex(earTipIndex, vertexCount);
                int nextIndex = earTipIndex == vertexCount ? 0 : earTipIndex;
                vertexTypes[previousIndex] = ClassifyVertex(previousIndex, indices, points);
                vertexTypes[nextIndex] = ClassifyVertex(nextIndex, indices, points);
            }

            if (vertexCount == 3)
            {
                triangles.Add(indices[0]);
                triangles.Add(indices[1]);
                triangles.Add(indices[2]);
            }

            return triangles;
        }

        private VertexType ClassifyVertex(int index, List<short> indices, List<CGUtilities.Point> points)
        {
            int previousIndex = PreviousIndex(index, indices.Count);
            int nextIndex = NextIndex(index, indices.Count);
            int p1 = indices[previousIndex];
            int p2 = indices[index];
            int p3 = indices[nextIndex];

            return ComputeSpannedAreaSign(points[p1].X, points[p1].Y, points[p2].X, points[p2].Y, points[p3].X, points[p3].Y);
        }

        private int FindEarTip(List<VertexType> vertexTypes, List<CGUtilities.Point> points)
        {
            for (int i = 0; i < vertexTypes.Count; i++)
            {
                if (IsEarTip(i, vertexTypes,points))
                {
                    return i;
                }
            }

            // if no vertex is an ear tip, we are dealing with a degenerate polygon.
            // Return a convex or tangential vertex if one exists.
            for (int i = 0; i < vertexTypes.Count; i++)
            {
                if (vertexTypes[i] != VertexType.Concave)
                {
                    return i;
                }
            }

            // If all vertices are concave, just return the first one.
            return 0;
        }

        private bool IsEarTip(int earTipIndex, List<VertexType> vertexTypes, List<CGUtilities.Point> points)
        {
            if (vertexTypes[earTipIndex] == VertexType.Concave)
            {
                return false;
            }

            int previousIndex = PreviousIndex(earTipIndex, vertexTypes.Count);
            int nextIndex = NextIndex(earTipIndex, vertexTypes.Count);
            int p1 = previousIndex;
            int p2 = earTipIndex;
            int p3 = nextIndex;

            // Check if any point is inside the triangle formed by previous, current, and next vertices.
            // Only consider vertices that are not part of this triangle.
            for (int i = NextIndex(nextIndex, vertexTypes.Count); i != previousIndex; i = NextIndex(i, vertexTypes.Count))
            {
                // Concave vertices can obviously be inside the candidate ear, but so can tangential vertices
                // if they coincide with one of the triangle's vertices.
                if (vertexTypes[i] != VertexType.Convex)
                {
                    int v = i;
                    // Because the polygon has clockwise winding order, the area sign will be positive if the point is strictly inside.
                    // It will be 0 on the edge, which we want to include as well.
                    // note: check the edge defined by p1->p3 first since this fails _far_ more than the other 2 checks.
                    if (ComputeSpannedAreaSign(points[p3].X, points[p3].Y, points[p1].X, points[p1].Y, points[v].X, points[v].Y) >= 0
                        && ComputeSpannedAreaSign(points[p1].X, points[p1].Y, points[p2].X, points[p2].Y, points[v].X, points[v].Y) >= 0
                        && ComputeSpannedAreaSign(points[p2].X, points[p2].Y, points[p3].X, points[p3].Y, points[v].X, points[v].Y) >= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void CutEarTip(int earTipIndex, List<short> indices, List<VertexType> vertexTypes, ref int vertexCount)
        {
            int previousIndex = PreviousIndex(earTipIndex, vertexCount);
            int nextIndex = NextIndex(earTipIndex, vertexCount);

            indices.RemoveAt(earTipIndex);
            vertexTypes.RemoveAt(earTipIndex);
            vertexCount--;

            indices.Add(indices[previousIndex]);
            indices.Add(indices[nextIndex]);
        }

        private int PreviousIndex(int index, int vertexCount)
        {
            return (index == 0 ? vertexCount : index) - 1;
        }

        private int NextIndex(int index, int vertexCount)
        {
            return (index + 1) % vertexCount;
        }

        private bool AreVerticesClockwise(List<CGUtilities.Point> points)
        {
            if (points.Count <= 2) return false;
            double area = 0;
            for (int i = 0, n = points.Count - 3; i < n; i++)
            {
                area += points[i].X * (points[i + 2].Y - points[i + 1].Y);
            }
            area += points[points.Count - 2].X * (points[points.Count - 1].Y - points[points.Count - 3].Y);
            area += points[points.Count - 1].X * (points[points.Count - 3].Y - points[points.Count - 2].Y);
            return area < 0;
        }

        private VertexType ComputeSpannedAreaSign(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y)
        {
            double area = p1x * (p3y - p2y);
            area += p2x * (p1y - p3y);
            area += p3x * (p2y - p1y);
            return (VertexType)Math.Sign(area);
        }


    }
}
