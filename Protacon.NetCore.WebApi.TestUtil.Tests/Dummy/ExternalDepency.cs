using System;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Dummy
{
    public class ExternalDepency : IExternalDepency
    {
        public string SomeCall(string argument)
        {
            throw new NotImplementedException();
        }
    }
}
