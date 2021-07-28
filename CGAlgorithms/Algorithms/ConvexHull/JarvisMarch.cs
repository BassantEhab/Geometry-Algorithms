using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        { 
            points.Sort(HelperMethods.sortOnY);
            int i = 0; 
            List<Point> outputList = new List<Point>(); 
            outputList.Add(points[0]); 
            outPoints.Add(points[0]);
            Point intiPoint = new Point(outputList[i].X + 1, points[i].Y);

            // get angle between the intiLine and all lines 
            do
            { 
                Point minAngPoint = null;
                double MinDegree;
                if (i == 0)
                    MinDegree = 999999999;
                else
                    MinDegree = -99999999;
                for (int j = 0; j < points.Count; j++)
                {
                    if (points[j] != outputList[i])
                    {
                        Point testedPoint;
                        if (i == 0)
                            testedPoint = HelperMethods.getVector(outputList[i], intiPoint);
                        else
                        {
                            if (intiPoint == outputList[i - 1])
                                continue;
                            testedPoint = HelperMethods.getVector(intiPoint, outputList[i - 1]);
                        }
                        Point crrPoint = HelperMethods.getVector(outputList[i], points[j]);
                        double dotProdct = HelperMethods.DotProduct(testedPoint, crrPoint);
                        double Radian = HelperMethods.magnitude(testedPoint, crrPoint);
                        double Degree = Math.Acos(dotProdct / Radian) * (180 / Math.PI);
                        if (Degree < 0)
                            Degree += 360;
                        //if i = 0 get the min else get the max 
                        if (i == 0)
                        {
                            if (Degree < MinDegree || (Degree == MinDegree && HelperMethods.GetDistance(outputList[i], points[j]) > HelperMethods.GetDistance(outputList[i], minAngPoint)))
                            {
                                MinDegree = Degree;
                                minAngPoint = points[j];
                            }
                        }
                        else
                        {
                            if (Degree > MinDegree || (Degree == MinDegree && minAngPoint != null && HelperMethods.GetDistance(outputList[i], points[j]) > HelperMethods.GetDistance(outputList[i], minAngPoint)))
                            {
                                MinDegree = Degree;
                                minAngPoint = points[j];
                            }
                        }
                    }
                }
                bool visited = false;
                if (minAngPoint != null)
                {
                    intiPoint = minAngPoint;
                    if (minAngPoint != points[0])
                    {
                        foreach (Point p in outputList)
                            if (p == minAngPoint)
                            {
                                visited = true;
                                break;
                            }

                        if (!visited)
                        {
                            outputList.Add(minAngPoint);
                            outPoints.Add(minAngPoint);
                            i++;
                        }
                    }
                    else
                        break;
                }
            }
            while (outputList[outputList.Count - 1] != points[0]);

            for (int j = 0; j < outputList.Count - 1; j++)
                outLines.Add(new Line(outputList[j], outputList[j + 1]));
            if (outputList.Count >= 2)
                outLines.Add(new Line(outputList[outputList.Count - 1], outputList[0]));
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
