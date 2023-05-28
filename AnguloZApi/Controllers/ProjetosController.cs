using AnguloZApi.APIModels.ProjetosModel;
using AnguloZApi.Domain;
using AnguloZApi.Repositories;
using AnguloZApi.Services.Abstractions;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnguloZApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetosController : ControllerBase
    {
        private readonly IProjetoArchRepository _projetoArchRepository;
        private readonly IBlobService _blobService;
        private readonly IAuthorizationService _authorizationService;

        public ProjetosController(IProjetoArchRepository projetoArchRepository, IBlobService blobService, IAuthorizationService authorizationService)
        {
            _projetoArchRepository = projetoArchRepository;
            _blobService = blobService;
            _authorizationService = authorizationService;
        }

        // GET: api/<ProjetosController>
        [HttpGet]
        public async Task<IEnumerable<ProjetoArch>> GetAsync()
        {
            return await _projetoArchRepository.GetAll();
        }

        // GET api/<ProjetosController>/5
        [HttpGet("{id}")]
        public async Task<ProjetoArch> GetAsync(Guid id)
        {
            return await _projetoArchRepository.Get(id);
        }

        // POST api/<ProjetosController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjetoArquiteturaRequest value, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            var newEntity = ProjetoRequestToRepositoryInput(value);
            foreach (var imagem in value.Imagens)
            {
                var stream = new MemoryStream(imagem);
                string nomeImagem = await _blobService.UploadBlobByProjectAsync(value.Titulo, stream);
                newEntity.Imagens.Add(nomeImagem);
            }
            var id = await _projetoArchRepository.Create(newEntity);
            return CreatedAtAction("Get", new { id });
        }
        [HttpPut]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProjetoArquiteturaRequest value, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            var oldEntity = await _projetoArchRepository.Get(id);
            var newEntity = ProjetoRequestToRepositoryInput(value) with 
            { 
                Imagens = oldEntity.Imagens 
            };
            return AcceptedAtAction(nameof(GetAsync), new { id = oldEntity.Id });
        }
        [HttpPut]
        [Route("OverwriteImage/{index}")]
        public async Task<IActionResult> OverwriteImageAsync(int index, [FromBody] OverwriteImageRequest request, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            var stream = new MemoryStream(request.Imagem);
            await _blobService.OverrideBlobByIndexAsync(request.NomeProjeto, index, stream);
            return Accepted();
        }
        [HttpDelete]
        [Route("DeleteImage/{projeto}/{index}")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute]string projeto, [FromRoute]int index, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            await _blobService.DeleteBlobByIndexAsync(projeto, index);
            return NoContent();
        }
        // DELETE api/<ProjetosController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            var projeto = await _projetoArchRepository.Get(id);
            await _projetoArchRepository.Delete(id);
            await _blobService.ClearProjectAsync(projeto.Titulo);
            return NoContent();
        }

        private ProjetoArchInput ProjetoRequestToRepositoryInput(ProjetoArquiteturaRequest value)
        {
            return new ProjetoArchInput(value.Titulo, value.Descricao, new List<string>());
        }
    }
}
