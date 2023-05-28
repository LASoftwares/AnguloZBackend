using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace AnguloZApi.Test
{
    [TestClass]
    public class ProjetosTest
    {
        [TestMethod]
        public async Task PostProjeto()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(@"https://1baldasso.vercel.app/static/media/ProfilePic.608e39885653716e0a78.jpg");
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var input = new
            {
                Titulo = "teste",
                Descricao = "teste",
                Imagens = new List<byte[]> { bytes }
            };
            var str = JsonConvert.SerializeObject(input);
            HttpContent content = new StringContent(str);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response2 = await client.PostAsync(@"https://localhost:7026/api/Projetos",content);
            Assert.AreEqual(HttpStatusCode.Created,response2.StatusCode);
        }
    }
}