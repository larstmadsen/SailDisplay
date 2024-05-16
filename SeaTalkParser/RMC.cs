using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class RMC : MessageSeaTalk
    {
        public RMC(string message)
        {
            Type = MessageType.RMC;
            var msg = message.Split(',');
            TimeUTC = Parse(msg[1]);
            Date = Parse(msg[9]);
            Latitude = Parse(msg[3]);
            Longitude = Parse(msg[5]);

        }

        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public double TimeUTC { get; private set; }
        public double Date { get; private set; }


    }
}
