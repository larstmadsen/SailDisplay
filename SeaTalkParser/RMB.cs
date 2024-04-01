using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class RMB : Message
    {
        public RMB(string message)
        {
            Type = MessageType.RMB;
            var msg = message.Split(',');

            Latitude = Parse(msg[6]);
            Longitude = Parse(msg[8]);
            Distance = Parse(msg[10]);
            Heading = Parse(msg[11]);
        }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public double Distance { get; private set; }
        public double Heading { get; private set; }



    }
}
