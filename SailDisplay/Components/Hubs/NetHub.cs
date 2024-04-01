using Microsoft.AspNetCore.SignalR;

namespace SailDisplay.Components.Hubs
{
    public class NetHub : Hub
    {
        public enum eDataType {TWA, TWS, SOG, COG, Heading, STW, Heeling, HeadingToWP, TimeToStart, DistanceToStartLine, TimeToBurn, Position, StartMarkStarboard, StartMarkPort,  }

    }
}
