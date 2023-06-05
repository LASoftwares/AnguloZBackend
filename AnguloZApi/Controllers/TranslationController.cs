using CognitiveServices.Translator;
using CognitiveServices.Translator.Translate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnguloZApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        [HttpPost]
        public IActionResult TranslateAsync([FromBody] string texto, [FromServices] ITranslateClient services)
        {
            var response = services.Translate(
                new RequestContent { Text = texto },
                new RequestParameter
                {
                    From = "pt-br",
                    To = new[] { "en" },
                    IncludeAlignment = true
                });
            if(response.First().Translations.Count>0)
                return Ok(response.First().Translations.First().Text);
            return StatusCode(500);
        }
    }
}
