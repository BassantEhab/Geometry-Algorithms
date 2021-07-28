using System;
using CGUtilities.DataStructures;
using CGUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    public class Event : IComparable
    {
        public double X;
        public double Y;
        public int event_type;
        public int indexLine1; 
        public int indexLine2;

        public Event(double x, double y, int type, int indexLine1, int indexLine2)
        {
            this.X = x; 
            this.Y = y;
            this.event_type = type;
            this.indexLine1 = indexLine1;
            this.indexLine2 = indexLine2;
        }

        public int CompareTo(object obj)
        {
            Event evnt = obj as Event;
            if (evnt.X > X)
                return -1;
            else if (evnt.X < X)
                return 1;
            else
            {
                if (evnt.Y > Y)
                    return -1;
                else if (evnt.Y < Y)
                    return 1;
                else
                    return 0;
            }
        }
    }
    public class line : IComparable
    {
        public int index;
        public Point intsection;
        public line(int indx, Point intersection)
        {
            this.index = indx;
            this.intsection = intersection;
        }
        public line() { }

        public int CompareTo(object obj)
        {
            line intersectionLine = obj as line;

            if (intersectionLine.intsection.Y < intsection.Y)
                return -1;
            else if (intersectionLine.intsection.Y > intsection.Y)
                return 1;
            else
                return 0;
        }
    }
    class SweepLine : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            Line sweepLine = new Line(new Point(0, 0), new Point(0, 0));
            OrderedSet<line> strLineSet = new OrderedSet<line>();
            OrderedSet<Event> events = new OrderedSet<Event>();
            Dictionary<int, Point> intersections = new Dictionary<int, Point>();

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Start.X < lines[i].End.X)
                {
                    events.Add(new Event(lines[i].Start.X, lines[i].Start.Y, 0, i, i));
                    events.Add(new Event(lines[i].End.X, lines[i].End.Y, 1, i, i));
                }
            }

            while (events.Count > 0)
            {
                Event currentEvent = events.RemoveFirst();
                sweepLine.Start = new Point(currentEvent.X, 1000);
                sweepLine.End = new Point(currentEvent.X, -1000);

                if (currentEvent.event_type == 0)
                {
                    line currentLine = new line(currentEvent.indexLine1, lines[currentEvent.indexLine1].Start);
                    strLineSet.Add(currentLine);
                    KeyValuePair<line, line> nextAndPrev = strLineSet.DirectUpperAndLower(currentLine);
                    line next = nextAndPrev.Key;
                    line prev = nextAndPrev.Value;

                    if (prev != null)
                    {
                        Point currentInter = getIntersection(lines[currentLine.index], lines[prev.index]);
                        if (currentInter != null && currentInter.X > sweepLine.Start.X)
                        {
                            events.Add(new Event(currentInter.X, currentInter.Y, 2, currentLine.index, prev.index));
                            outPoints.Add(currentInter);
                        }
                    }

                    if (next != null)
                    {
                        Point currentInter = getIntersection(lines[currentLine.index], lines[next.index]);
                        if (currentInter != null && currentInter.X > sweepLine.Start.X)
                        {
                            events.Add(new Event(currentInter.X, currentInter.Y, 2, currentLine.index, next.index));
                            outPoints.Add(currentInter);
                        }
                    }
                }

                else if (currentEvent.event_type == 1)
                { 
                    Point sweepInterPoint = lines[currentEvent.indexLine1].Start;
                    if (intersections.ContainsKey(currentEvent.indexLine1))
                        sweepInterPoint = intersections[currentEvent.indexLine1];

                    line item = new line();
                    if (strLineSet.TryGetItem(new line(currentEvent.indexLine1, sweepInterPoint), out item))
                    {
                        KeyValuePair<line, line> nextAndPrev = strLineSet.DirectUpperAndLower(item);
                    line next = nextAndPrev.Key;
                    line prev = nextAndPrev.Value;
                        strLineSet.Remove(item); 
                        if (prev != null && next != null)
                        {
                            Point currentInter = getIntersection(lines[prev.index], lines[next.index]);
                            if (currentInter != null && currentInter.X > sweepLine.Start.X)
                            {
                                events.Add(new Event(currentInter.X, currentInter.Y, 2, prev.index, next.index));
                                outPoints.Add(currentInter);
                            }
                        }
                    }
                }

                else if (currentEvent.event_type == 2)
                {
                    Point sweep1 = lines[currentEvent.indexLine1].Start;
                    Point sweep2 = lines[currentEvent.indexLine2].Start;

                    if (intersections.ContainsKey(currentEvent.indexLine1))
                        sweep1 = intersections[currentEvent.indexLine1];
                    if (intersections.ContainsKey(currentEvent.indexLine2))
                        sweep2 = intersections[currentEvent.indexLine2];

                    line line1 = new line();
                    line line2 = new line();
                    if (strLineSet.TryGetItem(new line(currentEvent.indexLine1, sweep1), out line1) && strLineSet.TryGetItem(new line(currentEvent.indexLine2, sweep2), out line2))
                    {
                        strLineSet.Remove(line1);
                        strLineSet.Remove(line2);
                        sweepLine.Start.X = sweepLine.Start.X + 0.1;
                        sweepLine.End.X = sweepLine.End.X + 0.1;

                        sweep1 = getIntersection(lines[line1.index], sweepLine);
                        sweep2 = getIntersection(lines[line2.index], sweepLine);

                        line1.intsection = sweep1;
                        line2.intsection = sweep2;

                        if (sweep1 != null)
                            strLineSet.Add(line1);
                        if (sweep2 != null)
                            strLineSet.Add(line2);

                        intersections[line1.index] = line1.intsection;
                        intersections[line2.index] = line2.intsection;
                    }

                    KeyValuePair<line, line> nextAndPrev1 = strLineSet.DirectUpperAndLower(line1);
                    line next1 = nextAndPrev1.Key;
                    line prev1 = nextAndPrev1.Value;
                    KeyValuePair<line, line> nextAndPrev2 = strLineSet.DirectUpperAndLower(line2);
                    line next2 = nextAndPrev2.Key;
                    line prev2 = nextAndPrev2.Value;


                    if (prev1 != null)
                    {
                        Point currentInter = getIntersection(lines[line1.index], lines[prev1.index]);
                        if (currentInter != null && currentInter.X > sweepLine.Start.X)
                        {
                            events.Add(new Event(currentInter.X, currentInter.Y, 2, line1.index, prev1.index));
                            outPoints.Add(currentInter);
                        }
                    }

                    if (next1 != null)
                    {
                        Point currentInter = getIntersection(lines[line1.index], lines[next1.index]);
                        if (currentInter != null && currentInter.X > sweepLine.Start.X)
                        {
                            events.Add(new Event(currentInter.X, currentInter.Y, 2, line1.index, next1.index));
                            outPoints.Add(currentInter);
                        }
                    }

                    if (prev2 != null)
                    {
                        Point currentInter = getIntersection(lines[line2.index], lines[prev2.index]);
                        if (currentInter != null && currentInter.X > sweepLine.Start.X)
                        {
                            events.Add(new Event(currentInter.X, currentInter.Y, 2, line2.index, prev2.index));
                            outPoints.Add(currentInter);
                        }
                    }

                    if (next2 != null)
                    {
                        Point currentInter = getIntersection(lines[line2.index], lines[next2.index]);
                        if (currentInter != null && currentInter.X > sweepLine.Start.X)
                        {
                            events.Add(new Event(currentInter.X, currentInter.Y, 2, line2.index, next2.index));
                            outPoints.Add(currentInter);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {

            return "Sweep Line";
        }
        public Point getIntersection(Line Line1, Line line2)
        {
            bool isintersection = true;
            if (HelperMethods.CheckTurn(Line1, line2.Start) != HelperMethods.CheckTurn(Line1, line2.End))
            {
                if (HelperMethods.CheckTurn(line2, Line1.Start) != HelperMethods.CheckTurn(line2, Line1.End))
                    isintersection = false;
            }
            if (isintersection)
                return null;

            double deffX1 = Line1.End.X - Line1.Start.X;
            double deffy1 = Line1.End.Y - Line1.Start.Y;
            double deffX2 = line2.End.X - line2.Start.X;
            double deffy2 = line2.End.Y - line2.Start.Y;
            double deffy = Line1.Start.Y - line2.Start.Y;
            double deffx = Line1.Start.X - line2.Start.X;

            double t = (deffX2 * (deffy) - deffy2 * (deffx)) / (-deffX2 * deffy1 + deffX1 * deffy2);

            return new Point(Line1.Start.X + (t * deffX1), Line1.Start.Y + (t * deffy1));
        }
    }
}
