using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
	public class GrahamScan : Algorithm
	{
		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			double MIN = 9999999;
			int index = 0;
			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].Y < MIN)
				{
					MIN = points[i].Y;
					index = i;
				}
			}
			Point minY = points[index];
			Point intiPoint = new Point(minY.X + 1, minY.Y);
			Line Horizontal_Line = new Line(minY, intiPoint);
			points.Remove(minY);

			List<KeyValuePair<Point, double>> Sorted_Points = new List<KeyValuePair<Point, double>>();
			double crossProduct, dotProduct, radAngel, degAngel;
			Point p = new Point((Horizontal_Line.End.X - Horizontal_Line.Start.X), (Horizontal_Line.End.Y - Horizontal_Line.Start.Y));
			for (int i = 0; i < points.Count; i++) 
			{
				Point tmp = new Point((points[i].X - Horizontal_Line.Start.X), (points[i].Y - Horizontal_Line.Start.Y));
				crossProduct = CGUtilities.HelperMethods.CrossProduct(p, tmp);
				dotProduct = CGUtilities.HelperMethods.DotProduct(p, tmp);
				radAngel = Math.Atan2(dotProduct, crossProduct);
				degAngel = (180 / Math.PI)*(radAngel);
				Sorted_Points.Add(new KeyValuePair<Point, double>(points[i], degAngel));
			}
			Sorted_Points.Sort((x, y) => x.Value.CompareTo(y.Value));
			Sorted_Points.Add(new KeyValuePair<Point, double>(minY, 0));

			Stack<Point> hull = new Stack<Point>();
			hull.Push(minY);
			hull.Push(Sorted_Points[0].Key);
			Point top, preTop;

			for (int i = 1; i < Sorted_Points.Count; i++)
			{
				top = hull.Pop();
				preTop = hull.Pop();
				hull.Push(preTop);
				hull.Push(top);
				Line segment = new Line(top, preTop);
				while (hull.Count > 2 && CGUtilities.HelperMethods.CheckTurn(segment, Sorted_Points[i].Key) != CGUtilities.Enums.TurnType.Left)
				{
					hull.Pop();
					top = hull.Pop();
					preTop = hull.Pop();
					hull.Push(preTop);
					hull.Push(top);
					segment = new Line(top, preTop);
				}
				hull.Push(Sorted_Points[i].Key);
			}
			while (hull.Count > 0)
				outPoints.Add(hull.Pop());
			outPoints.RemoveAt(outPoints.Count - 1);
		}
		public override string ToString()
		{
			return "Convex Hull - Graham Scan";
		}
		
	}
}

