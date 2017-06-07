using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Protacon.NetCore.WebApi.TestUtil
{
    public static class MockExtensions
    {
        public static IServiceCollection RemoveService<TInterface>(this IServiceCollection services)
        {
            return RemoveService(services, typeof(TInterface));
        }

        public static IServiceCollection RemoveService(this IServiceCollection services, Type type)
        {
            var serviceLocation = services
                .Select((item, index) => new { item, index })
                .SingleOrDefault(x => x.item.ServiceType == type)?.index;

            if (serviceLocation == null)
                throw new InvalidOperationException($"Cannot find matching service for '{type}', available services are '{ GetServicesAsString(services) }'");

            services.RemoveAt((int)serviceLocation);

            return services;
        }

        private static string GetServicesAsString(IServiceCollection services)
        {
            return services.Select(x => x.ServiceType.ToString()).Aggregate((a, b) => $"{a}, {b}");
        }
    }
}
