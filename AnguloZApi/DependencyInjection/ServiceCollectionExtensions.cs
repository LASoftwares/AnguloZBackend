using Azure.Storage.Blobs;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AnguloZApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString) 
        {
            var factory = new Factory.MongoDatabaseFactory(connectionString);
            services.AddScoped<IMongoDatabase>(x => factory.CreateBuilder(services));
            return services;
        }
        public static IServiceCollection AddBlobClient(this IServiceCollection services, string connectionString, string blobContainerName)
        {
            var factory = new Factory.AzureBlobClientFactory(connectionString,blobContainerName);
            services.AddScoped<BlobContainerClient>(x => factory.CreateBuilder(services));
            return services;
        }
    }
}
