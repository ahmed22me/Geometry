using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
                throw new ArgumentException("Convex hull requires at least 3 points.");

            points = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

            List<Point> upperHull = new List<Point>();
            List<Point> lowerHull = new List<Point>();

            foreach (var point in points)
            {
                while (upperHull.Count >= 2 && IsCounterClockwise(upperHull[upperHull.Count - 2], upperHull[upperHull.Count - 1], point))
                    upperHull.RemoveAt(upperHull.Count - 1);

                while (lowerHull.Count >= 2 && IsCounterClockwise(lowerHull[lowerHull.Count - 2], lowerHull[lowerHull.Count - 1], point))
                    lowerHull.RemoveAt(lowerHull.Count - 1);

                upperHull.Add(point);
                lowerHull.Add(point);
            }

            upperHull.AddRange(lowerHull.GetRange(1, lowerHull.Count - 2));

            outPoints = upperHull;
        }

        private static bool IsCounterClockwise(Point a, Point b, Point c)
        {
            double crossProduct = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
            return crossProduct < 0;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
