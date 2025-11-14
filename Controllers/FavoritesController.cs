using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voxia.Api.Domain.Entities;
using Voxia.Api.DTOs.Favorites;
using Voxia.Api.Services.Files;
using VoxiasApp.Infrastructure.Data;
using VoxiasApp.Utils;

namespace Voxia.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IFileService _fileService;

    public FavoritesController(ApplicationDbContext db, IFileService fileService)
    {
        _db = db;
        _fileService = fileService;
    }

    [HttpPost("cards")]
    public async Task<IActionResult> CreateCard([FromForm] CreateFavoriteCardDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // verifica se a categoria existe e pertence ao usuário
        var categoria = await _db.CategoriasFavoritos
            .FirstOrDefaultAsync(c => c.Id == dto.CategoriaFavoritoId && c.UserId == userId, ct);

        if (categoria == null) return BadRequest(new { message = "Categoria favorita não encontrada para o usuário." });

        string imagemPath = string.Empty;
        string audioPath = string.Empty;

        if (dto.Imagem != null)
        {
            var imgResult = await _fileService.SaveImageAsync(dto.Imagem, userId, ct);
            imagemPath = imgResult.RelativePath;
        }

        if (dto.Audio != null)
        {
            var audioResult = await _fileService.SaveAudioAsync(dto.Audio, userId, ct);
            audioPath = audioResult.RelativePath;
        }

        var card = new CardFavorito
        {
            Nome = dto.Nome,
            ImagemPath = imagemPath,
            AudioPath = audioPath,
            CategoriaFavoritoId = dto.CategoriaFavoritoId
        };

        _db.CardsFavoritos.Add(card);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetCard), new { id = card.Id }, new
        {
            card.Id,
            card.Nome,
            card.ImagemPath,
            card.AudioPath,
            card.CategoriaFavoritoId
        });
    }

    [HttpGet("cards/{id:guid}")]
    public async Task<IActionResult> GetCard([FromRoute] Guid id)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var card = await _db.CardsFavoritos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Categoria.UserId == userId);

        if (card == null) return NotFound();
        return Ok(new
        {
            card.Id,
            card.Nome,
            card.ImagemPath,
            card.AudioPath,
            card.CategoriaFavoritoId
        });
    }
}
