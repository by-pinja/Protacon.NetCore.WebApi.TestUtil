using System;
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
    }
}
