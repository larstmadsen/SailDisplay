using Microsoft.AspNetCore.SignalR;

namespace SailDisplay.Components.Hubs
{
    public class NetHub : Hub
    {
        public enum eDataType {TWA, TWS, AWA, AWS, SOG, COG, Heading, STW, Heeling, HeadingWayPoint, TimeToStart, DistanceToStartLine, TimeToBurn, Position, StartMarkStarboard, StartMarkPort,  }

    }
}
