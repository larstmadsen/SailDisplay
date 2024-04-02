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
    }
}
