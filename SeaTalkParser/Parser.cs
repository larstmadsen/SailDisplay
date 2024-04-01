using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class Parser
    {
        public static List<Message> MessageParser(string message)
        {
            var splitMessage = message.Split('$');
            List<Message> return_val = new List<Message>();
            foreach (string msg in splitMessage)
            {
                try
                {
                    if (msg.Contains("VWT"))
                    {
                        return_val.Add(new VWT(msg));
                    }
                    else if (msg.Contains("VWR"))
                    {
                        return_val.Add(new VWR(msg));
                    }
                    else if (msg.Contains("DBS"))
                    {
                        return_val.Add(new DBS(msg));
                    }
                    else if (msg.Contains("RSA"))
                    {
                        return_val.Add(new RSA(msg));
                    }
                    else if (msg.Contains("VHW"))
                    {
                        return_val.Add(new VHW(msg));
                    }
                    else if (msg.Contains("VTG"))
                    {
                        return_val.Add(new VTG(msg));
                    }
                    else if (msg.Contains("GGA"))
                    {
                        return_val.Add(new GGA(msg));
                    }
                    else if (msg.Contains("RMB"))
                    {
                        return_val.Add(new RMB(msg));
                    }
                    else if (msg.Contains("RMC"))
                    {
                        return_val.Add(new RMC(msg));
                    }
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
