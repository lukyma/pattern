﻿using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using pattern.strategy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace patterns.strategy
{
    [ExcludeFromCodeCoverage]
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add scoped strategy
        /// </summary>
        /// <typeparam name="TInterface">Interface the type IStrategy<,> </typeparam>
        /// <typeparam name="TImplementation">Implementation of IStrategy<,> </typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScoppedStrategy<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : IStrategy
            where TImplementation : class, IStrategy

        {
            return AddStrategy<TInterface, TImplementation>(services, ServiceLifetime.Scoped);
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
                .Where(o => o.CustomAttributes.Any(p => p.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))))
                .SelectMany(o => o.GetCustomAttributes(false).Where(p => p.GetType().IsSubclassOf(typeof(InterceptorAttribute))).Select(o => o))
                .Select(o => (InterceptorAttribute)o);

            if (!services.Any(o => o.ServiceType == typeof(IProxyGenerator)))
            {
                services.Add(ServiceDescriptor.Describe(typeof(IProxyGenerator), typeof(ProxyGenerator), ServiceLifetime.Singleton));
            }

            IList<object> attributesInterceptor = new List<object>();

            if (methodsInterceptors.Any())
            {
                foreach (var item in methodsInterceptors)
                {
                    services.Add(ServiceDescriptor.Describe(typeof(IAsyncInterceptor), (sp) =>
                    {
                        PropertyInfo typeClass = item.GetType().GetProperty("TypeClass", BindingFlags.NonPublic | BindingFlags.Instance);
                        string nameClass = typeof(TImplementation).Name;
                        if (string.IsNullOrEmpty((string)typeClass.GetValue(item)))
                        {
                            typeClass.SetValue(item, nameClass);
                        }
                        item.GetType().GetProperty("ServiceProvider", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(item, sp);
                        AsyncInterceptorBaseAttribute interceptor = Activator.CreateInstance<AsyncInterceptorBaseAttribute>();
                        interceptor.Order = item.Order;
                        interceptor.TypeClass = nameClass;
                        interceptor.Interceptor = item;
                        return interceptor;
                    }, lifetime));
                }
            }


            services.Add(ServiceDescriptor.Describe(typeInterface, (sp) =>
            {
                object instancia = ActivatorUtilities.GetServiceOrCreateInstance(sp, typeof(TImplementation));

                if (!methodsInterceptors.Any())
                {
                    return instancia;
                }

                IProxyGenerator proxyGenerator = sp.GetService<IProxyGenerator>();
                IAsyncInterceptor[] interceptors = sp.GetServices<IAsyncInterceptor>()
                .Where(o => ((AsyncInterceptorBaseAttribute)o)
                           .GetType()
                           .GetProperty("TypeClass", BindingFlags.NonPublic | BindingFlags.Instance)
                           .GetValue(o) as string == instancia.GetType().Name)
                .OrderBy(o => ((AsyncInterceptorBaseAttribute)o).Order)
                .ToArray();
                object proxy = proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), instancia, interceptors);

                if (instancia is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                return proxy;
            }, lifetime));

            return services;
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

            var methodsInterceptors = typeof(TImplementation)
                .GetMethods()
                .Where(o => o.CustomAttributes.Any(p => p.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))))
                .SelectMany(o => o.GetCustomAttributes(false).Where(p => p.GetType().IsSubclassOf(typeof(InterceptorAttribute))).Select(o => o))
                .Select(o => (InterceptorAttribute)o);

            if (!services.Any(o => o.ServiceType == typeof(IProxyGenerator)))
            {
                services.Add(ServiceDescriptor.Describe(typeof(IProxyGenerator), typeof(ProxyGenerator), ServiceLifetime.Singleton));
            }

            IList<object> attributesInterceptor = new List<object>();

            if (methodsInterceptors.Any())
            {
                foreach (var item in methodsInterceptors)
                {
                    services.Add(ServiceDescriptor.Describe(typeof(IAsyncInterceptor), (sp) =>
                    {
                        PropertyInfo typeClass = item.GetType().GetProperty("TypeClass", BindingFlags.NonPublic | BindingFlags.Instance);
                        string nameClass = typeof(TImplementation).Name;
                        if (string.IsNullOrEmpty((string)typeClass.GetValue(item)))
                        {
                            typeClass.SetValue(item, nameClass);
                        }
                        item.GetType().GetProperty("ServiceProvider", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(item, sp);
                        AsyncInterceptorBaseAttribute interceptor = Activator.CreateInstance<AsyncInterceptorBaseAttribute>();
                        interceptor.Order = item.Order;
                        interceptor.TypeClass = nameClass;
                        interceptor.Interceptor = item;
                        return interceptor;
                    }, lifetime));
                }
            }


            services.Add(ServiceDescriptor.Describe(typeInterface, (sp) =>
            {
                object instancia = ActivatorUtilities.GetServiceOrCreateInstance(sp, typeof(TImplementation));

                if (!methodsInterceptors.Any())
                {
                    return instancia;
                }

                IProxyGenerator proxyGenerator = sp.GetService<IProxyGenerator>();
                IAsyncInterceptor[] interceptors = sp.GetServices<IAsyncInterceptor>()
                .Where(o => ((AsyncInterceptorBaseAttribute)o)
                           .GetType()
                           .GetProperty("TypeClass", BindingFlags.NonPublic | BindingFlags.Instance)
                           .GetValue(o) as string == instancia.GetType().Name)
                .OrderBy(o => ((AsyncInterceptorBaseAttribute)o).Order)
                .ToArray();
                object proxy = proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), instancia, interceptors);

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
