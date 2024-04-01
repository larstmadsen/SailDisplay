using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class VWT : Message
    {
        public VWT(string message)
        {
            Type = MessageType.VWT;
            var msg = message.Split(',');
            CalculatedWindAngelRelativeToTheVessel = (msg[2] == "R" ? Parse(msg[1]) : 360 - Parse(msg[1]));
            CalculatedWindSpeedRelativeToTheVessel = Parse(msg[5]);
        }
        public double CalculatedWindAngelRelativeToTheVessel { get; private set; }
        public double CalculatedWindSpeedRelativeToTheVessel { get; private set; }
    }
}
