using System.Net.Sockets;
using System.Text;

namespace Utilities
{
    public class NetworkClient
    {
        private TcpClient _client;
        public NetworkClient()
        {
            _client = new TcpClient();
        }

        public NetworkClient(TcpClient client)
        {
            _client = client;
        }

        public string Read()
        {
            var buffer = new byte[1024];
            var msgCount = _client.GetStream().Read(buffer);
            return Encoding.UTF8.GetString(buffer, 0, msgCount);
        }
        
        static void Main(string[] args){
        }
    }
}    