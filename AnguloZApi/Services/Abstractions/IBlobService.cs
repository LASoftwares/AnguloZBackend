namespace AnguloZApi.Services.Abstractions
{
    public interface IBlobService
    {
        Task<IEnumerable<byte[]>> ReadBlobsFromProjectAsync(string nomeProjeto);
        Task<byte[]> ReadBlobByIndexAsync(string nomeProjeto, int indexImagem);
        Task<string> UploadBlobByProjectAsync(string nomeProjeto, Stream blobStream);
        Task<string> OverrideBlobByIndexAsync(string nomeProjeto, int indexImagem, Stream blobStream);
        Task DeleteBlobByIndexAsync(string nomeProjeto, int indexImagem);
        Task ClearProjectAsync(string nomeProjeto);
    }
}
