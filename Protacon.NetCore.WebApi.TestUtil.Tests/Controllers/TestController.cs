using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Controllers
{
    public class TestController: Controller
    {
        private readonly IExternalDepency _depency;

        public TestController(IExternalDepency depency)
        {
            _depency = depency;
        }

        [HttpGet("/returnthree/")]
        public IActionResult Get()
        {
            return Ok(3);
        }

        [HttpGet("/returnobject/")]
        public IActionResult GetObject()
        {
            return Ok(new DummyRequest
            {
                Value = "5",
                AnotherValue = new CustomTestObject("default_value")
            });
        }

        [HttpPost("/returnsame/")]
        public IActionResult Post([FromBody] DummyRequest arg)
        {
            return Ok(arg);
        }

        [HttpPut("/returnsame/")]
        public IActionResult Put([FromBody] DummyRequest arg)
        {
            return Ok(arg);
        }

        [HttpDelete("/something/{arg}")]
        public IActionResult Delete(string arg)
        {
            return NoContent();
        }

        [HttpGet("/external/{id}")]
        public IActionResult GetFromExternal(string id)
        {
            return Ok(new DummyRequest
            {
                Value = _depency.SomeCall(id)
            });
        }

        [HttpGet("/headertest/")]
        public IActionResult Header([FromHeader] string example)
        {
            if (example == null)
                throw new InvalidOperationException();

            return NoContent();
        }

        [HttpGet("/headertest/response-headers/")]
        public IActionResult ResponseHeaders()
        {
            Response.Headers.Add("X-Powered-By", "pinja");
            Response.Headers.Add("Content-Language", "fi_FI");

            return NoContent();
        }

        [HttpGet("/file/")]
        public IActionResult File()
        {
            var stream = new MemoryStream(new byte[]{ 1, 2, 3, 4 });
            return new FileStreamResult(stream, "application/pdf");
        }

        [HttpGet("/errorcontent/")]
        public IActionResult ErrorWithContent()
        {
            return NotFound(new DummyRequest
            {
                Value = "error"
            });
        }

        [HttpPatch("/patchtestroute/")]
        public IActionResult PatchTestRoute([FromBody] DummyRequest request)
        {
            var value = new DummyRequest
            {
                Value = "Not modified"
            };

            if(request.Value != default)
                value.Value = request.Value;

            return Ok(value);
        }

        public class PatchRequest
        {
            public int? TestPatchValue { get; set; }
        }

        [HttpGet("/page/")]
        public IActionResult Page()
        {
            var content = "<html><body><h1>Hello World</h1><p>Some text</p></body></html>";

            return new ContentResult
            {
                Content = content,
                ContentType = "text/html",
            };
        }
    }
}
