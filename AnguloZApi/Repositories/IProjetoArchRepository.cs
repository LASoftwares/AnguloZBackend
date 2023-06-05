using AnguloZApi.Domain;

namespace AnguloZApi.Repositories
{
    public interface IProjetoArchRepository
    {
        public Task<IEnumerable<ProjetoEntityModel>> GetAll();
        public Task<ProjetoEntityModel> Get(Guid id);
        public Task<Guid> Create(ProjetoArchInput projetoArch);
        public Task Update(Guid Id,ProjetoArchInput projetoArch);
        public Task Delete(Guid Id);
    }
}
