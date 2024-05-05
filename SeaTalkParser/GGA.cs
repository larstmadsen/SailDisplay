using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class GGA : MessageSeaTalk
    {
        public GGA(string message)
        {
            Type = MessageType.GGA;
            var msg = message.Split(',');
            TimeUTC = Parse(msg[1]);
            Latitude = Parse(msg[2]);
            Longitude = Parse(msg[4]);
        }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public double TimeUTC { get; private set; }
    }
}
