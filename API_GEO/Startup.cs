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
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using API_GEO.Models;
using API_GEO.Librery;
using System.Threading;

namespace API_GEO
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
            services.AddMvc();

            var hostname = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? @"db";
            var password = Environment.GetEnvironmentVariable("SQLSERVER_SA_PASSWORD") ?? "APIGEO2020#";
            var connString = $"server={hostname};Initial Catalog=master;User ID=sa;Password={password};";

            if(!SQLClient.CheckDatabaseExist(connString,"API_GEO_DB"))
            {
                SQLClient.CreateDataBase(connString);
            }
            else
            {
                connString = "";
            }
            
            connString = $"server={hostname};Initial Catalog=API_GEO_DB;User ID=sa;Password={password};";
            
            services.AddDbContext<ApiContext>(options => options.UseSqlServer(connString));

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
        }
    }
}
