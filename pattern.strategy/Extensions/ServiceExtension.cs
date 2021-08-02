using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using pattern.strategy;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace patterns.strategy
{
    [ExcludeFromCodeCoverage]
    public static class ServiceExtensions
    {
        public static IServiceCollection AddScoppedStrategy<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : IStrategy
            where TImplementation : class, IStrategy

        {
            return AddStrategy<TInterface, TImplementation>(services, ServiceLifetime.Scoped);
        }
        //public static ServiceCollection AddTransientStrategy<TInterface, TImplementation>(this IServiceCollection services)
        //    where TInterface : IStrategy
        //    where TImplementation : class, IStrategy
        //{
        //    return AddStrategy<TInterface, TImplementation>(services, ServiceLifetime.Transient);
        //}
        //public static ServiceCollection AddSingletonStrategy<TInterface, TImplementation>(this IServiceCollection services)
        //    where TInterface : IStrategy
        //    where TImplementation : class, IStrategy
        //{
        //    return AddStrategy<TInterface, TImplementation>(services, ServiceLifetime.Singleton);
        //}
        public static IServiceCollection AddStrategy<TInterface, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
            where TInterface : IStrategy
            where TImplementation : class, IStrategy
        {
            var typeInterface = typeof(TInterface);
            var serviceTypeStrategyContext = typeof(IStrategyContext);
            var implementationTypeStrategyContext = typeof(StrategyContext);

            var methodInfoHandle = typeof(TImplementation).GetMethod("HandleAsync");
            bool validationAttribute = methodInfoHandle
                .CustomAttributes
                .Any(o => o.AttributeType == typeof(ValidatorAttribute));

            services.Add(ServiceDescriptor.Describe(typeInterface, (sp) =>
            {
                var instancia = ActivatorUtilities.GetServiceOrCreateInstance(sp, typeof(TImplementation));

                if (!validationAttribute)
                {
                    return instancia;
                }

                var proxyGenerator = sp.GetRequiredService<IProxyGenerator>();
                var interceptors = sp.GetServices<IAsyncValidatorInterceptor>().ToArray();
                var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), instancia, interceptors);

                if (instancia is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                return proxy;
            }, lifetime));

            var servicesStrategy = services
            .Where(s => s.ServiceType == typeInterface)
            .ToList();

            if (servicesStrategy.Count == 0)
            {
                throw new InvalidOperationException($"Não foi encontrado {typeInterface} " +
                                                     "no ServiceCollection.");
            }
            services.Add(ServiceDescriptor.Describe(serviceTypeStrategyContext, implementationTypeStrategyContext, lifetime));

            return services;
        }
    }
}
