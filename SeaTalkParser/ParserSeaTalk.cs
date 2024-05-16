using System;
using System.Collections.Generic;
using System.Text;

namespace SeaTalkParser
{
    public class ParserSeaTalk
    {
        public static List<MessageSeaTalk> MessageParser(string message)
        {
            var splitMessage = message.Split('$');
            List<MessageSeaTalk> return_val = new List<MessageSeaTalk>();
            foreach (string msg in splitMessage)
            {
                try
                {
                    if (msg == "")
                    { }
                    else if (msg.Contains("VWT"))
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
                    else if (msg.Contains("GSV"))
                    {
                        //Satellite information
                    }
                    else if (msg.Contains("HDG"))
                    {
                        //Magnetic heading, deviation, variation
                    }
                    else if (msg.Contains("DPT"))
                    {
                        //Depth
                        //"YDDPT,4.12,0.00,*76\r\n"
                    }
                    else if (msg.Contains("MWV"))
                    {
                        //Wind speed/direction??
                        //"YDMWV,148.0,R,1.3,M,A*2C\r\n"
                    }
                    else if (msg.Contains("DTM"))
                    {
                        //Identifies the local geodetic datum
                        //"YDDTM,W84,,0000.0000,N,00000.0000,E,,W84*7B\r\n"
                    }
                    else if (msg.Contains("GLL"))
                    {
                        //Position data....
                        //"YDGLL,5542.6459,N,01235.4998,E,101357.98,A,A*62\r\n"
                    }
                    else if (msg.Contains("ZDA"))
                    {
                        //Date time data
                        //"YDZDA,101358.03,05,05,2024,-02,00*4A\r\n"
                        //12:13 5/5-24
                    }
                    else if (msg.Contains("HDT"))
                    {
                        //Heading True
                        //"YDHDT,333.8,T*34\r\n"
                    }
                    else if (msg.Contains("MWD"))
                    {
                        return_val.Add(new MWD(msg));
                        //Wind direction and speed
                        //"YDMWD,128.8,T,124.1,M,2.5,N,1.2,M*58\r\n"
                    }
                    else if (msg.Contains("VDR"))
                    {
                        //??
                        //"YDVDR,71.5,T,66.8,M,0.1,N*2E\r\n"
                    }
                    else if (msg.Contains("DBT"))
                    {
                        //Depth below transducer
                        //"YDDBT,13.5,f,4.12,M,2.25,F*39\r\n"
                    }
                    else if (msg.Contains("VLW"))
                    {
                        //Distance traveled through water
                        //"YDVLW,11690.099,N,0.000,N*5F\r\n"
                    }
                    else if (msg.Contains("MTW"))
                    {
                        //Mean temperature through water
                        //"YDMTW,16.5,C*0C\r\n"
                    }
                    else if (msg.Contains("MDA"))
                    {
                        //Meteorological composite
                        //"YDMDA,,I,,B,,C,16.5,C,,,,C,128.8,T,124.1,M,2.5,N,1.2,M*1A\r\n"
                    }
                    else if (msg.Contains("HDM"))
                    {
                        //Heading magnetic
                        //"YDHDM,329.1,M*36\r\n"
                    }
                    else if (msg.Contains("GRS"))
                    {
                        //GPS information, range residuals
                        //"YDGRS,101401.77,1,,,,,,,,,,,,,1*5C\r\n"
                    }
                    else if (msg.Contains("VDM"))
                    {
                        //AIS
                        //"!AIVDM,1,1,,A,13@otuOP00Pqm8VOom0ktgwl20S2,0*4A\r\n\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0..."
                    }
                    else if (msg.Contains("XTE"))
                    {
                        //Measured cross track error
                        //"YDXTE,A,A,0.004,L,N,A*3D\r\n"
                    }
                    else if (msg.Contains("BWR"))
                    {
                        //Bearing and distance to waypoint
                        //"YDBWR,140327.49,5543.5400,N,01236.1300,E,21.6,T,16.9,M,0.96,N,StartSB,A*32\r\n"
                    }
                    else if (msg.Contains("APB"))
                    {
                        //Autopilot Sentence B
                        //"YDAPB,A,A,0.004,L,N,V,V,21.9,T,StartSB,21.6,T,21.6,T,A*1A\r\n"
                    }
                    else if (msg.Contains("BOD"))
                    {
                        //Bearing - Origin to distination // USE RMB instead
                        //"YDBOD,21.9,T,17.2,M,StartSB,*12\r\n"
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
