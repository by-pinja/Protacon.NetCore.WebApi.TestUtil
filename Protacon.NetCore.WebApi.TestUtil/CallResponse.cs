using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Protacon.NetCore.WebApi.TestUtil
{
    // HttpResponse is wrapped because we don't want add extension methods
    // to HttpResponseMessage directly because
    // - they pollute intellisense and user may use those extensions as crazy ways.
    // - extend chain with possible metadata like ExpectedStatusCode call does.
    // - Support cases where same call must be executed multiple times like polling certain status code.
    public class Call
    {
        internal HttpStatusCode? ExpectedStatusCode { get; set; }

        internal Task<HttpResponseMessage> HttpTask { get; }
        private readonly Func<Task<HttpResponseMessage>> _httpTaskFactory;

        public Call(Func<Task<HttpResponseMessage>> httpCall)
        {
            _httpTaskFactory = httpCall;
            HttpTask = httpCall.Invoke();
        }

        internal Task<Call> Clone()
        {
            return Task.Run(() => new Call(_httpTaskFactory));
        }
    }
}
