namespace SailDisplay.Components.Data
{
    public class GeoLine
    {
        public GeoLine(GeoCordinate g1, GeoCordinate g2)
        {
            G1 = g1;
            G2 = g2;
        }
        public GeoLine(GeoCordinate g1, double direction)
        {
            G1 = g1;
            G2 = new GeoCordinate(g1.Latitude+Math.Sin(direction), g1.Longitude+Math.Cos(direction));
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
            if(a == null || gl.a == null || a == gl.a)
                return null;

            var x = (gl.b - b) / (a - gl.a);
            var y = a * x + b;
            return new GeoCordinate((double)y, (double)x);
        }
    }
}
