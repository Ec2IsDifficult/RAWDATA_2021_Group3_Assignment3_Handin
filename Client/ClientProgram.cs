using System;
using System.Net;
using Utilities;


namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            var client = new NetworkClient();

            client.Connect("localhost", 5000);
            var message = " Hello";
            client.Write(message);

            var response = client.Read();
            
            Console.WriteLine("Server response " + response);

        }
        
        
    }
}