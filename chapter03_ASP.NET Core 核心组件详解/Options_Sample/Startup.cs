using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Options_Sample.Services;

namespace Options_Sample
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
            


            //services.Configure<MyOption>(Configuration.GetSection("myOption"));
            //Ĭ������ʹ��Section myOption
            services.Configure<MyOption>(Configuration.GetSection("myOption"))
                 ;
            //����ΪmyOption2������ʹ��Section myOption2
            services.Configure<MyOption>("myOption2", Configuration.GetSection("myOption2"));

            //services.ConfigureAll<MyOption>(option => {
            //    option.MinAge = 1; //�������е�MinAgeΪ1
            //});

            services.PostConfigure<MyOption>(option =>
            {
                option.MinAge += 1; //Ϊ��С�����1
            });

            

            services.PostConfigure<MyOption>("myOption2", option =>
            {
                option.MinAge += 2; //ΪmyOption2��MinAge��2
            });


            services.PostConfigureAll<MyOption>(option =>
            {
                option.MinAge += 5; //Ϊ����������MyOption��MinAge��5
            });


            //services.AddOptions<MyOption>()
            //        .Bind(Configuration.GetSection("myOption"))
            //        .ValidateDataAnnotations()  //����DataAnnotations��֤
            //        .Validate(option =>         //����ί����֤
            //        {
            //            return option.MinAge >= 0;
            //        }, "MinAge����С��0");

            ////����IValidateOptions��֤
            //services.AddSingleton<IValidateOptions<MyOption>, MyOptionValidation>();




            services.AddSingleton<MyService>();
            services.AddSingleton<MyService2>();
            services.AddScoped<MyService3>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
