using MongoDB.Driver;

namespace AnguloZApi.DependencyInjection.Factory
{
    public class MongoDatabaseFactory : IServiceProviderFactory<IMongoDatabase>
    {
        private readonly string _connectionString;
        public MongoDatabaseFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public IMongoDatabase CreateBuilder(IServiceCollection services)
        {
            IMongoClient client;
            try
            {
                client = new MongoClient(_connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting to MongoDb", ex);
            }
            return client.GetDatabase("AnguloZ");
        }

        public IServiceProvider CreateServiceProvider(IMongoDatabase containerBuilder)
        {
            return containerBuilder as IServiceProvider;
        }
    }
}
