using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
            {
                outPoints = points;
                return;
            }

            Point lowestPoint = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].Y < lowestPoint.Y || (points[i].Y == lowestPoint.Y && points[i].X < lowestPoint.X))
                {
                    lowestPoint = points[i];
                }
            }

            points.Sort((p1, p2) => PolarAngle(lowestPoint, p1).CompareTo(PolarAngle(lowestPoint, p2)));

            if (CheckTurn(points[0], points[1], points[2]) == Enums.TurnType.Colinear)
            {
                outPoints = points;
                return;
            }

            Stack<Point> stack = new Stack<Point>();
            stack.Push(points[0]);
            stack.Push(points[1]);

            for (int i = 2; i < points.Count; i++)
            {
                while (stack.Count >= 2 && CheckTurn(stack.ElementAt(1), stack.Peek(), points[i]) != Enums.TurnType.Left)
                {
                    stack.Pop();
                }

                stack.Push(points[i]);
            }

            outPoints = stack.ToList();
        }

        private double PolarAngle(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Atan2(dy, dx);
        }

        private Enums.TurnType CheckTurn(Point p1, Point p2, Point p3)
        {
            return HelperMethods.CheckTurn(p1.Vector(p2), p2.Vector(p3));
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}