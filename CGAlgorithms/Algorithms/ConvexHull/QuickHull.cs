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
			double MIN = 999999999;
			double MAX = -99999999;
			int minIndx = 0;
			int maxIndx = 0;

			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].X < MIN)
				{
					MIN = points[i].X;
					minIndx = i;
				}
				if (points[i].X > MAX)
				{
					MAX = points[i].X;
					maxIndx = i;
				}
			}
			Point minp = points[minIndx];
			Point maxp = points[maxIndx];
			outPoints.Add(minp);
			outPoints.Add(maxp);

			List<Point> Points1 = new List<Point>();
			Line line1 = new Line(minp, maxp);
			for (int i = 0; i < points.Count; i++)
				if (CGUtilities.HelperMethods.CheckTurn(line1, points[i]) == CGUtilities.Enums.TurnType.Left)
					Points1.Add(points[i]);
			getHull(Points1, line1, ref outPoints);

			List<Point> Points2 = new List<Point>();
			Line line2 = new Line(maxp, minp);
			for (int i = 0; i < points.Count; i++)
				if (CGUtilities.HelperMethods.CheckTurn(line2, points[i]) == CGUtilities.Enums.TurnType.Left)
					Points2.Add(points[i]);
			getHull(Points2, line2, ref outPoints);

		}
		
		public List<Point> getHull(List<Point> Points, Line extremLine, ref List<Point> hull)
		{
			if (Points.Count == 0)
				return hull;

			Point b, c;
			List<KeyValuePair<Point, double>> final = new List<KeyValuePair<Point, double>>();

			for (int i = 0; i < Points.Count; i++)
			{
				b = new Point((extremLine.Start.X - Points[i].X), (extremLine.Start.Y - Points[i].Y));
				c = new Point((extremLine.End.X - Points[i].X), (extremLine.End.Y - Points[i].Y));
				double crossProduct = b.X * c.Y - b.Y * c.X;
				double mag = (Math.Sqrt(Math.Pow((extremLine.Start.X - extremLine.End.X), 2) + Math.Pow((extremLine.Start.Y - extremLine.End.Y), 2)));
				double hieght = crossProduct / mag;
				final.Add(new KeyValuePair<Point, double>(Points[i], hieght));
			}
			final.Sort((x, y) => x.Value.CompareTo(y.Value));
			Point maxHieght = final[final.Count - 1].Key;
			hull.Add(maxHieght);

			// devide points into 3: inside traingle, on left, on right 
			List<Point> left = new List<Point>();
			Line leftLine = new Line(extremLine.Start, maxHieght);
			for (int i = 0; i < Points.Count; i++)
				if (CGUtilities.HelperMethods.CheckTurn(leftLine, Points[i]) == CGUtilities.Enums.TurnType.Left)
					left.Add(Points[i]);
			getHull(left, leftLine, ref hull);

			List<Point> right = new List<Point>();
			Line rightLine = new Line(maxHieght, extremLine.End);
			for (int i = 0; i < Points.Count; i++)
				if (CGUtilities.HelperMethods.CheckTurn(rightLine, Points[i]) == CGUtilities.Enums.TurnType.Left)
					right.Add(Points[i]);
			getHull(right, rightLine, ref hull);

			return hull;
		}
		public override string ToString()
		{
			return "Convex Hull - Quick Hull";
		}
	}
}
