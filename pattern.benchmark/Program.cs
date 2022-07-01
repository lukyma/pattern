using System;
using System.Threading.Tasks;
using static pattern.benchmark.AopProxyBenchmark;

namespace pattern.benchmark
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Teste teste = new Teste();
            await teste.Teste1();
            //_ = BenchmarkRunner.Run<AopProxyBenchmark>();
            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddScoppedStrategy<IStrategy<Request, Response>, TestStrategy>(true);

            //var serviceProvider = serviceCollection.BuildServiceProvider();

            //var strategy = serviceProvider.GetService<IStrategy<Request, Response>>();

            //var response = strategy.HandleAsync(new Request(), CancellationToken.None).Result;
            Console.ReadLine();
        }
    }
}
