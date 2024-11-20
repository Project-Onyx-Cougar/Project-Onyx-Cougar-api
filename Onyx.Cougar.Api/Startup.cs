using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Onyx.Cougar.Data;
using Onyx.Cougar.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace onyx.cougar.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string authority = Configuration["Auth0:Authority"]
                ?? throw new ArgumentNullException("Auth0:Authority");

            string audience = Configuration["Auth0:Audience"]
                ?? throw new ArgumentNullException("Auth0:Audience");

            services.AddControllers();

            services.AddAuthentication(options =>
                {   
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
.               AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                });

            // Add authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("delete:catalog", policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("scope", "delete:catalog"));
            });



            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen();

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName); // Use fully qualified names for all schemas
            });


            services.AddDbContext<StoreContext>(options =>
                options.UseSqlite("Data Source=../Registrar.sqlite",
                b => b.MigrationsAssembly("Onyx.Cougar.Api"))
            );
            services.AddCors(options => 
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            //services.AddEndpointsApiExplorer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Onyx.Cougar.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
