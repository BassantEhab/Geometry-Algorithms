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
            if (points.Count == 1)
            {
                outPoints = points;
                return;
            }
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    bool right = false, left = false, no = false;
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == i || k == j) 
                            continue;
                        if(HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Right)
                            right = true;
                        else if(HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Left)
                            left = true ;
                        if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]) == Enums.TurnType.Colinear && !HelperMethods.PointOnSegment(points[k], points[i], points[j]))
                            no = true;
                    }
                    if (no)
                        continue;
                    if (!right || !left)
                    {
                        outLines.Add(new Line(points[i], points[j]));
                        if (!outPoints.Contains(points[i])) 
                            outPoints.Add(points[i]);
                        if (!outPoints.Contains(points[j])) 
                            outPoints.Add(points[j]);
                    }
                }
            }
        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
