﻿using System;

namespace SeaTalkParser
{
    public class DBS : MessageSeaTalk
    {
        public DBS(string message)
        {
            Type = MessageType.DBS;
            var msg = message.Split(',');
            Depth = Parse(msg[3]);
        }
        public double Depth { get; private set; }
    }
}
