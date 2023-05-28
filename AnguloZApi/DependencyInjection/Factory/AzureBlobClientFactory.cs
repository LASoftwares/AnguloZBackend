using Azure.Storage.Blobs;

namespace AnguloZApi.DependencyInjection.Factory
{
    public class AzureBlobClientFactory : IServiceProviderFactory<BlobContainerClient>
    {
        private readonly string blobConnectionString;
        private readonly string blobContainerName;

        public AzureBlobClientFactory(string blobConnectionString, string blobContainerName)
        {
            this.blobConnectionString = blobConnectionString;
            this.blobContainerName = blobContainerName;
        }
        public BlobContainerClient CreateBuilder(IServiceCollection services)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(this.blobConnectionString);
            return blobServiceClient.GetBlobContainerClient(blobContainerName);
        }

        public IServiceProvider CreateServiceProvider(BlobContainerClient containerBuilder)
        {
            return containerBuilder as IServiceProvider;
        }
    }
}
