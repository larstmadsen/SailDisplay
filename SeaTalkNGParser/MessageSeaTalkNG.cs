using System.Globalization;

namespace SeaTalkNGParser
{
    public enum MessageTypeSeaTalkNG { VWT, VWR, DBS, RSA, VHW, VTG, GGA, RMB, RMC }
    public class MessageSeaTalkNG
    {
        public MessageTypeSeaTalkNG Type { get; protected set; }

        public double Parse(string text)
        {
            var pointCulture = new CultureInfo("en")
            {
                NumberFormat =
            {
                NumberDecimalSeparator = "."
            }
            };


            return double.Parse(text, pointCulture);
        }
    }

}
