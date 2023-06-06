using AnguloZApi.DependencyInjection;
using AnguloZApi.Repositories;
using AnguloZApi.Services.Abstractions;
using AnguloZApi.Services.Implementations;
using CognitiveServices.Translator.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace AnguloZApi.Tests
{

	public class ProjetosTestFixture
	{
		public ProjetosTestFixture()
		{
			IConfigurationBuilder cBuilder = new ConfigurationBuilder();
			cBuilder.Sources.Add(new JsonConfigurationSource
			{
				Path = "appsettings.test.json",
				ReloadOnChange = true
			});
			IConfiguration config = cBuilder.Build();
			var mongoCS = config.GetConnectionString("MongoDb");
			var blobCS = config.GetConnectionString("AzureBlobs:ConnectionString");
			var blobContainer = config.GetConnectionString("AzureBlobs:ContainerName");
			var services = new ServiceCollection();
			services.AddMongoDb(mongoCS);
			services.AddBlobClient(config["AzureBlobs:ConnectionString"], config["AzureBlobs:ContainerName"]);
			services.AddScoped<IProjetoArchRepository, ProjetoRepositoryTestImp>();
			services.AddScoped<IBlobService, BlobService>();
			services.AddScoped<IAuthorizationService, AuthorizationService>();
			services.AddCognitiveServicesTranslator(config);

			this.ServiceProvider = services.BuildServiceProvider();
		}
		public ServiceProvider ServiceProvider { get; }
	}
}