namespace AnguloZApi.Domain
{
    public class ProjetoArch
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public List<string> Imagens { get; set; }
        public string Categoria { get; set; }
    }
}
