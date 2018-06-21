using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public class Call
    {
        private HttpStatusCode? _expectedCode;
        private Func<HttpResponseMessage> _httpCall;
        private readonly Lazy<HttpResponseMessage> _response;

        public Call(Func<HttpResponseMessage> httpCall)
        {
            _response = new Lazy<HttpResponseMessage>(httpCall);
            _httpCall = httpCall;
        }

        public Call ExpectStatusCode(HttpStatusCode code)
        {
            if (_response.Value.StatusCode != code)
            {
                throw new ExpectedStatusCodeException($"Expected statuscode '{code}' but got '{(int)_response.Value.StatusCode}'");
            }

            _expectedCode = code;

            return this;
        }

        public Call HeaderPassing(string header, Action<string> assertsForValue)
        {
            var match = _response.Value.Headers
                .SingleOrDefault(x => x.Key == header);

            if (match.Equals(default(KeyValuePair<string, IEnumerable<string>>)))
                throw new InvalidOperationException($"Header '{header}' not found, available headers are '{HeadersAsReadableList()}'");

            assertsForValue.Invoke(match.Value.Single());

            return this;
        }

        private string HeadersAsReadableList()
        {
            return _response.Value.Headers.Select(x => x.Key.ToString()).Aggregate("", (a, b) => $"{a}, {b}");
        }

        public CallData<T> WithContentOf<T>()
        {
            var code = (int)_response.Value.StatusCode;

            if ((code > 299 || code < 199) && code != (int?)_expectedCode)
                throw new ExpectedStatusCodeException(
                    $"Tried to get data from non ok statuscode response, expected status is '2xx' or '{_expectedCode}' but got '{code}' with content '{_response.Value.Content.ReadAsStringAsync().Result}'");

            if (!_response.Value.Content.Headers.Contains("Content-Type"))
                throw new InvalidOperationException("Response didn't contain any 'Content-Type'. Reason may be that you didn't return anything?");

            var contentType = _response.Value.Content.Headers.Single(x => x.Key == "Content-Type").Value.FirstOrDefault() ?? "";

            switch (contentType)
            {
                case var ctype when ctype.StartsWith("application/json"):
                    return ParseJson<T>();
                case "application/pdf":
                    if(typeof(T) != typeof(byte[]))
                        throw new InvalidOperationException("Only output type of 'byte[]' is supported for 'application/pdf'.");

                    var data = (object)_response.Value.Content.ReadAsByteArrayAsync().Result.ToArray();
                    return new CallData<T>((T)data);
                default:
                    if (typeof(T) != typeof(string))
                        throw new InvalidOperationException($"Only output type of 'string' is supported for '{contentType}'.");

                    var result = (object)_response.Value.Content.ReadAsStringAsync().Result;
                    return new CallData<T>((T)result);
            }
        }

        internal Call Clone()
        {
            return new Call(_httpCall);
        }

        private CallData<T> ParseJson<T>()
        {
            try
            {
                var asObject = JsonConvert.DeserializeObject<T>(_response.Value.Content.ReadAsStringAsync().Result);
                return new CallData<T>(asObject);
            }
            catch (JsonSerializationException)
            {
                throw new InvalidOperationException($"Cannot serialize '{_response.Value.Content.ReadAsStringAsync().Result}' as type '{typeof(T)}'");
            }
        }
    }
}
