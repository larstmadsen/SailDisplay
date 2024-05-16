using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaTalkParser
{
    public class MWD : MessageSeaTalk
    {
        public MWD(string message)
        {
            Type = MessageType.MWD;
            var msg = message.Split(',');

            if (msg[2] == "T")
            {
                WindDirectionTrue = Parse(msg[1]);
            }
            else
            {
                WindDirectionMagnetic = Parse(msg[1]);
            }
            if (msg[4] == "T")
            {
                WindDirectionTrue = Parse(msg[3]);
            }
            else
            {
                WindDirectionMagnetic = Parse(msg[3]);
            }
            if (msg[6] == "M")
            {
                WindSpeedTrue = Parse(msg[5]);
            }
            else if (msg[8] == "M")
            {
                WindDirectionTrue = Parse(msg[7]);
            }
        }
        public double WindSpeedTrue { get; private set; }
        public double WindDirectionTrue { get; private set; }
        public double WindDirectionMagnetic { get; private set; }



    }
}
