using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PollyTimeout.Domain;

namespace PollyTimeout.Api.Controllers
{
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Content((await _documentRepository.GetDocumentAsync("foo", "bar.json")).ToString());
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}