using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SailDisplay.Components.Hubs;

namespace SailDisplay.Components.Data
{
    public class NetService
    {
        private static Thread workerThread;
        private readonly IHubContext<NetHub> _hub;
        private static bool workerBusy = false;

        public double SOG { get; private set; } = 6.0;
        public double COG { get; private set; } = 34.0;
        public double STW { get; private set; } = 6.0;
        public double Heading { get; private set; } = 34.0;

        public NetService(IHubContext<NetHub> hub)
        {
            _hub = hub;

            if (workerThread == null)
            {
                workerThread = new Thread(WorkerThread);
                workerThread.Start();
                Console.WriteLine("NetService WorkerThread start");
            }


        }

        public void WorkerThread()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Console.WriteLine("NetService Thread Subscripe");
            }).Start();

            while (true)
            {
                Worker();
                Thread.Sleep(500);
            }
        }
        public async Task Worker()
        {
            if (!workerBusy)
            {

                workerBusy = true;

                Console.WriteLine("NetService Worker");

                Random r = new Random();

                SOG += r.Next(-5, 5) * 0.01;
                await _hub.Clients.All.SendAsync("Data", NetHub.eDataType.SOG, SOG);

                COG += r.Next(-5, 5) * 0.01;
                await _hub.Clients.All.SendAsync("Data", NetHub.eDataType.COG, COG);

                STW += r.Next(-5, 5) * 0.01;
                await _hub.Clients.All.SendAsync("Data", NetHub.eDataType.STW, STW);

                Heading += r.Next(-5, 5) * 0.01;
                await _hub.Clients.All.SendAsync("Data", NetHub.eDataType.Heading, Heading);


                workerBusy = false;
            }
        }

    }
}
