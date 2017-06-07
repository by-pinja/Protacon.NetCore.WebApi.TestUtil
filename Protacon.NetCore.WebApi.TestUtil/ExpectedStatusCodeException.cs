using System;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public class ExpectedStatusCodeException : Exception
    {
        public ExpectedStatusCodeException(string message) : base(message)
        {
        }
    }
}