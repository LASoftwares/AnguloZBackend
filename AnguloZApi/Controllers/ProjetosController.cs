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
        [Route("{lang}")]
        public async Task<IEnumerable<ProjetoArquiteturaResponse>> GetAsync(string lang)
        {
            var projetos = await _projetoArchRepository.GetAll();
            var entities = new List<ProjetoArquiteturaResponse>();
            foreach (var item in projetos)
            {
                var entity = await ProjetoRepositoryOutputToResponse(item,lang);
                entities.Add(entity);
            }
            return entities;
        }

        // GET api/<ProjetosController>/5
        [HttpGet("{id}/{lang}")]
        public async Task<ProjetoArquiteturaResponse> GetAsync(Guid id,string lang)
        {
            var projeto = await _projetoArchRepository.Get(id);
            return await ProjetoRepositoryOutputToResponse(projeto, lang);
        }

        // POST api/<ProjetosController>
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Post([FromBody] ProjetoArquiteturaRequest value, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            var newEntity = ProjetoRequestToRepositoryInput(value);
            var id = await _projetoArchRepository.Create(newEntity);
            return StatusCode(201, id);
        }
        [HttpPut]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProjetoArquiteturaRequest value, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            var oldEntity = await _projetoArchRepository.Get(id);
            return AcceptedAtAction(nameof(GetAsync), new { id = oldEntity.Id });
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
            await _blobService.ClearProjectAsync(projeto.Languages.FirstOrDefault(x=>x.Language == "br")?.Projeto.Titulo);
            return NoContent();
        }
        [HttpPost]
        [Route("Images/Add/{id}")]
        public async Task<IActionResult> AddImageAsync([FromRoute]Guid id, [FromForm]IFormFileCollection imageFiles, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            if(imageFiles.Count < 1)
                return BadRequest("No files were sent");
            var project = await _projetoArchRepository.Get(id);
            foreach (var imageFile in imageFiles)
            {
                var stream = imageFile.OpenReadStream();
                string nomeImagem = await _blobService.UploadBlobByProjectAsync(
                    project.Languages
                        .FirstOrDefault(x=>x.Language == "br").Projeto.Titulo, 
                    stream);
            }

            return Ok(project);
        }
        

        [HttpPut]
        [Route("Images/Overwrite/{projeto}/{index}")]
        public async Task<IActionResult> OverwriteImageAsync([FromRoute]int index, [FromRoute]string projeto, IFormFile file, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            var stream = file.OpenReadStream();
            await _blobService.OverrideBlobByIndexAsync(projeto, index, stream);
            return Accepted();
        }
        [HttpDelete]
        [Route("Images/{projeto}/{index}")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute]string projeto, [FromRoute]int index, [FromHeader] Guid userSecret)
        {
            bool validSecret = await _authorizationService.ValidateUserSecretAsync(userSecret);
            if (!validSecret)
                return Unauthorized();

            await _blobService.DeleteBlobByIndexAsync(projeto, index);
            return NoContent();
        }

        private ProjetoArchInput ProjetoRequestToRepositoryInput(ProjetoArquiteturaRequest value)
        {
            return new ProjetoArchInput(value.Titulo, value.Descricao, value.Categoria);
        }

        private async Task<ProjetoArquiteturaResponse> ProjetoRepositoryOutputToResponse(ProjetoEntityModel value, string lang)
        {
            var entity = value.Languages.FirstOrDefault(value=> value.Language == lang).Projeto;
            var projeto =  new ProjetoArquiteturaResponse 
            {
                Id = value.Id,
                Categoria= entity.Categoria,
                Descricao = entity.Descricao,
                Titulo = entity.Titulo,
                Imagens = new List<byte[]>()
            };
            var blobs = await _blobService.ReadBlobsFromProjectAsync(entity.Titulo);
            foreach (var blob in blobs)
            {
                projeto.Imagens.Add(blob);
            }
            return projeto;
        }
    }
}
