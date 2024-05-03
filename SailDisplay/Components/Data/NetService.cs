using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SailDisplay.Components.Hubs;
using SeaTalkParser;

namespace SailDisplay.Components.Data
{
    public class NetService
    {
        private bool Simulate = true;
        private static Thread workerThread;
        private static bool workerThreadActive = true;
        private readonly IHubContext<NetHub> _hub;
        private static bool workerBusy = false;

        private YachtDevice.YachtDevice ds;

        public double TWA { get; private set; } = 45.0;

        public double TWS { get; private set; } = 6.0;
        public double AWA { get; private set; } = 0.0;
        public double AWS { get; private set; } = 6.0;

        public double AWA { get; private set; } = 0;
        public double AWS { get; private set; } = 0;

        public double SOG { get; private set; } = 15.0;
        public double COG { get; private set; } = -6.0;
        public double STW { get; private set; } = 6.0;
        public double CTW { get; private set; } = 34.0;
        public double HeadingToWP { get; private set; } = 34.0;
        public double Heading { get; private set; } = 0.0;
        public double HeadingWayPoint { get; private set; } = 34.0;
        public double Heeling { get; private set; } = 0.0;
        public double DBS { get; private set; } = 2;

        public double RSA { get; private set; } = 0;


        public DateTime StartTimestamp { get; set; } = DateTime.Now.AddMinutes(10);
        public double? DistanceToLine { get; private set; } = null;
        public double? TimeToBurn { get; private set; } = null;
        public DateTime StartTimestamp { get; set; } = DateTime.Now.AddMinutes(1);

        public GeoCordinate StartPort { get; set; } = new GeoCordinate(5537.484, 1258.904);
        public GeoCordinate StartStarboard { get; set; } = new GeoCordinate(5537.541, 1259.033);
        public GeoCordinate ActualPosition { get; set; } = new GeoCordinate(5537.700, 1259.000);
        public GeoCordinate ActualWayPoint { get; set; } = new GeoCordinate(5537.450, 1259.010);
        public GeoCordinate NextWayPoint { get; set; } = new GeoCordinate(5537.450, 1259.100);

        public NetService(IHubContext<NetHub> hub)
        {
            _hub = hub;
            if (!Simulate) //False = Simulate
            {
                ds = new YachtDevice.YachtDevice();
                ds.Connect("192.168.4.1", 1456);
            }

            Task workerThread = Task.Run(async () => await Worker());


        }

        public async Task Worker()
        {
            while (workerThreadActive)
            {
                if (Simulate)//Simulate
                {
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

                    GeoLine glWayPoint = new GeoLine(ActualPosition, ActualWayPoint);
                    HeadingWayPoint = glWayPoint.Angel;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.HeadingWayPoint, HeadingWayPoint);

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
                }
                else
                {
                    var msgNMEA = ds.Read();
                 
                    var msgList = Parser.MessageParser(msgNMEA);

                    //Debug.WriteLine(msgNMEA);

                    foreach (Message msg in msgList)
                    {
                        if (msg == null)
                        {
                            Thread.Sleep(50);
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
                            await _hub.Clients.All.SendAsync("double", NetHub.eDataType.STW, STW);
                            await _hub.Clients.All.SendAsync("double", NetHub.eDataType.CTW, CTW);

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
                            var m = (GGA)msg;
                            ActualPosition = new GeoCordinate(m.Latitude, m.Longitude);
                            await _hub.Clients.All.SendAsync("GeoCordinate", NetHub.eDataType.Position, ActualPosition);

                            GeoLine glStart = new GeoLine(StartPort, StartStarboard);
                            GeoLine glActual = new GeoLine(ActualPosition, CTW);

                            GeoCordinate cross = glStart.CrossingPoint(glActual);
                            if (cross != null)
                            {
                                double distance = ActualPosition.GetDistanceTo_Meter(cross);
                                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.DistanceToStartLine, distance);
                            }
                            else
                            {
                                await _hub.Clients.All.SendAsync("double", NetHub.eDataType.DistanceToStartLine, 0);
                            }

                            /*Longitude.Add(m.Longitude);
                            Latitude.Add(m.Latitude);
                            ActualPosition = new GeoCordinate(m.Latitude, m.Longitude);
                            var dist = Math.Round(ActualPosition.GetDistanceTo_Meter(StartPort), 4);
                            var distNM = Math.Round(ActualPosition.GetDistanceTo_NauticMiles(StartPort), 4);
                            DistanceToStart.Add(dist);*/
                        }
                        else if (msg.Type == MessageType.RMB)
                        {
                            var m = (RMB)msg;
                            /*WaypointLongitude.Add(m.Longitude);
                            WaypointLatitude.Add(m.Latitude);
                            WaypointDistance.Add(m.Distance);
                            WaypointHeading.Add(m.Heading);
                            StartPort = new GeoCordinate(m.Latitude, m.Longitude);*/
                        }
                        else if (msg.Type == MessageType.RMC)
                        {
                            var m = (RMC)msg;
                            DateTime dt = DateTime.Now;// DateTime.ParseExact(m.Date.ToString() + " " + m.TimeUTC.ToString("000000.00"), "ddMMyy HHmmss.ff", new CultureInfo("en-US"),
                                                       //DateTimeStyles.None);
                            /*dt = dt.AddHours(2);
                            LocalTime.Add(dt.Ticks);
                            bool test = LocalTime.LatestValue == dt.Ticks;
                            OnPropertyChanged(nameof(LocalTimeHHMMSS));*/
                        }

                    }
                    var splitMessage = msgNMEA.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                }



            }
        }

    }
}
