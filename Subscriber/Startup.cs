using PubSub;

namespace Subscriber
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private IWebHostEnvironment HostingEnvironment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddPubSub(HostingEnvironment.ApplicationName);
        }

        // This method gets called by the runtime and is used to configure the HTTP request pipeline.  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use routing middleware to match incoming HTTP requests to routes  
            app.UseRouting();

            // Use authorization and authentication middleware if needed  
            // app.UseAuthentication();  
            // app.UseAuthorization();  

            // Configure the request pipeline to use endpoints, mapping controllers  
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Map attribute-routed controllers  
                                            // You can also map other types of endpoints like Razor pages, gRPC, SignalR, etc.  
            });
        }
    }
}