using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
            {
                outPoints = new List<Point>(points);
                return;

            }
            // Sort points based on polar angle with the lowest point as the reference
            Point lowest = points.OrderBy(p => p.Y).ThenBy(p => p.X).First();
            points.Sort((p1, p2) =>
            {
                int angle1 = Math.Atan2(p1.Y - lowest.Y, p1.X - lowest.X).CompareTo(Math.Atan2(p2.Y - lowest.Y, p2.X - lowest.X));
                if (angle1 == 0)
                    return p1.X.CompareTo(p2.X);
                return angle1;
            });

            //outLines= new List<Line>();
            //outPoints = new List<Point>();

            outPoints.Add(points[0]);
            outPoints.Add(points[1]);

            for (int i = 2; i < points.Count; i++)
            {
                Point top = outPoints[outPoints.Count - 1];
                while (CheckTurn2(outPoints[outPoints.Count - 2], top, points[i]) != Enums.TurnType.Left)
                {
                    outPoints.RemoveAt(outPoints.Count - 1);
                    top = outPoints[outPoints.Count - 1];
                }
                outPoints.Add(points[i]);
            }

            for (int i = 0; i < outPoints.Count - 1; i++)
            {
                outLines.Add(new Line(outPoints[i], outPoints[i + 1]));
            }

            outLines.Add(new Line(outPoints[outPoints.Count - 1], outPoints[0]));



        }


        private static Enums.TurnType CheckTurn2(Point p1, Point p2, Point p3)
        {
            int crossProduct = (int)Math.Round((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X));

            if (crossProduct == 0)
                return Enums.TurnType.Colinear;
            else if (crossProduct > 0)
                return Enums.TurnType.Left;
            else
                return Enums.TurnType.Right;
        }


        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
