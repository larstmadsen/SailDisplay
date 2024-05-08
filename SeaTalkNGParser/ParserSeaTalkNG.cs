using SeaTalkNGParser;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SeaTalkNGParser
{
    public class ParserSeaTalkNG
    {
        public static List<MessageSeaTalkNG> MessageParser(string message)
        {
            var splitMessage = message.Split("\r\n");
            List<MessageSeaTalkNG> return_val = new List<MessageSeaTalkNG>();
            foreach (string msg in splitMessage)
            {
                try
                {
                    if (msg == "")
                    { }
                   
                    else if (msg.Contains("RMC"))
                    {
                        //return_val.Add(new RMC(msg));
                    }
                    else { }
                }
                catch (Exception e)
                {
                    //Debug.WriteLine("Parse exception: " + e.ToString());
                }
            }
            return return_val;
        }

    }
}
