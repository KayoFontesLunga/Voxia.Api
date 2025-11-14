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

    [HttpGet("categories")]
    public async Task<IActionResult> GetUserCategories(CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var categories = await _db.CategoriasFavoritos
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Nome)
            .Select(c => new
            {
                c.Id,
                c.Nome
            })
            .ToListAsync(ct);

        return Ok(categories);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateFavoriteCategoryDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.Nome))
            return BadRequest(new { message = "Nome da categoria é obrigatório." });

        // opcional: evitar nomes duplicados para o mesmo usuário
        var exists = await _db.CategoriasFavoritos
            .AnyAsync(c => c.UserId == userId && c.Nome.ToLower() == dto.Nome.Trim().ToLower(), ct);
        if (exists)
            return Conflict(new { message = "Você já possui uma categoria com esse nome." });

        var categoria = new CategoriaFavorito
        {
            Nome = dto.Nome.Trim(),
            UserId = userId
        };

        _db.CategoriasFavoritos.Add(categoria);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetUserCategories), new { id = categoria.Id }, new { categoria.Id, categoria.Nome });
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateFavoriteCategoryDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.Nome))
            return BadRequest(new { message = "Nome da categoria é obrigatório." });

        var categoria = await _db.CategoriasFavoritos
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);

        if (categoria == null) return NotFound(new { message = "Categoria não encontrada." });

        // opcional: evita duplicidade
        var exists = await _db.CategoriasFavoritos
            .AnyAsync(c => c.UserId == userId && c.Id != id && c.Nome.ToLower() == dto.Nome.Trim().ToLower(), ct);
        if (exists)
            return Conflict(new { message = "Você já possui outra categoria com esse nome." });

        categoria.Nome = dto.Nome.Trim();
        await _db.SaveChangesAsync(ct);

        return Ok(new { categoria.Id, categoria.Nome });
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // carrega categoria com cards relacionados
        var categoria = await _db.CategoriasFavoritos
            .Include(c => c.Cards)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);

        if (categoria == null) return NotFound(new { message = "Categoria não encontrada." });

        // Deleta arquivos físicos dos cards (images/audio) antes de remover do banco
        foreach (var card in categoria.Cards)
        {
            if (!string.IsNullOrWhiteSpace(card.ImagemPath))
            {
                await _fileService.DeleteFileAsync(card.ImagemPath, ct);
            }
            if (!string.IsNullOrWhiteSpace(card.AudioPath))
            {
                await _fileService.DeleteFileAsync(card.AudioPath, ct);
            }
        }

        // remove do banco (cascade configured)
        _db.CategoriasFavoritos.Remove(categoria);
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpGet("categories/{categoryId:guid}/cards")]
    public async Task<IActionResult> GetCardsByCategory(Guid categoryId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // valida que categoria é do usuário
        var categoria = await _db.CategoriasFavoritos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId, ct);

        if (categoria == null)
            return NotFound(new { message = "Categoria não encontrada." });

        var cards = await _db.CardsFavoritos
            .AsNoTracking()
            .Where(c => c.CategoriaFavoritoId == categoryId)
            .OrderBy(c => c.Nome)
            .Select(c => new {
                c.Id,
                c.Nome,
                c.ImagemPath,
                c.AudioPath
            })
            .ToListAsync(ct);

        return Ok(cards);
    }

    [HttpPut("cards/{id:guid}")]
    public async Task<IActionResult> UpdateCard(Guid id, [FromForm] UpdateFavoriteCardDto dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var card = await _db.CardsFavoritos
            .Include(c => c.Categoria)
            .FirstOrDefaultAsync(c => c.Id == id && c.Categoria.UserId == userId, ct);

        if (card == null)
            return NotFound(new { message = "Card não encontrado." });

        // Atualiza nome
        if (!string.IsNullOrWhiteSpace(dto.Nome))
            card.Nome = dto.Nome.Trim();

        // Trocar imagem (se enviada)
        if (dto.Imagem != null)
        {
            if (!string.IsNullOrWhiteSpace(card.ImagemPath))
                await _fileService.DeleteFileAsync(card.ImagemPath, ct);

            var imgResult = await _fileService.SaveImageAsync(dto.Imagem, userId, ct);
            card.ImagemPath = imgResult.RelativePath;
        }

        // Trocar áudio (se enviado)
        if (dto.Audio != null)
        {
            if (!string.IsNullOrWhiteSpace(card.AudioPath))
                await _fileService.DeleteFileAsync(card.AudioPath, ct);

            var audioResult = await _fileService.SaveAudioAsync(dto.Audio, userId, ct);
            card.AudioPath = audioResult.RelativePath;
        }

        await _db.SaveChangesAsync(ct);

        return Ok(new
        {
            card.Id,
            card.Nome,
            card.ImagemPath,
            card.AudioPath
        });
    }

    [HttpDelete("cards/{id:guid}")]
    public async Task<IActionResult> DeleteCard(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var card = await _db.CardsFavoritos
            .Include(c => c.Categoria)
            .FirstOrDefaultAsync(c => c.Id == id && c.Categoria.UserId == userId, ct);

        if (card == null)
            return NotFound(new { message = "Card não encontrado." });

        // apaga arquivos
        if (!string.IsNullOrWhiteSpace(card.ImagemPath))
            await _fileService.DeleteFileAsync(card.ImagemPath, ct);

        if (!string.IsNullOrWhiteSpace(card.AudioPath))
            await _fileService.DeleteFileAsync(card.AudioPath, ct);

        _db.CardsFavoritos.Remove(card);
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }
}
