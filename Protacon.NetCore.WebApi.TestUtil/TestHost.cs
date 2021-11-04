using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;

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
            }, GetSerializerOptionsOrDefault(server)));
        }

        public static Task<Call> Post(this TestServer server, string path, object data, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);

                var content = JsonSerializer.Serialize(data, GetSerializerOptionsOrDefault(server));
                return client.PostAsync(path, new StringContent(content, Encoding.UTF8, "application/json"));
            }, GetSerializerOptionsOrDefault(server)));
        }

        // Similar solution as in https://github.com/RicoSuter/NSwag/blob/d71fc5e5c2e3422b8a00f3d4c2ff792163af6457/src/NSwag.Generation.AspNetCore/AspNetCoreOpenApiDocumentGenerator.cs#L139
        // If newtonsoft support is needed Nswag has example from it too.
        private static JsonSerializerOptions GetSerializerOptionsOrDefault(TestServer server)
        {
            var serviceProvider = server.Services;
            var optionsAssembly = Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc.Core"));
            var optionsType = typeof(IOptions<>).MakeGenericType(optionsAssembly.GetType("Microsoft.AspNetCore.Mvc.JsonOptions", true));

            var options = serviceProvider?.GetService(optionsType) as dynamic;
            return (JsonSerializerOptions)options?.Value?.JsonSerializerOptions;
        }

        public static Task<Call> Put(this TestServer server, string path, object data, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);

                var content = JsonSerializer.Serialize(data, GetSerializerOptionsOrDefault(server));
                return client.PutAsync(path, new StringContent(content, Encoding.UTF8, "application/json"));
            }, GetSerializerOptionsOrDefault(server)));
        }

        public static Task<Call> Patch(this TestServer server, string path, object data, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);

                var content = JsonSerializer.Serialize(data, GetSerializerOptionsOrDefault(server));
                return client.PatchAsync(path, new StringContent(content, Encoding.UTF8, "application/json"));
            }, GetSerializerOptionsOrDefault(server)));
        }

        public static Task<Call> Delete(this TestServer server, string path, Dictionary<string, string> headers = null)
        {
            return Task.Run(() => new Call(() =>
            {
                var client = server.CreateClient();
                AddHeadersIfAny(headers, client);

                return client.DeleteAsync(path);
            }, GetSerializerOptionsOrDefault(server)));
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
