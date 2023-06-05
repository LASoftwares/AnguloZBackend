using AnguloZApi.APIModels.ProjetosModel;
using AnguloZApi.Controllers;
using AnguloZApi.Repositories;
using AnguloZApi.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;

namespace AnguloZApi.Tests
{
    public class ProjetosTest : IClassFixture<ProjetosTestFixture>
    {
        private readonly ProjetosController _controller;
        private readonly IProjetoArchRepository _repository;
        private Guid? UserSecret;
        public ProjetosTest(ProjetosTestFixture fixture)
        {
            var blobService = fixture.ServiceProvider.GetService<IBlobService>();
            var authorizationService = fixture.ServiceProvider.GetService<IAuthorizationService>();
            _repository = fixture.ServiceProvider.GetService<IProjetoArchRepository>();
            var mongoDb = fixture.ServiceProvider.GetService<IMongoDatabase>();
            var authController = new AuthController(mongoDb);
            _controller = new ProjetosController(_repository, blobService, authorizationService);
            GetUserSecret(authController).Wait();
        }

        private async Task GetUserSecret(AuthController auth)
        {
            IActionResult result  = await auth.UserLogin(new APIModels.AuthModels.UserLoginRequest
            {
                Username = "admanguloz",
                Password = "anguloz123"
            });
            if(result is OkObjectResult okObjectResult)
            {
                if(okObjectResult.Value is Guid userSecret)
                {
                    UserSecret = userSecret;
                    return;
                }
            }
            UserSecret = null;
        }
        
        [Fact]
        public async Task CreateProjeto()
        {
            var projetos = await _repository.GetAll();
            var initialCount = projetos.Count();
            ProjetoArquiteturaRequest projeto = new ProjetoArquiteturaRequest
            {
                Titulo = "Teste",
                Descricao = "Esse é um teste automático, favor desconsiderar",
                Categoria = "Teste"
            };
            var result = await _controller.Post(projeto, this.UserSecret ?? Guid.Empty);
            if (result is StatusCodeResult actionResult)
            {
                Assert.Equal(200, actionResult.StatusCode);
            }
            projetos = await _repository.GetAll();
            Assert.True(initialCount < projetos.Count());
        }
        
        [Fact]
        public async Task DeleteProjeto()
        {
            var projetos = await _repository.GetAll();
            var initialCount = projetos.Count();
            var projeto = projetos.FirstOrDefault();
            if(projeto is null)
            {
                Assert.True(false, "Não há projetos para deletar");
            }
            _ = await _controller.Delete(projeto.Id, this.UserSecret ?? Guid.Empty);
            projetos = await _repository.GetAll();
            Assert.True(initialCount > projetos.Count());
        }
        
        [Fact]
        public async Task UpdateProjeto()
        {
            var projetos = await _repository.GetAll();
            var projeto = projetos.FirstOrDefault(x=>x.Languages
            .FirstOrDefault(z=>z.Language == "br")
            .Projeto.Titulo != "Teste de atualização");
            if (projeto is null)
            {
                Assert.True(false, "Não há projetos para atualizar");
            }
            var newProjeto = new ProjetoArquiteturaRequest
            {
                Titulo = "Teste de atualização",
                Descricao = "Esse é um teste de atualização de entidade",
                Categoria = "Teste Atualização"
            };
            _ = await _controller.Put(projeto.Id, newProjeto, this.UserSecret ?? Guid.Empty);

            var projetoAtualizado = await _repository.Get(projeto.Id);
            var tituloProjetoInicial = projeto.Languages.First().Projeto.Titulo;
            var tituloNovoProjeto = projetoAtualizado.Languages.First().Projeto.Titulo;
            Assert.NotEqual(tituloNovoProjeto, tituloProjetoInicial);
        }

        [Fact]
        public async Task GetAllProjetos()
        {
            var projetos = await _repository.GetAll();
            var projetosApi = await _controller.GetAsync("br");
            var projetosCount = projetos.Count();
            var projetosApiCount = projetosApi.Count();
            var firstProjeto = projetos.FirstOrDefault();
            var firstApiProjeto = projetosApi.FirstOrDefault();
            Assert.Equal(projetosApiCount, projetosCount);
            var tituloProjeto = firstProjeto.Languages.FirstOrDefault(x => x.Language == "br").Projeto.Titulo;
            var tituloApiProjeto = firstApiProjeto.Titulo;
            Assert.Equal(tituloApiProjeto, tituloProjeto);
        }

        [Fact]
        public async Task GetSingle()
        {
            var projetos = await _repository.GetAll();
            var projeto = projetos.FirstOrDefault();
            var projetoApi = await _controller.GetAsync(projeto.Id, "br");
            var tituloProjeto = projeto.Languages.FirstOrDefault(x => x.Language == "br").Projeto.Titulo;
            var tituloApiProjeto = projetoApi.Titulo;
            Assert.Equal(tituloProjeto, tituloApiProjeto);
        }

        [Fact]
        public async Task Translate()
        {
            var projetos = await _repository.GetAll();
            var projeto = projetos.FirstOrDefault();
            var projetoApi = await _controller.GetAsync(projeto.Id, "en");
            var tituloProjeto = projeto.Languages.FirstOrDefault(x => x.Language == "br").Projeto.Titulo;
            var tituloApiProjeto = projetoApi.Titulo;
            Assert.NotEqual(tituloProjeto, tituloApiProjeto);
        }
    }
}