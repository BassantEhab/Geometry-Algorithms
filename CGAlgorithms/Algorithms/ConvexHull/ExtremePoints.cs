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
            if (points.Count == 1)
            {
                outPoints = points;
                return;
            }
            List<bool> flagPoints = new List<bool>(points.Count);
            for (int i = 0; i < points.Count; i++)
                flagPoints.Add(true);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (j == i)
                        continue;
                    if (points[i].X == points[j].X && points[i].Y == points[j].Y)
                    {
                        if (flagPoints[j])
                            flagPoints[i] = false;
                    }
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == i || k == j)
                            continue;
                        for (int l = 0; l < points.Count; l++)
                        {

                            if (l == k || l == j || l == i)
                                continue;

                            if (HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]) == Enums.PointInPolygon.Inside)
                                flagPoints[i] = false;

                            else if (HelperMethods.CheckTurn(new Line(points[j], points[k]), points[i]) == Enums.TurnType.Colinear && HelperMethods.IN_Between(new Line(points[j], points[k]), points[i]))
                                flagPoints[i] = false;
                            else if (HelperMethods.CheckTurn(new Line(points[k], points[l]), points[i]) == Enums.TurnType.Colinear && HelperMethods.IN_Between(new Line(points[k], points[l]), points[i]))
                                flagPoints[i] = false;
                            else if (HelperMethods.CheckTurn(new Line(points[l], points[j]), points[i]) == Enums.TurnType.Colinear && HelperMethods.IN_Between(new Line(points[l], points[j]), points[i]))
                                flagPoints[i] = false;
                        }
                    }
                }
            }
            for (int i = 0; i < flagPoints.Count; i++)
                if (flagPoints[i] == true)
                    outPoints.Add(points[i]);

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
