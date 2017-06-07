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
            if(example == null)
                throw new InvalidOperationException();

            return NoContent();
        }

        [HttpGet("/file/")]
        public IActionResult File()
        {
            var stream = new MemoryStream(new byte[]{ 1, 2, 3, 4 });
            return new FileStreamResult(stream, "application/pdf");
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
