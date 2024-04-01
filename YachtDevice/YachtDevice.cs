using System;
using System.Net.Sockets;
using System.Text;

namespace YachtDevice
{
    public class YachtDevice
    {
        private TcpClient client;
        private NetworkStream netStream;

        public YachtDevice()
        {

        }
        public void Connect(string server, int port)
        {
            client = new TcpClient();
            client.Connect(server, port);
            netStream = client.GetStream();
        }

        public string Read()
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];

            // Read can return anything from 0 to numBytesToRead. 
            // This method blocks until at least one byte is read.
            netStream.Read(bytes, 0, (int)client.ReceiveBufferSize);

            // Returns the data received from the host to the console.
            string returndata = Encoding.UTF8.GetString(bytes);

            return returndata;

        }

    }
}
