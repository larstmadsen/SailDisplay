using Microsoft.AspNetCore.SignalR;

namespace SailDisplay.Components.Hubs
{
    public class NetHub : Hub
    {
        public enum eDataType { SOG, COG, Heading, STW }
        public async Task SendData(eDataType dataType, object data)
        {
            await Clients.All.SendAsync("Data", dataType, data);

        }
    }
}
