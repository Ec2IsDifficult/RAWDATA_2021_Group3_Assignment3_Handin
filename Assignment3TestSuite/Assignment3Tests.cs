//#define COMMENT
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;


namespace Assignment3TestSuite
{

    public class Response
    {
        public string Status { get; set; }
        public string Body { get; set; }
    }

    public class Category
    {
        [JsonPropertyName("cid")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Assignment3Tests
    {
        private const int Port = 5000;
        
       [Fact]
        public void Constraint_ConnectionWithoutRequest_ShouldConnect()
        {
            var client = Connect();
            Assert.True(client.Connected);
        }

        /*    Method Tests     */
        
        [Fact]
        public void Constraint_RequestWithoutMethod_MissingMethodError()
        {
            var client = Connect();

            client.SendRequest("{}");

            var response = client.ReadResponse();
            Assert.Contains("missing method", response.Status.ToLower());
        }

//if comment
        [Fact]
        public void Constraint_RequestWithUnknownMethod_IllegalMethodError()
        {
            var client = Connect();

            var request = new
            {
                Method = "xxxx",
                Path = "testing",
                Date = UnixTimestamp(),
                Body = "{}"
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            Assert.Contains("illegal method", response.Status.ToLower());
        }

        [Theory]
        [InlineData("create")]
        [InlineData("read")]
        [InlineData("update")]
        [InlineData("delete")]
        public void Constraint_RequestForCreateReadUpdateDeleteWithoutResource_MissingResourceError(string method)
        {
            var client = Connect();

            var request = new
            {
                Method = method,
                Date = DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
            };

            client.SendRequest(request.ToJson());

            var response = client.ReadResponse();

            Assert.Contains("missing resource", response.Status.ToLower());
        }

        /* Date Tests    */

        [Fact]
        public void Constraint_RequestWithoutDate_MissingDateError()
        {
            var client = Connect();

            client.SendRequest("{}");

            var response = client.ReadResponse();

            Assert.Contains("missing date", response.Status.ToLower());
        }

        [Fact]
 /*       public void Constraint_RequestWhereDateIsNotUnixTime_IllegalDateError()
        {
            var client = Connect();

            var request = new
            {
                Method = "update",
                Path = "testing",
                Date = DateTimeOffset.Now.ToString(),
                Body = (new {cid = 1, Name = "Beverages"}).ToJson()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            Assert.Contains("illegal date", response.Status.ToLower());
        }

        /* Body Tests    */
/*
        [Theory]
        [InlineData("create")]
        [InlineData("update")]
        [InlineData("echo")]
        public void Constraint_RequestForCreateUpdateEchoWithoutBody_MissingBodyError(string method)
        {
            var client = Connect();

            var request = new
            {
                Method = method,
                Path = "testing",
                Date = UnixTimestamp()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            Assert.Contains("missing body", response.Status.ToLower());
        }


        [Fact]
        public void Constraint_RequestUpdateWithoutJsonBody_IllegalBodyError()
        {
            var client = Connect();

            var request = new
            {
                Method = "update",
                Path = "/api/categories/1",
                Date = UnixTimestamp(),
                Body = "Hello World"
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();


            Assert.Contains("illegal body", response.Status.ToLower());

        }

        /* Echo Test */

/*
        [Fact]
        public void Echo_RequestWithBody_ReturnsBody()
        {
            var client = Connect();

            var request = new
            {
                Method = "echo",
                Date = UnixTimestamp(),
                Body = "Hello World"
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            Assert.Equal("Hello World", response.Body);

        }

        //////////////////////////////////////////////////////////
        /// 
        /// Testing API 
        /// 
        ////////////////////////////////////////////////////////// 

        /* Path tests  */
/*
        [Fact]
        public void Constraint_RequestWithInvalidPath_StatusBadRequest()
        {
            var client = Connect();

            var request = new
            {
                Method = "read",
                Path = "/api/xxx",
                Date = UnixTimestamp()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var expectedResponse = new Response {Status = "4 Bad Request"};

            Assert.Equal(expectedResponse.ToJson().ToLower(), response.ToJson().ToLower());
        }

        [Fact]
        public void Constraint_RequestWithInvalidPathId_StatusBadRequest()
        {
            var client = Connect();

            var request = new
            {
                Method = "read",
                Path = "/api/categories/xxx",
                Date = UnixTimestamp()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var expectedResponse = new Response {Status = "4 Bad Request"};

            Assert.Equal(expectedResponse.ToJson().ToLower(), response.ToJson().ToLower());
        }

        [Fact]
        public void Constraint_CreateWithPathId_StatusBadRequest()
        {
            var client = Connect();

            var request = new
            {
                Method = "create",
                Path = "/api/categories/1",
                Date = UnixTimestamp(),
                Body = (new {Name = ""}).ToJson()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var expectedResponse = new Response {Status = "4 Bad Request"};

            Assert.Equal(expectedResponse.ToJson().ToLower(), response.ToJson().ToLower());
        }

        [Fact]
        public void Constraint_UpdateWithOutPathId_StatusBadRequest()
        {
            var client = Connect();

            var request = new
            {
                Method = "update",
                Path = "/api/categories",
                Date = UnixTimestamp(),
                Body = (new {Name = ""}).ToJson()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var expectedResponse = new Response {Status = "4 Bad Request"};

            Assert.Equal(expectedResponse.ToJson().ToLower(), response.ToJson().ToLower());
        }

        [Fact]
        public void Constraint_DeleteWithOutPathId_StatusBadRequest()
        {
            var client = Connect();

            var request = new
            {
                Method = "delete",
                Path = "/api/categories",
                Date = UnixTimestamp()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var expectedResponse = new Response {Status = "4 Bad Request"};

            Assert.Equal(expectedResponse.ToJson().ToLower(), response.ToJson().ToLower());
        }



        /* Read tests */
/*
        [Fact]
        public void Request_ReadCategories_StatusOkAndListOfCategoriesInBody()
        {
            var client = Connect();

            var request = new
            {
                Method = "read",
                Path = "/api/categories",
                Date = UnixTimestamp()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var categories = new List<object>
            {
                new {cid = 1, name = "Beverages"},
                new {cid = 2, name = "Condiments"},
                new {cid = 3, name = "Confections"}
            };

            var expectedResponse = new
            {
                Status = "1 Ok",
                Body = categories.ToJson()
            };

            Assert.Equal(expectedResponse.ToJson(), response.ToJson());
        }

        [Fact]
        public void Request_ReadCategoryWithValidId_StatusOkAndCategoryInBody()
        {
            var client = Connect();

            var request = new
            {
                Method = "read",
                Path = "/api/categories/1",
                Date = UnixTimestamp()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var expectedResponse = new Response
            {
                Status = "1 Ok",
                Body = (new {cid = 1, name = "Beverages"}.ToJson())
            };

            Assert.Equal(expectedResponse.ToJson(), response.ToJson());
        }

        [Fact]
        public void Request_ReadCategoryWithInvalidId_StatusNotFound()
        {
            var client = Connect();

            var request = new
            {
                Method = "read",
                Path = "/api/categories/123",
                Date = UnixTimestamp()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            Assert.Contains("5 not found", response.Status.ToLower());
        }


        /* Update tests  */
/*
        [Fact]
        public void Request_UpdateCategoryWithValidIdAndBody_StatusUpdated()
        {
            var client = Connect();

            var request = new
            {
                Method = "update",
                Path = "/api/categories/1",
                Date = UnixTimestamp(),
                Body = (new {cid = 1, name = "BeveragesTesting"}).ToJson()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();


            Assert.Contains("3 updated", response.Status.ToLower());

            // reset data

            client = Connect();

            var resetRequest = new
            {
                Method = "update",
                Path = "/api/categories/1",
                Date = UnixTimestamp(),
                Body = (new {cid = 1, name = "Beverages"}).ToJson()
            };

            client.SendRequest(resetRequest.ToJson());
            client.ReadResponse();
        }

        [Fact]
        public void Request_UpdateCategotyValidIdAndBody_ChangedCategoryName()
        {
            var client = Connect();

            var request = new
            {
                Method = "update",
                Path = "/api/categories/1",
                Date = UnixTimestamp(),
                Body = (new {cid = 1, name = "BeveragesTesting"}).ToJson()
            };

            client.SendRequest(request.ToJson());
            client.ReadResponse();

            client = Connect();
            var readRequest = new
            {
                Method = "read",
                Path = "/api/categories/1",
                Date = UnixTimestamp()
            };

            client.SendRequest(readRequest.ToJson());
            var response = client.ReadResponse();

            Assert.Equal("BeveragesTesting", response.Body.FromJson<Category>().Name);

            // reset data

            client = Connect();

            var resetRequest = new
            {
                Method = "update",
                Path = "/api/categories/1",
                Date = UnixTimestamp(),
                Body = (new {cid = 1, name = "Beverages"}).ToJson()
            };

            client.SendRequest(resetRequest.ToJson());
            client.ReadResponse();
        }

        [Fact]
        public void Request_UpdateCategotyInvalidId_NotFound()
        {
            var client = Connect();

            var request = new
            {
                Method = "update",
                Path = "/api/categories/123",
                Date = UnixTimestamp(),
                Body = (new {cid = 1, name = "BeveragesTesting"}).ToJson()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            Assert.Contains("5 not found", response.Status.ToLower());
        }


        /* Create Tests  */
/*
        [Fact]
        public void Request_CreateCategoryWithValidBodyArgument_CreateNewCategory()
        {
            var client = Connect();

            var request = new
            {
                Method = "create",
                Path = "/api/categories",
                Date = UnixTimestamp(),
                Body = (new {name = "Testing"}).ToJson()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            var category = response.Body.FromJson<Category>();

            Assert.Contains("Testing", category.Name);
            Assert.True(category.Id > 0);

            // reset

            client = Connect();
            var resetRequest = new
            {
                Method = "delete",
                Path = "/api/categories/" + category.Id,
                Date = UnixTimestamp()
            };

            client.SendRequest(resetRequest.ToJson());
            client.ReadResponse();
        }


        /* Delete Tests  */
/*
        [Fact]
        public void Request_DeleteCategoryWithValidId_RemoveCategory()
        {
            var client = Connect();

            var request = new
            {
                Method = "create",
                Path = "/api/categories",
                Date = UnixTimestamp(),
                Body = (new {name = "TestingDeleteCategory"}).ToJson()
            };

            client.SendRequest(request.ToJson());
            var response = client.ReadResponse();

            client = Connect();
            var verifyRequest = new
            {
                Method = "delete",
                Path = "/api/categories/" + response.Body.FromJson<Category>().Id,
                Date = UnixTimestamp()
            };

            client.SendRequest(verifyRequest.ToJson());
            response = client.ReadResponse();

            Assert.Contains("1 ok", response.Status.ToLower());
        }

        [Fact]
        public void Request_DeleteCategoryWithInvalidId_StatusNotFound()
        {
            var client = Connect();
            var verifyRequest = new
            {
                Method = "delete",
                Path = "/api/categories/1234",
                Date = UnixTimestamp()
            };

            client.SendRequest(verifyRequest.ToJson());
            var response = client.ReadResponse();

            Assert.Contains("5 not found", response.Status.ToLower());
        }

        /**********************************************************
         * 
         *  Helper Methods
         * 
        **********************************************************/

        private static string UnixTimestamp()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        }

        private static TcpClient Connect()
        {
            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, Port);
            return client;
        }

    }

    /**********************************************************
    * 
    *  Helper Clases
    * 
    **********************************************************/

    public static class Util
    {
        public static string ToJson(this object data)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public static T FromJson<T>(this string element)
        {
            return JsonSerializer.Deserialize<T>(element, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public static void SendRequest(this TcpClient client, string request)
        {
            var msg = Encoding.UTF8.GetBytes(request);
            client.GetStream().Write(msg, 0, msg.Length);
        }

        public static Response ReadResponse(this TcpClient client)
        {
            var strm = client.GetStream();
            //strm.ReadTimeout = 250;
            byte[] resp = new byte[2048];
            using (var memStream = new MemoryStream())
            {
                int bytesread = 0;
                do
                {
                    bytesread = strm.Read(resp, 0, resp.Length);
                    memStream.Write(resp, 0, bytesread);

                } while (bytesread == 2048);
                
                var responseData = Encoding.UTF8.GetString(memStream.ToArray());
                return JsonSerializer.Deserialize<Response>(responseData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            }
        }
    }
}
