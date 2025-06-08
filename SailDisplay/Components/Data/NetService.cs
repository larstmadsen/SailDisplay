using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SailDisplay.Components.Hubs;
using SeaTalkParser;
using SeaTalkNGParser;
using System.Globalization;
using System.Text;

namespace SailDisplay.Components.Data
{
    public class NetService
    {
        private bool Simulate = false;
        private DateTime SimulateFastUntil = new DateTime(2024, 5, 14, 18, 10, 45);
        private static Thread workerThread;
        private static bool workerThreadActive = true;
        private readonly IHubContext<NetHub> _hub;
        private static bool workerBusy = false;

        private YachtDevice.YachtDevice dsSeaTalk;
        private YachtDevice.YachtDevice dsSeaTalkNG;

        public DateTime TimeNow { get; private set; }
        public double TWA { get; private set; } = 45.0;

        public double TWS { get; private set; } = 6.0;
        public double AWA { get; private set; } = 0.0;
        public double AWS { get; private set; } = 6.0;
        public double SOG { get; private set; } = 15.0;
        public double COG { get; private set; } = -6.0;
        public double STW { get; private set; } = 6.0;
        public double CTW { get; private set; } = 34.0;
        public double Heading { get; private set; } = 0.0;
        public double WaypointBearing { get; private set; } = 34.0;
        public double WaypointDistance { get; private set; } = 34.0;

        public double Heeling { get; private set; } = 0.0;
        public double DBS { get; private set; } = 2;

        public double RSA { get; private set; } = 0;

        public double WindSpeedTrue { get; private set; }
        public double WindDirectionTrue { get; private set; }
        public double WindDirectionMagnetic { get; private set; }

        public DateTime StartTimestamp { get; set; } = new DateTime(2025, 6, 8, 18, 12, 13); //DateTime.Now.AddMinutes(10);
        public double? DistanceToLine { get; private set; } = null;
        public double? TimeToBurn { get; private set; } = null;

        public GeoCordinate StartPort { get; set; } = new GeoCordinate(5543.5761, 1236.0715);// new GeoCordinate(5537.484, 1258.904); //55 43.58N 12 36.01E
        public GeoCordinate StartStarboard { get; set; } = new GeoCordinate(5543.5516, 1236.1213); // new GeoCordinate(5537.541, 1259.033); //55 43.58N 12 36.01E
        public GeoCordinate ActualPosition { get; set; } = new GeoCordinate(5543.4700, 1236.1000);
        public GeoCordinate WaypointPosition { get; set; } = new GeoCordinate(5537.450, 1259.010);
        
        public NetService(IHubContext<NetHub> hub)
        {
            _hub = hub;
            if (!Simulate) //False = Simulate
            {
                dsSeaTalk = new YachtDevice.YachtDevice();
                dsSeaTalk.Connect("192.168.4.1", 1456);

                dsSeaTalkNG = new YachtDevice.YachtDevice();
                dsSeaTalkNG.Connect("192.168.4.1", 1457);
            }

            Task workerThread = Task.Run(async () => await Worker());


        }

        public async Task Worker()
        {
            while (workerThreadActive)
            {
                if (Simulate) //Simulate
                {
                    using (var fileStream = File.OpenRead(@"c:\temp\SeaTalk_log.txt"))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128))
                    {
                        String line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            var msgListSeaTalk = ParserSeaTalk.MessageParser(line);
                            foreach (MessageSeaTalk msg in msgListSeaTalk)
                            {
                                await ParseSeTalkMessage(msg);
                            }
                            if (TimeNow > SimulateFastUntil)
                            {
                                Thread.Sleep(20);
                            }
                        }
                    }

                    
                    /*
                    Random r = new Random();

                    TWS += r.Next(-1, 1) * 0.01;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TWS, TWS);

                    TWA = (TWA + r.Next(-5, 5) * 1 + 360) % 360;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TWA, TWA);

                    AWS = TWS * 1.1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.AWS, AWS);

                    AWA = TWA * 0.8;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.AWA, AWA);

                    SOG += r.Next(-5, 5) * 0.01;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.SOG, SOG);

                    COG = (COG + r.Next(-5, 5) * 0.1 +360) % 360;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.COG, COG);

                    STW += r.Next(-5, 5) * 0.01;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.STW, STW);

                    CTW += r.Next(-5, 5) * 0.1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.CTW, CTW);
                    Heading = (Heading + r.Next(-5, 5) * 0.1 + 360) % 360;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.Heading, Heading);

                    Heeling += r.Next(-2, 2) * 0.1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.Heeling, Heeling);

                    GeoLine glWayPoint = new GeoLine(ActualPosition, WaypointPosition);
                    WaypointBearing = glWayPoint.Angel;
                    
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.WaypointBearing, WaypointBearing);

                    TimeSpan ts = StartTimestamp - DateTime.Now;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TimeToStart, ts.TotalSeconds);

                    ActualPosition = new GeoCordinate(ActualPosition.Latitude-0.001, ActualPosition.Longitude);
                    await _hub.Clients.All.SendAsync("GeoCordinate", NetHub.eDataType.Position, ActualPosition);

                    GeoLine glStart = new GeoLine(StartPort, StartStarboard);
                    GeoLine glActual = new GeoLine(ActualPosition, CTW);

                    GeoCordinate cross = glStart.CrossingPoint(glActual);
                    if (cross != null)
                    {
                        DistanceToLine = ActualPosition.GetDistanceTo_Meter(cross);
                        var angel = glStart.Angel;
                        GeoLine glPortMarkBoat = new GeoLine(StartPort, ActualPosition);
                        var angelPortMarkBoat = glPortMarkBoat.Angel;
                        if((360 + angel - angelPortMarkBoat) % 360 < 180)
                        {
                            DistanceToLine = -DistanceToLine;
                        }
                    }
                    else
                    {
                        DistanceToLine = null;
                    }
                    await _hub.Clients.All.SendAsync("double?", NetHub.eDataType.DistanceToStartLine, DistanceToLine);

                    if(DistanceToLine != null && DistanceToLine >= 0)
                    {
                        TimeToBurn = ts.TotalSeconds - (Converters.ToNauticMiles((double)DistanceToLine)/SOG)*60*60;
                    }
                    else
                    {
                        TimeToBurn = null;
                    }
                    await _hub.Clients.All.SendAsync("double?", NetHub.eDataType.TimeToBurn, TimeToBurn);

                    Thread.Sleep(500);
                    */
                }
                else
                {
                    var msgSeaTalk = dsSeaTalk.Read();
                    var msgListSeaTalk = ParserSeaTalk.MessageParser(msgSeaTalk);
                    foreach (MessageSeaTalk msg in msgListSeaTalk)
                    {
                        await ParseSeTalkMessage(msg);
                    }
                    await Log(msgSeaTalk, "SeaTalk");

                    var msgSeaTalkNG = dsSeaTalkNG.Read();
                    var msgListSeaTalkNG = ParserSeaTalkNG.MessageParser(msgSeaTalkNG);
                    foreach (MessageSeaTalkNG msg in msgListSeaTalkNG)
                    {
                        await ParseSeTalkNGMessage(msg);
                    }
                    await Log(msgSeaTalkNG, "SeaTalkNG");
                }

                

            }
        }

        public async Task Log(string msg, string type)
        {
            try
            {
                if(msg != null && msg.Length > 0 && !Simulate) 
                { 
                    //using (StreamWriter outputFile = new StreamWriter(Path.Combine("/home/admin/ww", "log.txt"), true))
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(@"c:\temp\", type + "_log.txt"), true))
                    {
                        outputFile.WriteLine(msg);
                    }
                }
            }
            catch
            { }
        }

        public async Task ParseSeTalkMessage(MessageSeaTalk msg)
        {
            if (msg == null)
            {
                if (!Simulate)
                {
                    Thread.Sleep(50);
                }
            }
            else if (msg.Type == MessageType.RMC)
            {
                var m = (RMC)msg;
                DateTime dt = /*DateTime.Now;//*/ DateTime.ParseExact(m.Date.ToString("000000") + " " + m.TimeUTC.ToString("000000.00"), "ddMMyy HHmmss.ff", new CultureInfo("en-US"), DateTimeStyles.None);
                dt = dt.AddHours(2);

                TimeNow = dt;

                TimeSpan ts = StartTimestamp - dt;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TimeToStart, ts.TotalSeconds);
                await _hub.Clients.All.SendAsync("DateTime", NetHub.eDataType.TimeNow, dt);


                ActualPosition = new GeoCordinate(m.Latitude, m.Longitude);
                await _hub.Clients.All.SendAsync("GeoCordinate", NetHub.eDataType.Position, ActualPosition);

                GeoLine glStart = new GeoLine(StartPort, StartStarboard);
                GeoLine glActual = new GeoLine(ActualPosition, CTW);

                GeoCordinate cross = glStart.CrossingPoint(glActual);
                if (cross != null)
                {
                    DistanceToLine = ActualPosition.GetDistanceTo_Meter(cross);
                }
                else
                {
                    DistanceToLine = null;
                }
                await _hub.Clients.All.SendAsync("double?", NetHub.eDataType.DistanceToStartLine, DistanceToLine);

                if (DistanceToLine != null && DistanceToLine >= 0 && SOG > 0)
                {
                    TimeToBurn = ts.TotalSeconds - (Converters.ToNauticMiles((double)DistanceToLine) / SOG) * 60 * 60;
                }
                else
                {
                    TimeToBurn = null;
                }
                await _hub.Clients.All.SendAsync("double?", NetHub.eDataType.TimeToBurn, TimeToBurn);

            }
            else if (Simulate && SimulateFastUntil > TimeNow)
            { 
            
            }
            else if (msg.Type == MessageType.VWT)
            {
                var m = (VWT)msg;
                TWS = m.CalculatedWindSpeedRelativeToTheVessel;
                TWA = m.CalculatedWindAngelRelativeToTheVessel;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TWS, m.CalculatedWindSpeedRelativeToTheVessel);
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TWA, m.CalculatedWindAngelRelativeToTheVessel);

                //TWS.Add(m.CalculatedWindSpeedRelativeToTheVessel);
                //TWA.Add(m.CalculatedWindAngelRelativeToTheVessel);
            }
            else if (msg.Type == MessageType.VWR)
            {
                var m = (VWR)msg;
                AWS = m.WindSpeedRelativeToTheVessel;
                AWA = m.WindAngelRelativeToTheVessel;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.AWS, m.WindSpeedRelativeToTheVessel);
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.AWA, m.WindAngelRelativeToTheVessel);

                //TWS.Add(m.WindSpeedRelativeToTheVessel);
                //AWA.Add(m.WindAngelRelativeToTheVessel);
            }
            else if (msg.Type == MessageType.VTG)
            {
                var m = (VTG)msg;
                SOG = m.SpeedRelativeToGround;
                COG = m.HeadingTrue;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.SOG, m.SpeedRelativeToGround);
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.COG, m.HeadingTrue);

                //SOG.Add(m.SpeedRelativeToGround);
                //COG.Add(m.HeadingTrue);
            }
            else if (msg.Type == MessageType.VHW)
            {
                var m = (VHW)msg;
                STW = m.SpeedRelativeToWater;
                CTW = m.HeadingMagnetic;
                Heading = m.HeadingMagnetic;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.STW, STW);
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.CTW, CTW);
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.Heading, Heading);


                //Speed.Add(m.SpeedRelativeToWater);
                //Heading.Add(m.HeadingMagnetic);
                //HeadingTrue.Add(m.HeadingTrue);
                //var cav = new View.ConverterAngelVessel();
                //cav.SetHeading(m.HeadingTrue);

            }
            else if (msg.Type == MessageType.DBS)
            {
                var m = (DBS)msg;
                DBS = m.Depth;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.DBS, m.Depth);
                //DBS.Add(m.Depth);

            }
            else if (msg.Type == MessageType.RSA)
            {
                var m = (RSA)msg;
                RSA = m.RudderAngel;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.RSA, m.RudderAngel);
                //RSA.Add(m.RudderAngel);
            }
            else if (msg.Type == MessageType.GGA)
            {
/*                var m = (GGA)msg;
                ActualPosition = new GeoCordinate(m.Latitude, m.Longitude);
                await _hub.Clients.All.SendAsync("GeoCordinate", NetHub.eDataType.Position, ActualPosition);

                GeoLine glStart = new GeoLine(StartPort, StartStarboard);
                GeoLine glActual = new GeoLine(ActualPosition, CTW);

                GeoCordinate cross = glStart.CrossingPoint(glActual);
                if (cross != null)
                {
                    DistanceToLine = ActualPosition.GetDistanceTo_Meter(cross);
                }
                else
                {
                    DistanceToLine = null;
                }
                await _hub.Clients.All.SendAsync("double?", NetHub.eDataType.DistanceToStartLine, DistanceToLine);

                if (DistanceToLine != null && DistanceToLine >= 0 && SOG > 0)
                {
                    TimeSpan ts = StartTimestamp - TimeNow;
                    TimeToBurn = ts.TotalSeconds - (Converters.ToNauticMiles((double)DistanceToLine) / SOG) * 60 * 60;
                }
                else
                {
                    TimeToBurn = null;
                }
                await _hub.Clients.All.SendAsync("double?", NetHub.eDataType.TimeToBurn, TimeToBurn);
*/

            }
            else if (msg.Type == MessageType.RMB)
            {
                var m = (RMB)msg;
                WaypointPosition = new GeoCordinate(m.Latitude, m.Longitude);
                await _hub.Clients.All.SendAsync("GeoCordinate", NetHub.eDataType.WaypointPosition, WaypointPosition);

                WaypointDistance = m.Distance;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.WaypointDistance, WaypointDistance);

                WaypointBearing = m.Bearing;
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.WaypointBearing, WaypointBearing);


                /*WaypointLongitude.Add(m.Longitude);
                WaypointLatitude.Add(m.Latitude);
                WaypointDistance.Add(m.Distance);
                WaypointHeading.Add(m.Heading);
                StartPort = new GeoCordinate(m.Latitude, m.Longitude);*/
            }
            else if (msg.Type == MessageType.MWD)
            {
                var m = (MWD)msg;

                WindSpeedTrue = m.WindSpeedTrue;
                WindDirectionTrue = m.WindDirectionTrue;
                WindDirectionMagnetic = m.WindDirectionMagnetic;

                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.WindSpeedTrue, WindSpeedTrue);
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.WindDirectionTrue, WindDirectionTrue);
                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.WindDirectionMagnetic, WindDirectionMagnetic);


                //dt = dt.AddHours(2);
                //LocalTime.Add(dt.Ticks);
                //bool test = LocalTime.LatestValue == dt.Ticks;
                //OnPropertyChanged(nameof(LocalTimeHHMMSS));
            }

        }

        public async Task ParseSeTalkNGMessage(MessageSeaTalkNG message)
        {

        }

    }
}
