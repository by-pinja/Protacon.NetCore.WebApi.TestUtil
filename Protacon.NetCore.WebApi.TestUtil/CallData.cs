using System;
using System.Threading.Tasks;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public class CallData<T>
    {
        private readonly T _data;

        public CallData(T data)
        {
            _data = data;
        }

        public async Task<CallData<T>> Passing(Action<T> asserts)
        {
            asserts.Invoke(_data);
            return await this;
        }

        public TSelect Select<TSelect>(Func<T, TSelect> selector)
        {
            return selector.Invoke(_data);
        }

        public T Select()
        {
            return _data;
        }
    }
}
