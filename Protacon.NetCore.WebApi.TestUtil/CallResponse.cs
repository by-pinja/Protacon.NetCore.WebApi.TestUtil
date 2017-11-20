﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public class CallResponse
    {
        private readonly HttpResponseMessage _response;
        private HttpStatusCode? _expectedCode;

        public CallResponse(HttpResponseMessage response)
        {
            _response = response;
        }

        public CallResponse ExpectStatusCode(HttpStatusCode code)
        {
            if (_response.StatusCode != code)
            {
                throw new ExpectedStatusCodeException($"Expected statuscode '{code}' but got '{(int)_response.StatusCode}'");
            }

            _expectedCode = code;

            return this;
        }

        public CallResponse HeaderPassing(string header, Action<string> assertsForValue)
        {
            var match = _response.Headers
                .SingleOrDefault(x => x.Key == header);

            if (match.Equals(default(KeyValuePair<string, IEnumerable<string>>)))
                throw new InvalidOperationException($"Header '{header}' not found, available headers are '{HeadersAsReadableList()}'");

            assertsForValue.Invoke(match.Value.Single());

            return this;
        }

        private string HeadersAsReadableList()
        {
            return _response.Headers.Select(x => x.Key.ToString()).Aggregate("", (a, b) => $"{a}, {b}");
        }

        public CallData<T> WithContentOf<T>()
        {
            var code = (int)_response.StatusCode;

            if ((code > 299 || code < 199) && code != (int?)_expectedCode)
                throw new ExpectedStatusCodeException(
                    $"Tried to get data from non ok statuscode response, expected status is '2xx' or '{_expectedCode}' but got '{code}' with content '{_response.Content.ReadAsStringAsync().Result}'");

            if (!_response.Content.Headers.Contains("Content-Type"))
                throw new InvalidOperationException("Response didn't contain any 'Content-Type'. Reason may be that you didn't return anything?");

            var contentType = _response.Content.Headers.Single(x => x.Key == "Content-Type").Value.FirstOrDefault() ?? "";

            switch (contentType)
            {
                case var ctype when ctype.StartsWith("application/json"):
                    return ParseJson<T>();
                case "application/pdf":
                    if(typeof(T) != typeof(byte[]))
                        throw new InvalidOperationException("Only output type of 'byte[]' is supported for 'application/pdf'.");

                    var data = (object)_response.Content.ReadAsByteArrayAsync().Result.ToArray();
                    return new CallData<T>((T)data);
                default:
                    if (typeof(T) != typeof(string))
                        throw new InvalidOperationException($"Only output type of 'string' is supported for '{contentType}'.");

                    var result = (object)_response.Content.ReadAsStringAsync().Result;
                    return new CallData<T>((T)result);
            }
        }

        private CallData<T> ParseJson<T>()
        {
            try
            {
                var asObject = JsonConvert.DeserializeObject<T>(_response.Content.ReadAsStringAsync().Result);
                return new CallData<T>(asObject);
            }
            catch (JsonSerializationException)
            {
                throw new InvalidOperationException($"Cannot serialize '{_response.Content.ReadAsStringAsync().Result}' as type '{typeof(T)}'");
            }
        }
    }
}
