using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using pattern.benchmark.Interceptor;
using pattern.benchmark.Strategy;
using System;
using System.Threading;
using System.Threading.Tasks;
using static pattern.benchmark.AopProxyBenchmark;

namespace pattern.benchmark
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cfg = ManualConfig.CreateEmpty()
                .AddJob(Job.Default.WithPlatform(Platform.X64));
            _ = BenchmarkRunner.Run<AopProxyBenchmark>();
            //Console.ReadLine();
        }
    }
}
