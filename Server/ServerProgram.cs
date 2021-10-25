using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server
{
    class ServerProgram
    {
        private Category _category;
        private readonly List<string> _methodList = new List<string>{"create", "read", "update","delete", "echo"};
        private readonly List<string> _needsBodyList = new List<string>{"create", "update", "echo"};
        private TcpListener _server;

        static void Main()
        {
            ServerProgram server = new ServerProgram();
            server.StartServer();
        }
        
        public void StartServer()
        {
            _category = new Category();
            _server = new TcpListener(IPAddress.Loopback, 5000);
            _server.Start();
            Console.WriteLine("Server started!");
            Listening();
        }

        public void Listening()
        {
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                NetworkClient client = new NetworkClient(_server.AcceptTcpClient());
                Console.WriteLine("Client connected!");
                Thread thread = new Thread(() => TreatClient(client));
                thread.Start();
            }
            
        }

        public void TreatClient(NetworkClient client)
        {
            ErrorResponseClass errorResponseClass = new ErrorResponseClass();
            
            var message = client.Read();
            var received = JsonConvert.DeserializeObject<ReceivedClass>(message);
            if (received.method == null)
            {
                errorResponseClass.ExpandResponse("missing method");
            }else
            {
                if (!_methodList.Contains(received.method))
                    errorResponseClass.ExpandResponse("illegal method");
            }
            if (received.path == null && received.method != "echo")
            {
                errorResponseClass.ExpandResponse("missing resource");
            }
            if (received.date == null)
            {
                errorResponseClass.ExpandResponse("missing date");
            }
            else
            {
                try
                {
                    var sentDate = long.Parse(received.date);
                    DateTimeOffset.FromUnixTimeSeconds(sentDate).DateTime.ToLocalTime();
                }
                catch
                {
                    errorResponseClass.ExpandResponse("illegal date");
                }
            }

            if (received.body == null)
            {
                if (_needsBodyList.Contains(received.method))
                {
                    errorResponseClass.ExpandResponse("missing body");
                }
            }else
            {
                if (received.method != "echo")
                {
                    if (received.body.StartsWith('{') && received.body.EndsWith('}') ||
                        received.body.StartsWith('[') && received.body.EndsWith(']'))
                    {
                        try
                        {
                            JsonConvert.DeserializeObject<dynamic>(received.body);
                        }
                        catch
                        {
                            errorResponseClass.ExpandResponse("illegal body");
                        }
                    }
                    else
                    {
                        errorResponseClass.ExpandResponse("illegal body");
                    }
                }
            }
            
            if (errorResponseClass.status != null)
            {
                SendErrorResponse(errorResponseClass, client);
                return;
            }

            SuccesfullResponseClass succesfullResponseClass = new SuccesfullResponseClass();

            if (received.method == "echo")
            {
                succesfullResponseClass.body = received.body;
                SendSuccesfullResponse(succesfullResponseClass, client);
                return;
            }

            if (received.path != null)
            {
                if (Regex.IsMatch(received.path, "^/api/categories/[0-9]*$")){
                    int index = Int32.Parse(Regex.Match(received.path, @"\d+$").Value);
                    var check = _category.ReturnDatabase();
                    if(check.ContainsKey(index))
                    {
                        switch (received.method)
                        {
                            case "read":
                                succesfullResponseClass.body = JsonConvert.SerializeObject(_category.ReadRow(index));
                                succesfullResponseClass.status = "1 Ok";
                                SendSuccesfullResponse(succesfullResponseClass, client);
                                break;
                            case "delete":
                                _category.DeleteRow(index);
                                succesfullResponseClass.status = "1 Ok";
                                SendSuccesfullResponse(succesfullResponseClass, client);
                                break;
                            case "update":
                                _category.UpdateRow(index, received.body);
                                succesfullResponseClass.status = "3 updated";
                                SendSuccesfullResponse(succesfullResponseClass, client);
                                break;
                            default:
                                errorResponseClass.ExpandResponse("4 Bad Request");
                                SendErrorResponse(errorResponseClass, client);
                                break;
                        }
                    }
                    else
                    {
                        errorResponseClass.ExpandResponse("5 not found");
                        SendErrorResponse(errorResponseClass, client);
                    }
                }else if (Regex.IsMatch(received.path, "^/api/categories$"))
                {
                    switch (received.method)
                    {
                        case "read":
                            succesfullResponseClass.body = JsonConvert.SerializeObject(_category.ReturnCategories());
                            succesfullResponseClass.status = "1 Ok";
                            SendSuccesfullResponse(succesfullResponseClass, client);
                            break;
                        case "create":
                            dynamic results = JsonConvert.DeserializeObject<dynamic>(received.body);
                            int newIndex = _category.CreateRow(results);
                            succesfullResponseClass.body = JsonConvert.SerializeObject(_category.ReadRow(newIndex));
                            succesfullResponseClass.status = "2 Created";
                            Console.WriteLine(succesfullResponseClass.body);
                            SendSuccesfullResponse(succesfullResponseClass, client);
                            break;
                        default:
                            errorResponseClass.ExpandResponse("4 Bad Request");
                            SendErrorResponse(errorResponseClass, client);
                            break;
                    }
                }
                else
                {
                    errorResponseClass.ExpandResponse("4 Bad Request");
                    SendErrorResponse(errorResponseClass, client);
                }
            }
        }

        private void SendErrorResponse(ErrorResponseClass errorResponse, NetworkClient client)
        {
            string json = JsonConvert.SerializeObject(errorResponse);
            client.Write(json);
        }
        
        private void SendSuccesfullResponse(SuccesfullResponseClass succesfullResponse, NetworkClient client)
        {
            string json = JsonConvert.SerializeObject(succesfullResponse);
            client.Write(json);
        }
    }
}

public class ReceivedClass
{
    public string method { get; set; }
    public string path { get; set; }
    public string date { get; set; }
    public string body { get; set; }
}


public class ErrorResponseClass
{
    public string status { get; set; }
    public void ExpandResponse(string response) {
        if (status == null)
        {
            status = response;
        }
        else
        {
            status = status + ", " + response;
        }
    }
}
public class SuccesfullResponseClass
{
    public string status { get; set; }
    public string body { get; set; }
}

public class Category
{
    private Dictionary<int, object> _database;
    public Category()
    {
        _database = new Dictionary<int, object>()
        {
            {1, new {cid = 1, name = "Beverages"}},
            {2, new {cid = 2, name = "Condiments"}},
            {3, new {cid = 3, name = "Confections"}}
        };
    }

    public Dictionary<int, object> ReturnDatabase()
    {
        return _database;
    }

    public List<object> ReturnCategories()
    {
        return _database.Values.ToList();
    }

    public object ReadRow(int cid)
    {
        return _database[cid];
    }
    public int CreateRow(dynamic input)
    {
        int newIndex = _database.Count + 1;
        _database.Add(newIndex, new {cid = newIndex, input.name});
        return newIndex;
    }
    
    public void UpdateRow(int index, string body)
    {
        _database[index] = JObject.Parse(body);
    }    
    public void DeleteRow(int cid)
    {
        _database.Remove(cid);
    }
}
