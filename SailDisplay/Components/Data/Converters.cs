namespace SailDisplay.Components.Data
{
    public class Converters
    {
        public static double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        public static double ToDegrees(double angle)
        {
            return (180 / Math.PI) * angle;
        }

        public static double ToNauticMiles(double meter)
        {
            return meter / 1852;
        }
        public static double ToMeter(double nauticMiles)
        {
            return nauticMiles * 1852;
        }
        public static double AngelDiff(double angel1, double angel2)
        {
            var diff = angel1 - angel2;
            if (diff < -180)
                diff += 360;
            if (diff > 180)
                diff -= 360;
            return diff;
        }
        public static double AngelDiff_0_360(double angel1, double angel2)
        {
            var diff = angel1 - angel2;
            if (diff < 0)
                diff += 360;
            return diff;
        }
    }
}
