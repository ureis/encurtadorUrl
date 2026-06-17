using EncurtadorURL.Application.Interfaces;
using EncurtadorURL.Domain.Entities;
using EncurtadorURL.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EncurtadorURL.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlRepository _repository;
        private readonly IShortCodeGenerator _generator;

        public UrlController(IUrlRepository repository, IShortCodeGenerator generator)
        {
            _repository = repository;
            _generator = generator;
        }

        public record CreateUrlRequest(string OriginalUrl, string? Alias);
        public record UrlListItemResponse(string ShortCode, string OriginalUrl, DateTime CreatedAt, string ShortUrl);

        [HttpGet("api/urls")]
        public async Task<IActionResult> GetAll()
        {
            var records = await _repository.GetAllAsync();

            var items = records.Select(record => new UrlListItemResponse(
                record.ShortCode,
                record.OriginalUrl,
                record.CreatedAt,
                $"{Request.Scheme}://{Request.Host}/{record.ShortCode}"
            ));

            return Ok(items);
        }

        [HttpPost("api/urls")]
        public async Task<IActionResult> CreateShortUrl([FromBody] CreateUrlRequest request)
        {
            if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out _))
                return BadRequest("URL original inválida.");

            string shortCode;

            if (!string.IsNullOrWhiteSpace(request.Alias))
            {
                var existingAlias = await _repository.GetByAliasAsync(request.Alias);
                if (existingAlias != null)
                    return Conflict("O alias informado já está em uso.");

                shortCode = request.Alias;
            }
            else
            {
                // Criamos um ID único baseado nos ticks para alimentar o gerador. 
                // Em produção com banco robusto, poderíamos usar um ID sequence do banco.
                int uniqueId = Math.Abs((int)(DateTime.UtcNow.Ticks % int.MaxValue));
                shortCode = _generator.Generate(uniqueId);
            }

            var urlRecord = new UrlRecord
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(urlRecord);
            await _repository.SaveChangesAsync();

            var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";
            return Created(shortUrl, new { ShortUrl = shortUrl });
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var record = await _repository.GetByShortCodeAsync(shortCode);

            if (record == null)
                return NotFound("URL não encontrada.");

            return Redirect(record.OriginalUrl);
        }

        [HttpGet("api/urls/{shortCode}")]
        public async Task<IActionResult> GetByOriginal(string shortCode)
        {
            var record = await _repository.GetByShortCodeAsync(shortCode);

            if (record == null)
                return NotFound("URL não encontrada.");

            return Ok(record.OriginalUrl);
        }
    }
}
