using Microsoft.AspNetCore.SignalR;

namespace SailDisplay.Components.Hubs
{
    public class NetHub : Hub
    {
        public enum eDataType {TimeNow, TWA, TWS, AWA, AWS, SOG, COG, Heading, 
            CTW, //Course Through Water
            STW, //Speed Through water 
            DBS, //depth below surface 
            RSA, //Rudder Angel
            Heeling, TimeToStart, DistanceToStartLine, TimeToBurn, Position, StartMarkStarboard, StartMarkPort,
            WaypointBearing,
            WaypointDistance,
            WaypointPosition}

    }
}
