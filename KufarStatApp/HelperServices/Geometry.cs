namespace KufarStatApp.HelperServices
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
    public static class Geometry
    {
        /// <summary>
        /// Checks if two lines intersect.
        /// </summary>
        public static bool DoLineSegmentsIntersect(Point p1, Point p2, Point p3, Point p4)
        {
            // Check if the two line segments are parallel
            if ((p2.Y - p1.Y) * (p4.X - p3.X) - (p2.X - p1.X) * (p4.Y - p3.Y) == 0)
                return false;

            // Check if the two line segments intersect
            double t1 = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / ((p2.Y - p1.Y) * (p4.X - p3.X) - (p2.X - p1.X) * (p4.Y - p3.Y));
            double t2 = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));

            return t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1;
        }

        /// <summary>
        /// Checks of the point is inside the polygon.
        /// </summary>
        public static bool IsPointInsidePolygon(Point point, List<Point> polygonPoints)
        {
            var minX = polygonPoints.Min(p => p.X);
            var maxX = polygonPoints.Max(p => p.X);
            var minY = polygonPoints.Min(p => p.Y);
            var maxY = polygonPoints.Max(p => p.Y);

            if (point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY)
                return false;

            var inside = false;
            for (int i = 0, j = polygonPoints.Count - 1; i < polygonPoints.Count; j = i++)
                if (polygonPoints[i].Y > point.Y != polygonPoints[j].Y > point.Y &&
                    point.X < (polygonPoints[j].X - polygonPoints[i].X) * (point.Y - polygonPoints[i].Y) / (polygonPoints[j].Y - polygonPoints[i].Y) + polygonPoints[i].X)
                    inside = !inside;

            return inside;
        }

        /// <summary>
        /// Checks if the figure defined by points has atleast 3 point and has no intersections with it's own lines.
        /// </summary>
        public static bool IsValidFigure(List<Point> points)
        {
            if (points.Count < 3)
                return false;

            for (int i = 0; i < points.Count - 1; i++)
                for (int j = i + 2; j < points.Count - 1; j++)
                    if (DoLineSegmentsIntersect(points[i], points[i + 1], points[j], points[j + 1]))
                        return false;

            return true;
        }
    }
}
