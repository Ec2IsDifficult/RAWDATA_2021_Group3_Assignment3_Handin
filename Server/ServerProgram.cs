using System;
using System.Net;
using System.Net.Sockets;
using Utilities; 

namespace Server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("localhost");
            var server = new TcpListener(ip, 5000);
            server.Start();
            Console.WriteLine("Server started!");

            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                var client = new NetworkClient(server.AcceptTcpClient());
                Console.WriteLine("Client connected!");

                var message = client.Read();
                Console.WriteLine("Message from client" + message);
                break;
            }
        }
    }
}