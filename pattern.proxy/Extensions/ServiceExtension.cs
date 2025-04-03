using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using pattern.proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace pattern.proxy
{
    [ExcludeFromCodeCoverage]
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add transient Proxy
        /// </summary>
        /// <typeparam name="TInterface">Interface the type IProxy<,> </typeparam>
        /// <typeparam name="TImplementation">Implementation of IProxy<,> </typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTransientProxy<TInterface, TImplementation>(this IServiceCollection services, bool withInterceptor = false)
            where TInterface : IProxy
            where TImplementation : class, IProxy

        {
            return AddProxy<TInterface, TImplementation>(services, ServiceLifetime.Transient, withInterceptor);
        }

        /// <summary>
        /// Add singleton Proxy
        /// </summary>
        /// <typeparam name="TInterface">Interface the type IProxy<,> </typeparam>
        /// <typeparam name="TImplementation">Implementation of IProxy<,> </typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSingletonProxy<TInterface, TImplementation>(this IServiceCollection services, bool withInterceptor = false)
            where TInterface : IProxy
            where TImplementation : class, IProxy

        {
            return AddProxy<TInterface, TImplementation>(services, ServiceLifetime.Singleton, withInterceptor);
        }

        /// <summary>
        /// Add scoped Proxy
        /// </summary>
        /// <typeparam name="TInterface">Interface the type IProxy<,> </typeparam>
        /// <typeparam name="TImplementation">Implementation of IProxy<,> </typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoppedProxy<TInterface, TImplementation>(this IServiceCollection services, bool withInterceptor = false)
            where TInterface : IProxy
            where TImplementation : class, IProxy

        {
            return AddProxy<TInterface, TImplementation>(services, ServiceLifetime.Scoped, withInterceptor);
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
                        var interceptor = (AsyncInterceptorBaseAttribute)asyncInterceptors.FirstOrDefault(o => ((AsyncInterceptorBaseAttribute)o).InterceptorAttributeName == item2.GetType().Name);
                        if (interceptor == null)
                        {
                            interceptor = Activator.CreateInstance<AsyncInterceptorBaseAttribute>();
                            interceptor.InterceptorAttributeName = item2.GetType().Name;
                            interceptor.InterceptorInfos.Add(new InterceptorInfo
                            {
                                Interceptor = item2,
                                MethodName = string.Join('|', item.GetParameters().Select(o => o.Name)) + nameMethod,
                                Order = item2.Order,
                                TypeClass = typeof(TImplementation).Name
                            });
                            asyncInterceptors.Add(interceptor);
                        }
                        else
                        {
                            interceptor.InterceptorInfos.Add(new InterceptorInfo
                            {
                                Interceptor = item2,
                                MethodName = string.Join('|', item.GetParameters().Select(o => o.Name)) + nameMethod,
                                Order = item2.Order,
                                TypeClass = typeof(TImplementation).Name
                            });
                        }
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
        public static IServiceCollection AddProxy<TInterface, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime, bool withInterceptor)
            where TInterface : IProxy
            where TImplementation : class, IProxy
        {
            var typeInterface = typeof(TInterface);
            var serviceTypeProxyContext = typeof(IProxyContext);
            var implementationTypeProxyContext = typeof(ProxyContext);

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

            var servicesProxy = services
            .Where(s => s.ServiceType == typeInterface)
            .ToList();

            if (servicesProxy.Count == 0)
            {
                throw new InvalidOperationException($"Não foi encontrado {typeInterface} " +
                                                     "no ServiceCollection.");
            }
            services.Add(ServiceDescriptor.Describe(serviceTypeProxyContext, implementationTypeProxyContext, lifetime));

            return services;
        }
    }
}
