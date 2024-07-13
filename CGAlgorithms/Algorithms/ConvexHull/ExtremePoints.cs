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

            if (points.Count <= 3)
            {
                outPoints = new List<Point>(points);
                return;

            }

            List<Point> newlistpoints = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                if (!newlistpoints.Contains(points[i]))
                    newlistpoints.Add(points[i]);
            }

            //outPoints = new List<Point>();

            foreach (var pi in newlistpoints)
            {
                bool isExtreme = true;

                foreach (var pj in newlistpoints)
                {
                    

                    foreach (var pk in newlistpoints)
                    {
                        

                        foreach (var pl in newlistpoints)
                        {

                            if (pi != pj && pi != pk && pi != pl) {

                                if (HelperMethods.PointInTriangle(pi, pj, pk, pl) == Enums.PointInPolygon.Inside || HelperMethods.PointInTriangle(pi, pj, pk, pl) == Enums.PointInPolygon.OnEdge)
                                {
                                    isExtreme = false;
                                    break;
                                }

                            }

                            
                        }

                        if (!isExtreme)
                            break;
                    }

                    if (!isExtreme)
                        break;
                }
                
                if (isExtreme)
                    outPoints.Add(pi);
            }

            for(int i =0; i < outPoints.Count; i++)
            {

                Console.WriteLine("X : " + outPoints[i].X + " Y : " + outPoints[i].Y);
            }
        }

        

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }




    }
}
