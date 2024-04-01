using Microsoft.AspNetCore.SignalR;

namespace SailDisplay.Components.Hubs
{
    public class NetHub : Hub
    {
        public enum eDataType { SOG, COG, Heading, STW, Heeling, HeadingToWP, TimeToStart, DistanceToStartLine, TimeToBurn, Position, StartMarkStarboard, StartMarkPort,  }
        public async Task SendData(eDataType dataType, object data)
        {
            await Clients.All.SendAsync("Data", dataType, data);

        }
    }
}
