using AnguloZApi.Domain;
using AnguloZApi.Extensions;
using AnguloZApi.Repositories;
using CognitiveServices.Translator;
using MongoDB.Driver;

namespace AnguloZApi.Tests
{
    public class ProjetoRepositoryTestImp : IProjetoArchRepository
    {
        private readonly IMongoCollection<ProjetoEntityModel> _projects;
        private readonly ITranslateClient _translateClient;

        public ProjetoRepositoryTestImp(IMongoDatabase database, ITranslateClient translateClient)
        {
            _projects = database.GetCollection<ProjetoEntityModel>("projetos-teste");
            _translateClient = translateClient;
        }

        public async Task<IEnumerable<ProjetoEntityModel>> GetAll()
        {
            var filter = Builders<ProjetoEntityModel>.Filter.Empty;
            var documents = await _projects.Find(filter).ToListAsync();
            return documents;
        }

        public async Task<ProjetoEntityModel> Get(Guid id)
        {
            var filter = Builders<ProjetoEntityModel>.Filter.Eq("_id", id);
            var result = await _projects.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<Guid> Create(ProjetoArchInput projetoArch)
        {
            var entity = TransformInputToEntity(projetoArch, Guid.Empty);
            await _projects.InsertOneAsync(entity);
            return entity.Id;
        }

        public async Task Update(Guid id, ProjetoArchInput projetoArch)
        {
            var entity = TransformInputToEntity(projetoArch, id);
            var filter = Builders<ProjetoEntityModel>.Filter.Eq("_id", id);
            await _projects.FindOneAndReplaceAsync(filter, entity);
        }

        public async Task Delete(Guid id)
        {
            await _projects.FindOneAndDeleteAsync(x => x.Id.Equals(id));
        }

        private ProjetoEntityModel TransformInputToEntity(ProjetoArchInput input, Guid id)
        {
            if (id == Guid.Empty)
                id = Guid.NewGuid();
            var entity = new ProjetoEntityModel
            {
                Id = id,
                Languages = new List<LanguageModel>
            {
                new LanguageModel
                {
                    Language = "br",
                    Projeto = new ProjetoArch
                    {
                        Titulo = input.Titulo,
                        Descricao = input.Descricao,
                        Categoria = input.Categoria
                    }
                },
                new LanguageModel
                {
                    Language = "en",
                    Projeto = new ProjetoArch
                    {
                        Titulo = input.Titulo.Translate(_translateClient),
                        Descricao = input.Descricao.Translate(_translateClient),
                        Categoria = input.Categoria.Translate(_translateClient)
                    }
                }
            }
            };

            return entity;
        }
    }
}


