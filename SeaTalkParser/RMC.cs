using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class RMC : Message
    {
        public RMC(string message)
        {
            Type = MessageType.RMC;
            var msg = message.Split(',');
            TimeUTC = Parse(msg[1]);
            Date = Parse(msg[9]);

        }
        public double TimeUTC { get; private set; }
        public double Date { get; private set; }


    }
}
