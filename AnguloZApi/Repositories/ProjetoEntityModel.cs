using AnguloZApi.Domain;

namespace AnguloZApi.Repositories
{
    public class ProjetoEntityModel
    {
        public Guid Id { get; set; }
        public List<LanguageModel> Languages { get; set; }
    }

    public class LanguageModel
    {
        public string Language { get; set; }
        public ProjetoArch Projeto { get; set; }
    }
}