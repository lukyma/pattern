using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using pattern.benchmark.Interceptor;
using pattern.benchmark.Strategy;
using patterns.strategy;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace pattern.benchmark
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [DryJob]
    public class AopProxyBenchmark
    {
        [Benchmark]
        public async Task WithoutProxyGenerator()
        {
            var strategy = new TestStrategy();
            await strategy.HandleAsync(new Request(), CancellationToken.None);
        }

        [Benchmark]
        public async Task WithDefaultProxyGenerator()
        {
            var proxyGenerator = new ProxyGenerator();
            var interceptor = new TestInterceptor();
            var strategy = new TestStrategy();
            proxyGenerator.CreateInterfaceProxyWithTarget<IStrategy<Request, Response>>(strategy, interceptor);

            await strategy.HandleAsync(new Request(), CancellationToken.None);
        }

        [Benchmark]
        public async Task WithCustomProxyGenerator()
        {
            var proxyGenerator = new ProxyGenerator();
            var interceptor = new TestInterceptorAttribute();
            var strategy = new TestStrategy();
            proxyGenerator.CreateInterfaceProxyWithTarget<IStrategy<Request, Response>>(strategy, interceptor);

            await strategy.HandleAsync(new Request(), CancellationToken.None);
        }

        [Benchmark]
        public async Task WithoutProxyGeneratorDependencyInjector()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoppedStrategy<IStrategy<Request, Response>, TestStrategy>(false);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var strategy = serviceProvider.GetService<IStrategy<Request, Response>>();

            await strategy.HandleAsync(new Request(), CancellationToken.None);
        }

        [Benchmark]
        public async Task WithProxyGeneratorDependencyInjector()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoppedStrategy<IStrategy<Request, Response>, TestStrategy>(true);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var strategy = serviceProvider.GetService<IStrategy<Request, Response>>();

            await strategy.HandleAsync(new Request(), CancellationToken.None);
        }

        [Benchmark]
        public async Task Test()
        {

        }

        [PSerializable]
        public class TestPostSharpAttribute : OnMethodBoundaryAspect
        {
            public override void OnEntry(MethodExecutionArgs args)
            {
                base.OnEntry(args);
            }
        }
        public class Teste
        {
            [TestPostSharp]
            public async Task Teste1()
            {

            }
        }
    }
}
