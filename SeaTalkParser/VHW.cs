using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class VHW : Message
    {
        public VHW(string message)
        {
            Type = MessageType.VHW;
            var msg = message.Split(',');
            HeadingTrue = Parse(msg[1]);
            HeadingMagnetic = Parse(msg[3]);
            SpeedRelativeToWater = Parse(msg[5]);
            DistanceRelativeToWater = Parse(msg[7]);
        }
        public double HeadingTrue { get; private set; }
        public double HeadingMagnetic { get; private set; }
        public double SpeedRelativeToWater { get; private set; }
        public double DistanceRelativeToWater { get; private set; }
    }
}
