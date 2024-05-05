using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class VTG : MessageSeaTalk
    {
        public VTG(string message)
        {
            Type = MessageType.VTG;
            var msg = message.Split(',');
            HeadingTrue = Parse(msg[1]);
            HeadingMagnetic = Parse(msg[3]);
            SpeedRelativeToGround = Parse(msg[5]);
            DistanceRelativeToGround = Parse(msg[7]);
        }
        public double HeadingTrue { get; private set; }
        public double HeadingMagnetic { get; private set; }
        public double SpeedRelativeToGround { get; private set; }
        public double DistanceRelativeToGround { get; private set; }
    }
}
