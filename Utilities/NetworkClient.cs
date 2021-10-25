using System;
using System.IO;
using System.Net.Http.Json;
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
            var memStream = new MemoryStream();
            return Encoding.UTF8.GetString(buffer, 0, msgCount);
        }

        public void Connect(string ip, int port)
        {
            _client.Connect(ip, port);
        }

        public void Write(string msg)
        {
            _client.GetStream().Write(Encoding.UTF8.GetBytes(msg));
        }
        
        static void Main(string[] args){
        }
    }
}    