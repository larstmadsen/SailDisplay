using System.Globalization;

namespace SeaTalkParser
{
    public enum MessageType { VWT, VWR, DBS, RSA, VHW, VTG, GGA, RMB, RMC }
    public class MessageSeaTalk
    {
        public MessageType Type { get; protected set; }

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
