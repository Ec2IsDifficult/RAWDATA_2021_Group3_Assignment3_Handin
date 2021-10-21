using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Server
{
    class ServerProgram
    {
        private ResponseClass _responseClass;
        private TcpClient _client;
        public void StartServer()
        {
            //Creates a TCPlistener object
            var server = new TcpListener(IPAddress.Loopback, 5000);
            
            //Starts listening on the specific port
            server.Start();
            Console.WriteLine("Server started!");
            
            //Loop until break
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                
                //When a pending connection request, creates a client object of the type NetworkClient
                var _client = new NetworkClient(server.AcceptTcpClient());
                Console.WriteLine("Client connected!");
                
                //The client has already written a message which is located on the network stream
                //The read function reads the message already written.
                var message = _client.Read();
                
                //TODO:This is where the responses should be read and returned
                ResponseClass response = new ResponseClass();
                JObject received = JObject.Parse(message);
                if (received["method"] == null)
                {
                    response.ExpandResponse("missing method");
                }else {
                    switch (received["method"].ToString()) {
                        case "create":
                            Console.WriteLine("Is create");
                            break;
                        case "read":
                            break;
                        case "update":
                            break;
                        case "delete":
                            break;
                        case "echo":
                            break;
                        default:
                            response.ExpandResponse("illegal method");
                            break;
                    }
                }
                if (received["path"] == null)
                {
                    response.ExpandResponse("missing resource");
                }
                if (received["date"] == null)
                {
                    response.ExpandResponse("missing date");
                }

                //Send response at the end
                
                
            }
        }

        public void SendResponse(ResponseClass response)
        {
            string json = JsonConvert.SerializeObject(response);
            _client.Write(json);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServerProgram server = new ServerProgram();
            server.StartServer();

        }
    }
}


public class ResponseClass
{
    public string status { get; set; }
    public void ExpandResponse(string response) {
        status = status + ", " + response;
        Console.WriteLine(status);
    }
}