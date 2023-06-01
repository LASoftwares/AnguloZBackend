using AnguloZApi.DependencyInjection;
using AnguloZApi.Repositories;
using AnguloZApi.Services.Abstractions;
using AnguloZApi.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AnguloZApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("MongoDb");
            services.AddMongoDb(connectionString);
            services.AddBlobClient(config["AzureBlobs:ConnectionString"], config["AzureBlobs:ContainerName"]);
            services.AddScoped<IProjetoArchRepository, ProjetoArchRepository>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app)
        {
         
            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader().Build());

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
