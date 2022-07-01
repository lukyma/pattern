using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using pattern.strategy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace patterns.strategy
{
    [ExcludeFromCodeCoverage]
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add transient strategy
        /// </summary>
        /// <typeparam name="TInterface">Interface the type IStrategy<,> </typeparam>
        /// <typeparam name="TImplementation">Implementation of IStrategy<,> </typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTransientStrategy<TInterface, TImplementation>(this IServiceCollection services, bool withInterceptor = false)
            where TInterface : IStrategy
            where TImplementation : class, IStrategy

        {
            return AddStrategy<TInterface, TImplementation>(services, ServiceLifetime.Transient, withInterceptor);
        }

        /// <summary>
        /// Add singleton strategy
        /// </summary>
        /// <typeparam name="TInterface">Interface the type IStrategy<,> </typeparam>
        /// <typeparam name="TImplementation">Implementation of IStrategy<,> </typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSingletonStrategy<TInterface, TImplementation>(this IServiceCollection services, bool withInterceptor = false)
            where TInterface : IStrategy
            where TImplementation : class, IStrategy

        {
            return AddStrategy<TInterface, TImplementation>(services, ServiceLifetime.Singleton, withInterceptor);
        }

        /// <summary>
        /// Add scoped strategy
        /// </summary>
        /// <typeparam name="TInterface">Interface the type IStrategy<,> </typeparam>
        /// <typeparam name="TImplementation">Implementation of IStrategy<,> </typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoppedStrategy<TInterface, TImplementation>(this IServiceCollection services, bool withInterceptor = false)
            where TInterface : IStrategy
            where TImplementation : class, IStrategy

        {
            return AddStrategy<TInterface, TImplementation>(services, ServiceLifetime.Scoped, withInterceptor);
        }

        /// <summary>
        /// Add Service with proxy interceptor and with ServiceLifeTime Singleton
        /// </summary>
        /// <typeparam name="TInterface">Interface service type</typeparam>
        /// <typeparam name="TImplementation">Implementation service type</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSingletonProxyInterceptor<TInterface, TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            return AddProxyInterceptor<TInterface, TImplementation>(services, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Add Service with proxy interceptor and with ServiceLifeTime Scoped
        /// </summary>
        /// <typeparam name="TInterface">Interface service type</typeparam>
        /// <typeparam name="TImplementation">Implementation service type</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedProxyInterceptor<TInterface, TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            return AddProxyInterceptor<TInterface, TImplementation>(services, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Add Service with proxy interceptor and with ServiceLifeTime Singleton
        /// </summary>
        /// <typeparam name="TInterface">Interface service type</typeparam>
        /// <typeparam name="TImplementation">Implementation service type</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTransientProxyInterceptor<TInterface, TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            return AddProxyInterceptor<TInterface, TImplementation>(services, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Add Service with proxy interceptor and ServiceLifetime
        /// </summary>
        /// <typeparam name="TInterface">Interface service type</typeparam>
        /// <typeparam name="TImplementation">Implementation service type</typeparam>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddProxyInterceptor<TInterface, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
            where TImplementation : class

        {
            var typeInterface = typeof(TInterface);

            var methodsInterceptors = typeof(TImplementation)
                .GetMethods()
                .Where(o => o.CustomAttributes.Any(p => p.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))));



            if (!services.Any(o => o.ServiceType == typeof(IProxyGenerator)))
            {
                services.Add(ServiceDescriptor.Describe(typeof(IProxyGenerator), typeof(ProxyGenerator), ServiceLifetime.Singleton));
            }

            services.Add(ServiceDescriptor.Describe(typeInterface, (sp) =>
            {
                object instancia = ActivatorUtilities.GetServiceOrCreateInstance(sp, typeof(TImplementation));

                if (!methodsInterceptors.Any())
                {
                    return instancia;
                }

                IList<IAsyncInterceptor> asyncInterceptors = new List<IAsyncInterceptor>();

                foreach (var item in methodsInterceptors)
                {
                    string nameMethod = item.Name;
                    var interceptorAttributes = item.GetCustomAttributes(false).Where(p => p.GetType().IsSubclassOf(typeof(InterceptorAttribute))).Select(o => (InterceptorAttribute)o);
                    foreach (var item2 in interceptorAttributes)
                    {
                        item2.ServiceProvider = sp;
                        AsyncInterceptorBaseAttribute interceptor = Activator.CreateInstance<AsyncInterceptorBaseAttribute>();
                        interceptor.MethodName = nameMethod;
                        interceptor.Order = item2.Order;
                        interceptor.TypeClass = typeof(TImplementation).Name;
                        interceptor.Interceptor = item2;
                        var teste = item.GetType();
                        asyncInterceptors.Add(interceptor);
                    }
                }

                IProxyGenerator proxyGenerator = sp.GetService<IProxyGenerator>();
                IAsyncInterceptor[] interceptors = asyncInterceptors.ToArray();
                object proxy = proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), instancia, interceptors);

                if (instancia is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                return proxy;
            }, lifetime));

            return services;
        }
        public static IServiceCollection AddStrategy<TInterface, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime, bool withInterceptor)
            where TInterface : IStrategy
            where TImplementation : class, IStrategy
        {
            var typeInterface = typeof(TInterface);
            var serviceTypeStrategyContext = typeof(IStrategyContext);
            var implementationTypeStrategyContext = typeof(StrategyContext);

            if (withInterceptor)
            {
                services.AddProxyInterceptor<TInterface, TImplementation>(lifetime);
            }
            else
            {
                services.Add(ServiceDescriptor.Describe(typeInterface, (sp) =>
                {
                    return ActivatorUtilities.GetServiceOrCreateInstance(sp, typeof(TImplementation));
                }, lifetime));
            }

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
