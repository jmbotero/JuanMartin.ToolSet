using JuanMartin.Kernel.Adapters;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace JuanMartin.PhotoGallery
{
    public class Startup
    {
        public static string ConnectionString { get; private set; }
        public static bool IsSignedIn { get; set; } 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IsSignedIn = false;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache  `
            services.AddSession();
            services.AddControllersWithViews();
            
            var photoService = new PhotoService(Configuration);

            services.AddSingleton<IPhotoService>(photoService);
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //var path = Directory.GetCurrentDirectory() + @"\wwwroot\photos.lnk";
            //PhotoService.LoadPhotographies(new AdapterMySql(ConnectionString), path, ".jpg", true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Gallery/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
               app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
