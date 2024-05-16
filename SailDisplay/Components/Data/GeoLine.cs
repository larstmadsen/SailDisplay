using System.Drawing;

namespace SailDisplay.Components.Data
{
    public class GeoLine
    {
        public GeoLine(GeoCordinate g1, GeoCordinate g2)
        {
            G1 = g1;
            G2 = g2;
        }
        public GeoLine(GeoCordinate g1, double angel)
        {
            G1 = g1;
            G2 = new GeoCordinate(g1.Latitude+Math.Cos(angel * Math.PI / 180.0), g1.Longitude+Math.Sin(angel * Math.PI / 180.0));
        }
        public GeoCordinate G1 { get; private set; }
        public GeoCordinate G2 { get; private set; }

        public double? a
        {
            get 
            {
                if(G1.Longitude == G2.Longitude)
                    return null;

                return (G1.Latitude - G2.Latitude) / (G1.Longitude - G2.Longitude);
            }
        }
        public double? b
        {
            get
            {
                if(a == null)
                    return null;

                return G1.Latitude - a * G1.Longitude;
            }
        }

        public GeoCordinate CrossingPoint(GeoLine gl)
        {
            return FindIntersection(gl);
            /*
            if(a == null || gl.a == null || a == gl.a)
                return null;

            var x = (gl.b - b) / (a - gl.a);
            var y = a * x + b;

            return new GeoCordinate((double)y, (double)x);
            */
        }

        //  Returns Point of intersection if do intersect otherwise default Point (null)
        public GeoCordinate FindIntersection(GeoLine gl, double tolerance = 0.001)
        {
            double x1 = gl.G1.X, y1 = gl.G1.Y;
            double x2 = gl.G2.X, y2 = gl.G2.Y;

            double x3 = G1.X, y3 = G1.Y;
            double x4 = G2.X, y4 = G2.Y;

                // equations of the form x=c (two vertical lines) with overlapping
                if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
                {
                    throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
                }

                //equations of the form y=c (two horizontal lines) with overlapping
                if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
                {
                    throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
                }

                //equations of the form x=c (two vertical parallel lines)
                if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
                {
                    //return default (no intersection)
                    return null;
                }

                //equations of the form y=c (two horizontal parallel lines)
                if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
                {
                    //return default (no intersection)
                    return null;
                }

                //general equation of line is y = mx + c where m is the slope
                //assume equation of line 1 as y1 = m1x1 + c1 
                //=> -m1x1 + y1 = c1 ----(1)
                //assume equation of line 2 as y2 = m2x2 + c2
                //=> -m2x2 + y2 = c2 -----(2)
                //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
                //so we will get below two equations 
                //-m1x + y = c1 --------(3)
                //-m2x + y = c2 --------(4)

                double x, y;

                //lineA is vertical x1 = x2
                //slope will be infinity
                //so lets derive another solution
                if (Math.Abs(x1 - x2) < tolerance)
                {
                    //compute slope of line 2 (m2) and c2
                    double m2 = (y4 - y3) / (x4 - x3);
                    double c2 = -m2 * x3 + y3;

                    //equation of vertical line is x = c
                    //if line 1 and 2 intersect then x1=c1=x
                    //subsitute x=x1 in (4) => -m2x1 + y = c2
                    // => y = c2 + m2x1 
                    x = x1;
                    y = c2 + m2 * x1;
                }
                //lineB is vertical x3 = x4
                //slope will be infinity
                //so lets derive another solution
                else if (Math.Abs(x3 - x4) < tolerance)
                {
                    //compute slope of line 1 (m1) and c2
                    double m1 = (y2 - y1) / (x2 - x1);
                    double c1 = -m1 * x1 + y1;

                    //equation of vertical line is x = c
                    //if line 1 and 2 intersect then x3=c3=x
                    //subsitute x=x3 in (3) => -m1x3 + y = c1
                    // => y = c1 + m1x3 
                    x = x3;
                    y = c1 + m1 * x3;
                }
                //lineA & lineB are not vertical 
                //(could be horizontal we can handle it with slope = 0)
                else
                {
                    //compute slope of line 1 (m1) and c2
                    double m1 = (y2 - y1) / (x2 - x1);
                    double c1 = -m1 * x1 + y1;

                    //compute slope of line 2 (m2) and c2
                    double m2 = (y4 - y3) / (x4 - x3);
                    double c2 = -m2 * x3 + y3;

                    //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
                    //plugging x value in equation (4) => y = c2 + m2 * x
                    x = (c1 - c2) / (m2 - m1);
                    y = c2 + m2 * x;

                    //verify by plugging intersection point (x, y)
                    //in orginal equations (1) & (2) to see if they intersect
                    //otherwise x,y values will not be finite and will fail this check
                    if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                        && Math.Abs(-m2 * x + y - c2) < tolerance))
                    {
                        //return default (no intersection)
                        return null;
                    }
                }

                //x,y can intersect outside the line segment since line is infinitely long
                //so finally check if x, y is within both the line segments
                if (IsInsideLine(gl, x, y) &&
                    IsInsideLine(this, x, y))
                {
                    return new GeoCordinate(y, x);
                }

                //return default (no intersection)
                return null;

            }

            // Returns true if given point(x,y) is inside the given line segment
            private static bool IsInsideLine(GeoLine line, double x, double y)
            {
                return (x >= line.G1.X && x <= line.G2.X
                            || x >= line.G2.X && x <= line.G1.X)
                       && (y >= line.G1.Y && y <= line.G2.Y
                            || y >= line.G2.Y && y <= line.G1.Y);
            }

            public double Angel
        {
            get
            {
                if(G1.Longitude == G2.Longitude)
                    return 0;

                return (Math.Atan((G1.Latitude - G2.Latitude) / (G1.Longitude - G2.Longitude)) * 180.0 / Math.PI + 90) % 360;
            }
        }
    }
}
