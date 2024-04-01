using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class VWR : Message
    {
        public VWR(string message)
        {
            Type = MessageType.VWR;
            var msg = message.Split(',');
            WindAngelRelativeToTheVessel = (msg[2] == "R" ? Parse(msg[1]) : 360 - Parse(msg[1]));
            WindSpeedRelativeToTheVessel = Parse(msg[5]);
        }
        public double WindAngelRelativeToTheVessel { get; private set; }
        public double WindSpeedRelativeToTheVessel { get; private set; }
    }
}
