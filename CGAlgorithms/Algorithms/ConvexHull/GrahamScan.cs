using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (points.Count < 3)
            {
                outPoints = points;
                return;
            }

            List<Point> newlistpoints = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                if (!newlistpoints.Contains(points[i]))
                    newlistpoints.Add(points[i]);
            }


            Point lowestPoint = newlistpoints.OrderBy(p => p.Y).ThenBy(p => p.X).First();

            List<Point> sortedPoints = newlistpoints.OrderBy(p => Math.Atan2(p.Y - lowestPoint.Y, p.X - lowestPoint.X)).ToList();

            Stack<Point> convexHull = new Stack<Point>();
            convexHull.Push(sortedPoints[0]);
            convexHull.Push(sortedPoints[1]);
            convexHull.Push(sortedPoints[2]);

            for (int i = 3; i < sortedPoints.Count; i++)
            {
                while (HelperMethods.CheckTurn(new Line(convexHull.ElementAt(1), convexHull.Peek()), sortedPoints[i]) != Enums.TurnType.Left)
                {
                    convexHull.Pop();
                }
                convexHull.Push(sortedPoints[i]);
            }

            Point[] hullArray = convexHull.ToArray();
            int le = hullArray.Length;


            if (HelperMethods.CheckTurn(new Line(hullArray[le-1], hullArray[le-2]), hullArray[0]) == Enums.TurnType.Colinear)
            {
                convexHull.Pop();

            }


            outPoints = convexHull.ToList();

            for (int i = 0; i < outPoints.Count; i++)
            {

                Console.WriteLine("X : " + outPoints[i].X + " Y : " + outPoints[i].Y);
            }

        }


        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }

    }
}
