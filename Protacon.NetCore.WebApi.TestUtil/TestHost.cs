using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public class TestHost
    {
        private readonly TestServer _server;

        private TestHost(TestServer server)
        {
            _server = server;
        }

        public CallResponse Get(string uri, Dictionary<string, string> headers = null)
        {
            using (var client = _server.CreateClient())
            {
                AddHeadersIfAny(headers, client);
                return new CallResponse(client.GetAsync(uri).Result);
            }
        }

        public CallResponse Post(string path, object data, Dictionary<string, string> headers = null)
        {
            using (var client = _server.CreateClient())
            {
                AddHeadersIfAny(headers, client);

                var content = JsonConvert.SerializeObject(data);
                return new CallResponse(client.PostAsync(path, new StringContent(content, Encoding.UTF8, "application/json")).Result);
            }
        }

        public CallResponse Put(string path, object data, Dictionary<string, string> headers = null)
        {
            using (var client = _server.CreateClient())
            {
                AddHeadersIfAny(headers, client);

                var content = JsonConvert.SerializeObject(data);
                return new CallResponse(client
                    .PutAsync(path, new StringContent(content, Encoding.UTF8, "application/json"))
                    .Result);
            }
        }

        public CallResponse Delete(string path, Dictionary<string, string> headers = null)
        {
            using (var client = _server.CreateClient())
            {
                AddHeadersIfAny(headers, client);

                return new CallResponse(client
                    .DeleteAsync(path)
                    .Result);
            }
        }

        private static void AddHeadersIfAny(Dictionary<string, string> headers, HttpClient client)
        {
            headers?
                .ToList()
                .ForEach(x => client.DefaultRequestHeaders.Add(x.Key, x.Value));
        }

        public TestHost MockPassing<TMock>(Action<TMock> asserts)
        {
            var target = _server.Host.Services.GetService(typeof(TMock));
            asserts((TMock)target);
            return this;
        }

        public TestHost MockSetup<TMock>(Action<TMock> setups)
        {
            var target = _server.Host.Services.GetService(typeof(TMock));

            if (target == null)
                throw new InvalidOperationException($"Tried to mock type '{typeof(TMock)}' but nothing matched in current host container.");

            setups((TMock)target);
            return this;
        }

        public static TestHost Run<T>() where T : class
        {
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<T>());

            return new TestHost(server);
        }
    }
}
