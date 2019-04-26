namespace Protacon.NetCore.WebApi.TestUtil
{
    public class CallData<T>
    {
        internal readonly T Data;

        public CallData(T data)
        {
            Data = data;
        }
    }
}
