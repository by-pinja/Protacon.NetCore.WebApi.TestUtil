using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public static class CallExtensions
    {
        public static async Task<Call> ExpectStatusCode(this Task<Call> callTask, HttpStatusCode code)
        {
            var call = await callTask.ConfigureAwait(false);
            var httpCall = await call.HttpTask.ConfigureAwait(false);

            if (httpCall.StatusCode != code)
            {
                throw new ExpectedStatusCodeException($"Expected statuscode '{code}' but got '{(int)httpCall.StatusCode}'");
            }

            call.ExpectedStatusCode = code;

            return call;
        }

        public static async Task<Call> HeaderPassing(this Task<Call> callTask, string header, Action<string> assertsForValue)
        {
            var call = await callTask.ConfigureAwait(false);
            var response = await call.HttpTask.ConfigureAwait(false);

            var match = response.Headers
                .Concat(response.Content.Headers)
                .SingleOrDefault(x => x.Key == header);

            if (match.Equals(default(KeyValuePair<string, IEnumerable<string>>)))
                throw new InvalidOperationException($"Header '{header}' not found, available headers are '{HeadersAsReadableList(response)}'");

            assertsForValue.Invoke(match.Value.Single());

            return call;
        }

        private static string HeadersAsReadableList(HttpResponseMessage message)
        {
            return message.Headers.Concat(message.Content.Headers)
                .Select(x => x.Key.ToString()).Aggregate("", (a, b) => $"{a}, {b}");
        }

        public static async Task<CallData<T>> WithContentOf<T>(this Task<Call> callTask)
        {
            var call = await callTask.ConfigureAwait(false);
            var result = await call.HttpTask.ConfigureAwait(false);
            var code = (int)result.StatusCode;

            var content = await result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

            if ((code > 299 || code < 199) && code != (int?)call.ExpectedStatusCode)
                throw new ExpectedStatusCodeException(
                    $"Tried to get data from non ok statuscode response, expected status is '2xx' or '{call.ExpectedStatusCode}' but got '{code}' with content '{Encoding.Default.GetString(content)}'");

            if (!result.Content.Headers.Contains("Content-Type"))
                throw new InvalidOperationException("Response didn't contain any 'Content-Type'. Reason may be that you didn't return anything?");

            var contentType = result.Content.Headers.Single(x => x.Key == "Content-Type").Value.FirstOrDefault() ?? "";

            switch (contentType)
            {
                case var ctype when ctype.StartsWith("application/json"):
                    return ParseJson<T>(content, call.SerializerOptions);
                case "application/pdf":
                    if (typeof(T) != typeof(byte[]))
                        throw new InvalidOperationException("Only output type of 'byte[]' is supported for 'application/pdf'.");

                    return new CallData<T>((T)(object)content.ToArray());
                default:
                    if (typeof(T) != typeof(string))
                        throw new InvalidOperationException($"Only output type of 'string' is supported for '{contentType}'.");

                    return new CallData<T>((T)(object)Encoding.Default.GetString(content));
            }
        }

        private static CallData<T> ParseJson<T>(byte[] content, JsonSerializerOptions options)
        {
            var asString = Encoding.Default.GetString(content);

            try
            {
                var asObject = JsonSerializer.Deserialize<T>(asString, options);

                if(asObject == null)
                    throw new InvalidOperationException($"Cannot serialize '{asString}' as type '{typeof(T)}': type resolved as null");

                return new CallData<T>(asObject);
            }
            catch (JsonException)
            {
                throw new InvalidOperationException($"Cannot serialize '{asString}' as type '{typeof(T)}'");
            }
        }
    }
}
