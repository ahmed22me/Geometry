using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
            {
                outPoints = new List<Point>(points);
                return;

            }
            Point leftmost = points.OrderBy(p => p.X).First();
            Point rightmost = points.OrderBy(p => p.X).Last();
            outPoints.Add(leftmost);
            outPoints.Add(rightmost);
            List<Point> pointsAboveLine = points.Where(p => CheckTurn2(leftmost, rightmost, p) == Enums.TurnType.Left).ToList();
            List<Point> pointsBelowLine = points.Where(p => CheckTurn2(leftmost, rightmost, p) == Enums.TurnType.Right).ToList();
            FindHullRec(leftmost, rightmost, pointsAboveLine, outPoints);
            FindHullRec(rightmost, leftmost, pointsBelowLine, outPoints);

            for (int i = 0; i < outPoints.Count - 1; i++)
            {
                outLines.Add(new Line(outPoints[i], outPoints[i + 1]));
            }

            outLines.Add(new Line(outPoints[outPoints.Count - 1], outPoints[0]));
        }
        public static Enums.TurnType CheckTurn2(Point p1, Point p2, Point p3)
        {
            int crossProduct = (int)Math.Round((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X));

            if (crossProduct == 0)
                return Enums.TurnType.Colinear;
            else if (crossProduct > 0)
                return Enums.TurnType.Left;
            else
                return Enums.TurnType.Right;
        }
        static void FindHullRec(Point p1, Point p2, List<Point> points, List<Point> convexHull)
        {
            if (points.Count == 0)
                return;

            Point farthestPoint = FindFarthestPoint(p1, p2, points);

            convexHull.Insert(convexHull.IndexOf(p2), farthestPoint);

            List<Point> pointsLeftOfLine = points.Where(p => CheckTurn2(p1, farthestPoint, p) == Enums.TurnType.Left).ToList();
            List<Point> pointsRightOfLine = points.Where(p => CheckTurn2(farthestPoint, p2, p) == Enums.TurnType.Left).ToList();

            FindHullRec(p1, farthestPoint, pointsLeftOfLine, convexHull);
            FindHullRec(farthestPoint, p2, pointsRightOfLine, convexHull);
        }

        static Point FindFarthestPoint(Point p1, Point p2, List<Point> points)
        {
            double maxDistance = 0;
            Point farthestPoint = null;

            foreach (var point in points)
            {
                double distance = PointToLineDistance(p1, p2, point);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPoint = point;
                }
            }

            return farthestPoint;
        }
        public static double PointToLineDistance(Point lineStart, Point lineEnd, Point point)
        {
            return Math.Abs((lineEnd.Y - lineStart.Y) * point.X - (lineEnd.X - lineStart.X) * point.Y +
                            lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X) /
                   Math.Sqrt(Math.Pow(lineEnd.Y - lineStart.Y, 2) + Math.Pow(lineEnd.X - lineStart.X, 2));
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
