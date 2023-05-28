using AnguloZApi.Domain;

namespace AnguloZApi.Repositories
{
    public interface IProjetoArchRepository
    {
        public Task<IEnumerable<ProjetoArch>> GetAll();
        public Task<ProjetoArch> Get(Guid id);
        public Task<Guid> Create(ProjetoArchInput projetoArch);
        public Task Update(Guid Id,ProjetoArchInput projetoArch);
        public Task Delete(Guid Id);
    }
}
