using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count == 1)
            {
                outPoints = points;
                return;
            }

            if (points.Count == 2)
            {
                outPoints = points;
                return;
            }
            if (points.Count < 3)
                return;

            outPoints.Clear();
            outLines.Clear();
            outPolygons.Clear();

            Point leftmost = points[0];
            Point rightmost = points[0];
            foreach (Point point in points)
            {
                if (point.X < leftmost.X)
                    leftmost = point;
                if (point.X > rightmost.X)
                    rightmost = point;
            }

            outPoints.Add(leftmost);
            outPoints.Add(rightmost);

            List<Point> abovePoints = new List<Point>();
            List<Point> belowPoints = new List<Point>();
            foreach (Point point in points)
            {
                if (HelperMethods.CheckTurn(leftmost.Vector(rightmost), leftmost.Vector(point)) == Enums.TurnType.Left)
                    abovePoints.Add(point);
                else if (HelperMethods.CheckTurn(leftmost.Vector(rightmost), leftmost.Vector(point)) == Enums.TurnType.Right)
                    belowPoints.Add(point);
            }

            QuickHullRecursive(leftmost, rightmost, abovePoints, outPoints);
            QuickHullRecursive(rightmost, leftmost, belowPoints, outPoints);
        }

        private void QuickHullRecursive(Point start, Point end, List<Point> points, List<Point> convexHull)
        {
            if (points.Count == 0)
                return;

            int farthestIndex = -1;
            double maxDistance = 0;

            for (int i = 0; i < points.Count; i++)
            {
                double distance = Math.Abs(HelperMethods.CrossProduct(start.Vector(end), start.Vector(points[i])));
                if (distance > maxDistance)
                {
                    farthestIndex = i;
                    maxDistance = distance;
                }
            }

            if (farthestIndex == -1)
                return;

            Point farthestPoint = points[farthestIndex];
            convexHull.Add(farthestPoint);

            List<Point> leftPoints = new List<Point>();
            List<Point> rightPoints = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if (HelperMethods.CheckTurn(start.Vector(farthestPoint), start.Vector(points[i])) == Enums.TurnType.Left)
                    leftPoints.Add(points[i]);
                else if (HelperMethods.CheckTurn(farthestPoint.Vector(end), farthestPoint.Vector(points[i])) == Enums.TurnType.Left)
                    rightPoints.Add(points[i]);
            }

            QuickHullRecursive(start, farthestPoint, leftPoints, convexHull);
            QuickHullRecursive(farthestPoint, end, rightPoints, convexHull);
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
