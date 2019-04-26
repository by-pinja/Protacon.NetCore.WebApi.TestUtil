using System;
using System.Threading.Tasks;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public static class CallDataExtensions
    {
        public static async Task<CallData<T>> Passing<T>(this Task<CallData<T>> dataWrapperTask, Action<T> asserts)
        {
            var wrappedData = await dataWrapperTask;
            asserts(wrappedData.Data);
            return wrappedData;
        }

        public static async Task<TSelect> Select<T, TSelect>(this Task<CallData<T>> dataWrapperTask, Func<T, TSelect> selector)
        {
            var wrappedData = await dataWrapperTask;
            return selector.Invoke(wrappedData.Data);
        }

        public static async Task<T> Select<T>(this Task<CallData<T>> dataWrapperTask)
        {
            var data = await dataWrapperTask;
            return data.Data;
        }
    }
}
