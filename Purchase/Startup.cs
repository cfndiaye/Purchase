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
using Microsoft.OpenApi.Models;
using Purchase.Models;
using Purchase.Services.Contract;
using Purchase.Services.Implementation;

namespace Purchase
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public string MyPolicy { get; set; } = "_myPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();
            services.Configure<PurchaseStoreDatabaseSettings>(Configuration.GetSection("PurchaseStoreDatabase"));
            services.AddSingleton<VendorService>();
            services.AddSingleton<OrderService>();
            services.AddCors(option => {
                option.AddPolicy(name: MyPolicy, builder => {
                    //builder.WithOrigins("https://localhost:44331");
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    //builder.AllowAnyHeader().WithMethods("GET, PATCH, DELETE, PUT, POST, OPTIONS");
                    builder.AllowAnyHeader().WithHeaders("content-type");
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Purchase", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(MyPolicy);
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Purchase v1"));

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
