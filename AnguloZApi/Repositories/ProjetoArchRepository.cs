using AnguloZApi.Domain;
using MongoDB.Driver;

namespace AnguloZApi.Repositories
{
    public class ProjetoArchRepository : IProjetoArchRepository
    {
        private readonly IMongoCollection<ProjetoArch> _projects;

        public ProjetoArchRepository(IMongoDatabase database)
        {
            _projects = database.GetCollection<ProjetoArch>("projetos");
        }

        public async Task<IEnumerable<ProjetoArch>> GetAll()
        {
            var filter = Builders<ProjetoArch>.Filter.Empty;
            var documents = await _projects.Find(filter).ToListAsync();
            return documents;
        }

        public async Task<ProjetoArch> Get(Guid id)
        {
            var filter = Builders<ProjetoArch>.Filter.Eq("_id", id);
            var result = await _projects.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<Guid> Create(ProjetoArchInput projetoArch)
        {
            var entity = TransformInputToEntity(projetoArch);
            await _projects.InsertOneAsync(entity);
            return entity.Id;
        }

        public async Task Update(Guid id, ProjetoArchInput projetoArch)
        {
            var entity = TransformInputToEntity(projetoArch);
            var filter = Builders<ProjetoArch>.Filter.Eq("_id", id);
            await _projects.FindOneAndReplaceAsync(filter, entity);
        }

        public async Task Delete(Guid id)
        {
            await _projects.FindOneAndDeleteAsync(x=>x.Id.Equals(id));
        }

        private ProjetoArch TransformInputToEntity(ProjetoArchInput input)
        {
            return new ProjetoArch
            {
                Id = Guid.NewGuid(),
                Titulo = input.Titulo,
                Descricao = input.Descricao,
                Imagens = input.Imagens
            };
        }
    }
    public record ProjetoArchInput(string Titulo, string Descricao, List<string> Imagens);
}
