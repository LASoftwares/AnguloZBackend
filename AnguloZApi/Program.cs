using AnguloZApi.DependencyInjection;
using AnguloZApi.Repositories;
using AnguloZApi.Services.Abstractions;
using AnguloZApi.Services.Implementations;
using CognitiveServices.Translator;
using CognitiveServices.Translator.Extension;

namespace AnguloZApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("MongoDb");
            builder.Services.AddMongoDb(connectionString);
            builder.Services.AddBlobClient(config["AzureBlobs:ConnectionString"], config["AzureBlobs:ContainerName"]);
            builder.Services.AddCognitiveServicesTranslator(config);
            builder.Services.AddScoped<IProjetoArchRepository, ProjetoArchRepository>();
            builder.Services.AddScoped<IBlobService, BlobService>();
            builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(builder =>
            {
                builder.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.UseCors(x => x.AllowAnyOrigin());
            app.MapControllers();

            app.Run();
        }
    }
}