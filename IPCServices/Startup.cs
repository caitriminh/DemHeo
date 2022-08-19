using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using IPC.SignalR;
using Microsoft.Extensions.FileProviders;
using IPCServices.Domain.Extends;
using IPCServices.Services.Interface;
using IPCServices.Services.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IPCServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddSignalR();
            services.AddControllersWithViews();
            services.AddDirectoryBrowser();
            services.AddControllers();

            services.AddDbContext<Models.AppContext>(options => options.UseSqlServer(Configuration.GetConnectionString("WineCP")));
            services.AddScoped<IDapperORM, DapperORM>();
            services.AddTransient<IDataRepository, DataRepository>();
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseStaticFiles(); // For the wwwroot folder.

            // using Microsoft.Extensions.FileProviders;
            // using System.IO;
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "UpdateFile")),
                RequestPath = "/UpdateFile",
                EnableDirectoryBrowsing = true
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SWINE CP API V1");
            });
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

                endpoints.MapHub<ApplicationHub>("/applicationHub");
            });

            SetConfigurationString();
        }
        private static void SetConfigurationString()
        {
            SqlHelper.ConnectionString = Configuration.GetConnectionString("SWineCP");
        }
    }
}
