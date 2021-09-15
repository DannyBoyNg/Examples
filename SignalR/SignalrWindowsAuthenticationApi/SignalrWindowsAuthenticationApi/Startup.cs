using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalrWindowsAuthenticationApi.Hubs;
using SignalrWindowsAuthenticationApi.WindowsAuth;

namespace SignalrWindowsAuthenticationApi
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
            //CORS
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200");
            }));

            //SignalR
            services
                .AddScoped<IClaimsTransformation, ClaimsTransformer>() //Optional: Set Roles for users
                .AddSingleton<IUserIdProvider, NameUserIdProvider>(); //Windows Authentication for SignalR
            services.AddAuthentication(IISDefaults.AuthenticationScheme); //Windows Authentication Scheme
            services.AddSignalR();

            //Web Api
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

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //SignalR endpoints
                endpoints.MapHub<BroadcastHub>("/chat");
                //Web Api endpoints
                endpoints.MapControllers();
            });
        }
    }
}
