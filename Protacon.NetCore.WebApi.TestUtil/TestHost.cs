using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public static class TestHost
    {
        public static Task<Call> Get(this TestServer server, string uri, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);
                return client.GetAsync(uri);
            }));
        }

        public static Task<Call> Post(this TestServer server, string path, object data, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);

                var content = JsonSerializer.Serialize(data);
                return client.PostAsync(path, new StringContent(content, Encoding.UTF8, "application/json"));
            }));
        }

        public static Task<Call> Put(this TestServer server, string path, object data, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);

                var content = JsonSerializer.Serialize(data);
                return client.PutAsync(path, new StringContent(content, Encoding.UTF8, "application/json"));
            }));
        }

        public static Task<Call> Delete(this TestServer server, string path, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);

                return client.DeleteAsync(path);
            }));
        }

        private static void AddHeadersIfAny(Dictionary<string, string> headers, HttpClient client)
        {
            headers?
                .ToList()
                .ForEach(x => client.DefaultRequestHeaders.Add(x.Key, x.Value));
        }

        public static TestServer Passing<TService>(this TestServer server, Action<TService> asserts)
        {
            var target = server.Host.Services.GetService(typeof(TService));

            if (target == null)
                throw new InvalidOperationException($"Tried to get type '{typeof(TService)}' but nothing matched in current host container.");

            asserts((TService)target);
            return server;
        }

        public static TestServer Setup<TService>(this TestServer server, Action<TService> setups)
        {
            var target = server.Host.Services.GetService(typeof(TService));

            if (target == null)
                throw new InvalidOperationException($"Tried to get type '{typeof(TService)}' but nothing matched in current host container.");

            setups((TService)target);
            return server;
        }

        public static TestServer Run<T>() where T : class
        {
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<T>());

            return server;
        }
    }
}
