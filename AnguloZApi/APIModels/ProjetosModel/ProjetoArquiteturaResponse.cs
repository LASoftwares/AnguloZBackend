namespace AnguloZApi.APIModels.ProjetosModel
{
    public class ProjetoArquiteturaResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public List<byte[]> Imagens { get; set; }
        public string Categoria { get; set; }
    }
}
