namespace SailDisplay.Components.Data
{
    public class GeoCordinate
    {
        public GeoCordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public double X { get { return Longitude; } }
        public double Y { get { return Latitude; } }

        private double LatitudeWGS84
        {
            get
            {
                var t1 = Latitude;
                var t2 = t1 % 1;
                var t3 = t2 * 0.60;
                return (t1 - t2 + t3) / 100;
            }
        }
        private double LongitudeWGS84
        {
            get
            {
                var t1 = Longitude;
                var t2 = t1 % 1;
                var t3 = t2 * 0.60;
                return (t1 - t2 + t3) / 100;
            }
        }
        private double LatitudeWGS84Decimal
        {
            get
            {
                var t1 = Latitude / 100;
                var t2 = t1 % 1;
                var t3 = t2 / 0.60;
                return (t1 - t2 + t3);
            }
        }
        private double LongitudeWGS84Decimal
        {
            get
            {
                var t1 = Longitude / 100;
                var t2 = t1 % 1;
                var t3 = t2 / 0.60;
                return (t1 - t2 + t3);
            }
        }
        public double GetDistanceTo_NauticMiles(GeoCordinate other)
        {

            return (GetDistanceTo_Meter(other) / 1852);
        }
        public double GetDistanceTo_Meter(GeoCordinate other)
        {
            if (other == null || double.IsNaN(LatitudeWGS84Decimal) || double.IsNaN(LongitudeWGS84Decimal) || double.IsNaN(other.LatitudeWGS84Decimal) ||
                double.IsNaN(other.LongitudeWGS84Decimal))
            {
                return 0;
            }

            var d1 = LatitudeWGS84Decimal * (Math.PI / 180.0);
            var num1 = LongitudeWGS84Decimal * (Math.PI / 180.0);
            var d2 = other.LatitudeWGS84Decimal * (Math.PI / 180.0);
            var num2 = other.LongitudeWGS84Decimal * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return (6376.5 * 1000) * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }


        


    }
}
