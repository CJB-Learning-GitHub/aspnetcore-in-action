using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HttpClientFactory_Sample.Clients;
using System.Net.Http;
using HttpClientFactory_Sample.DelegatingHandlers;
using Microsoft.AspNetCore.Http;

namespace HttpClientFactory_Sample
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
            services.AddMvc().AddControllersAsServices();
            services.AddHttpClient();
            services.AddTransient<OrderServiceClient>();


            services.AddHttpClient("orderService", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001"); //���ø���ַ
                client.DefaultRequestHeaders.Add("User-Agent", "NamedOrderServiceClient");    //����Ĭ��ͷ
            });
            services.AddScoped<NamedOrderServiceClient>();


            //services.AddTransient<RequestIdDelegatingHandler>();
            services.AddHttpClient<TypedOrderServiceClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001"); //���ø���ַ
                client.DefaultRequestHeaders.Add("User-Agent", "TypedOrderServiceClient");    //����Ĭ��ͷ
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                UseProxy = false,
                UseCookies = false,
            }).AddHttpMessageHandler(() => new RequestIdDelegatingHandler())
            .SetHandlerLifetime(TimeSpan.FromSeconds(30));//������������

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("hi");
            });
        }
    }
}
