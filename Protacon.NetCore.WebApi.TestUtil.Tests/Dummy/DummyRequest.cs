namespace Protacon.NetCore.WebApi.TestUtil.Tests.Dummy
{
    public class DummyRequest
    {
        public string Value { get; set; }
        public CustomTestObject AnotherValue { get; set; } = new CustomTestObject("asdasd");
    }
}