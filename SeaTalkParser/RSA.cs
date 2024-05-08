using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class RSA : MessageSeaTalk
    {
        public RSA(string message)
        {
            Type = MessageType.RSA;
            var msg = message.Split(',');
            RudderAngel = Parse(msg[1]);

        }
        public double RudderAngel { get; private set; }
    }
}
