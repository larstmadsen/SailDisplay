using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SailDisplay.Components.Hubs;

namespace SailDisplay.Components.Data
{
    public class NetService
    {
        private static Thread workerThread;
        private static bool workerThreadActive = true;
        private readonly IHubContext<NetHub> _hub;
        private static bool workerBusy = false;

        private YachtDevice.YachtDevice ds;

        public double TWA { get; private set; } = 45.0;

        public double TWS { get; private set; } = 6.0;

        public double AWA { get; private set; } = 0;
        public double AWS { get; private set; } = 0;

        public double SOG { get; private set; } = 15.0;
        public double COG { get; private set; } = -6.0;
        public double STW { get; private set; } = 6.0;
        public double Heading { get; private set; } = 6.0;
        public double HeadingToWP { get; private set; } = 34.0;
        public double Heeling { get; private set; } = 0.0;
        public double? DistanceToLine { get; private set; } = null;
        public double? TimeToBurn { get; private set; } = null;
        public DateTime StartTimestamp { get; set; } = DateTime.Now.AddMinutes(1);

        public GeoCordinate StartPort { get; set; } = new GeoCordinate(5537.484, 1258.904);
        public GeoCordinate StartStarboard { get; set; } = new GeoCordinate(5537.541, 1259.033);
        public GeoCordinate ActualPosition { get; set; } = new GeoCordinate(5537.700, 1259.000);

        public NetService(IHubContext<NetHub> hub)
        {
            _hub = hub;
            if (false) //Simulate
            {
                ds = new YachtDevice.YachtDevice();
            }

            Task workerThread = Task.Run(async () => await Worker());
            /*if (workerThread == null)
            {
                workerThread = new Thread(WorkerThread);
                workerThread.Start();
                Console.WriteLine("NetService WorkerThread start");
            }*/


        }

        /*public async Task WorkerThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Console.WriteLine("NetService Thread Subscripe");
            }).Start();

            while (workerThreadActive)
            {
                Worker();
            }
        }*/
        public async Task Worker()
        {
            while (workerThreadActive)
            {
                if (true)//Simulate
                {
                    Random r = new Random();

                    TWS += r.Next(-1, 1) * 0.01;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TWS, TWS);

                    TWA += r.Next(-1, 1) * 1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TWA, TWA);

                    AWS = TWS * 1.1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.AWS, AWS);

                    AWA = TWA * 0.8;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.AWA, AWA);

                    SOG += r.Next(-5, 5) * 0.01;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.SOG, SOG);

                    COG += r.Next(-5, 5) * 0.1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.COG, COG);

                    STW += r.Next(-5, 5) * 0.01;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.STW, STW);

                    Heading += r.Next(-5, 5) * 0.1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.Heading, Heading);

                    Heeling += r.Next(-2, 2) * 0.1;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.Heeling, Heeling);

                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.HeadingToWP, HeadingToWP);

                    TimeSpan ts = StartTimestamp - DateTime.Now;
                    await _hub.Clients.All.SendAsync("double", NetHub.eDataType.TimeToStart, ts.TotalSeconds);

                    ActualPosition = new GeoCordinate(ActualPosition.Latitude-0.001, ActualPosition.Longitude);
                    await _hub.Clients.All.SendAsync("GeoCordinate", NetHub.eDataType.Position, ActualPosition);

                    GeoLine glStart = new GeoLine(StartPort, StartStarboard);
                    GeoLine glActual = new GeoLine(ActualPosition, Heading);

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
                
                }



            }
        }

    }
}
