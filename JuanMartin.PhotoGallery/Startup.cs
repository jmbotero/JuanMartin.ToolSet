using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Models.Gallery;
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
using System.Xml.Linq;

namespace JuanMartin.PhotoGallery
{
    public class Startup
    {
        public static string DataLoadConnectionString { get; private set; }
        public static string ConnectionString { get; private set; }
        public static string Version { get; private set; }
        public static bool IsMobile { get; set; }
        public static bool IsSignedIn { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            DataLoadConnectionString = Configuration.GetConnectionString("DataLoadConnection");
            IsSignedIn = false;
            IsMobile = false;
            // set current version as major.minor.build number
            Version = Configuration["Version"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache  `
            services.AddSession();
            services.AddControllersWithViews();
            
            var photoService = new PhotoService(Configuration);

            // Execute command line  image load

            //string path;
            //string connectionString = DataLoadConnectionString;
            //int userId = 1;
            //IEnumerable<Photography> photographies;
            //path = Directory.GetCurrentDirectory() + @"\wwwroot\photos\digital\Tanzania\Kilimanjaro";
            //photographies = photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg", false, userId, "Africa, Tanzania");
            //photoService.AddTags(connectionString, userId, "Kilimanjaro", photographies);
            //path = Directory.GetCurrentDirectory() + @"\wwwroot\photos\digital\chile\glaciar grey";
            //photographies = photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg", false, userId, "South America, Chile");
            //photoService.AddTags(connectionString, userId, "Glaciar Grey", photographies);
            //path = Directory.GetCurrentDirectory() + @"\wwwroot\photos\digital\chile\pampa chilena";
            //photographies = photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg", false, userId, "South America, Chile");
            //photoService.AddTags(connectionString, userId, "Pampa Chilena", photographies);
            //path = Directory.GetCurrentDirectory() + @"\wwwroot\photos\digital\chile\punta arenas";
            //photographies = photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg", false, userId, "South America, Chile");
            //photoService.AddTags(connectionString, userId, "Punta Arenas", photographies);
            //path = Directory.GetCurrentDirectory() + @"\wwwroot\photos\digital\chile\santiago de chile";
            //photographies = photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg", false, userId, "South America, Chile");
            //photoService.AddTags(connectionString, userId, "Santiago de Chile", photographies);
            //path = Directory.GetCurrentDirectory() + @"\wwwroot\photos\digital\chile\torres del paine";
            //photographies = photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg", false, userId, "South America, Chile");
            //photoService.AddTags(connectionString, userId, "Torres del Paine", photographies);
            //path = Directory.GetCurrentDirectory() + @"\wwwroot\photos\digital\utah\Arches";
            //photographies = photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg", false, userId, "United States,  Utah");
            //photoService.AddTags(connectionString, userId, "Arches National Park", photographies);

            services.AddSingleton<IPhotoService>(photoService); 
            services.AddSingleton<IConfiguration>(Configuration);
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
