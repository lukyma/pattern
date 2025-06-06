using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pattern.sample.api.Service;
using pattern.sample.api.StrategyHandler;
using pattern.sample.api.StrategyHandler.Validator;
using pattern.proxy;

namespace pattern.sample.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IValidationErrors, ValidationErrors>();

            //services.AddProxyInterceptor<ITestService, TestService>(ServiceLifetime.Scoped);

            services.AddScopedProxyInterceptor<ITestService, TestService>();
            services.AddScoppedProxy<IProxy<TestStrategyRequest, TestStrategyResponse>, TestStrategy>(true);
            //services.AddScoppedStrategy<IStrategy<TestStrategyRequest2, TestStrategyResponse2>, TestStrategy2>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
