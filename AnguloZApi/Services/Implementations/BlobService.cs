using AnguloZApi.Extensions;
using AnguloZApi.Services.Abstractions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace AnguloZApi.Services.Implementations
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _blobContainerClient;
        public BlobService(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task<IEnumerable<byte[]>> ReadBlobsFromProjectAsync(string nomeProjeto)
        {
            List<byte[]> Imagens = new List<byte[]>();
            for (int i = 1;; i++)
            {
                string blobName = this.GetBlobName(nomeProjeto, i);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);
                if (!blobClient.Exists())
                    break;
                Stream stream = await blobClient.OpenReadAsync();
                stream.Position = 0;
                byte[] imagem = new byte[stream.Length];
                await stream.ReadAsync(imagem, 0, (int)stream.Length);
                Imagens.Add(imagem);
            }
            return Imagens;
        }

        public async Task<string> UploadBlobByProjectAsync(string nomeProjeto, Stream blobStream)
        {
            int index = this.GetProjectImagesLastIndex(nomeProjeto);
            string blobName = GetBlobName(nomeProjeto, index);
            await _blobContainerClient.UploadBlobAsync(blobName, blobStream);
            return blobName;
            
        }

        public async Task<byte[]> ReadBlobByIndexAsync(string nomeProjeto, int indexImagem)
        {
            string blobName = GetBlobName(nomeProjeto, indexImagem);
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            if(!blobClient.Exists())
                throw new Exception("Blob não encontrado");
            var stream = await blobClient.OpenReadAsync();
            stream.Position = 0;
            byte[] imagem = new byte[stream.Length];
            await stream.ReadAsync(imagem, 0, (int)stream.Length);
            return imagem;

        }

        public async Task<string> OverrideBlobByIndexAsync(string nomeProjeto, int indexImagem, Stream blobStream)
        {
            string blobName = GetBlobName(nomeProjeto, indexImagem);
            await _blobContainerClient.DeleteBlobIfExistsAsync(blobName);
            await _blobContainerClient.UploadBlobAsync(blobName,blobStream);
            return blobName;
        }

        public async Task DeleteBlobByIndexAsync(string nomeProjeto, int indexImagem)
        {
            string blobName = GetBlobName(nomeProjeto, indexImagem);
            await _blobContainerClient.DeleteBlobIfExistsAsync(blobName);
        }

        public async Task ClearProjectAsync(string nomeProjeto)
        {
            for (int i = 1; ; i++)
            {
                string blobName = this.GetBlobName(nomeProjeto, i);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);
                if (!blobClient.Exists())
                    break;
                await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
        }

        private int GetProjectImagesLastIndex(string nomeProjeto)
        {
            for (int i = 1; ; i++)
            {
                string blobName = this.GetBlobName(nomeProjeto, i);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);
                if (!blobClient.Exists())
                    return i;
            }
        }

        private string GetBlobName(string nomeProjeto, int indexImagem)
        {
            nomeProjeto = nomeProjeto.RemoverAcentos();
            return $"{nomeProjeto}/Imagem_{indexImagem}.jpg";
        }

    }
}
