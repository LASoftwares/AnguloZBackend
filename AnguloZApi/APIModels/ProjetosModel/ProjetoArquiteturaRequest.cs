namespace AnguloZApi.APIModels.ProjetosModel
{
    public class ProjetoArquiteturaRequest
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public List<byte[]> Imagens { get; set; }
    }
}
