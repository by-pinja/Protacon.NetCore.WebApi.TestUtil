using System;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public class CallData<T>
    {
        private readonly T _data;

        public CallData(T data)
        {
            _data = data;
        }

        public CallData<T> Passing(Action<T> asserts)
        {
            asserts.Invoke(_data);
            return this;
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
