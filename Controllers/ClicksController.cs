using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voxia.Api.Domain.Entities;
using VoxiasApp.Infrastructure.Data;
using VoxiasApp.Utils;

namespace Voxia.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClicksController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ClicksController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost("card/{id:guid}")]
    public async Task<IActionResult> RegisterClick([FromRoute] Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // tenta encontrar card na home (system)
        var homeCard = await _db.CardsHome
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (homeCard != null)
        {
            var stat = new CardClickStat
            {
                CardNome = homeCard.Nome,
                Data = DateTime.UtcNow,
                UserId = userId,
                IsHomeCard = true
            };

            _db.CardStats.Add(stat);
            await _db.SaveChangesAsync(ct);
            return Ok(new { message = "Clique registrado (home card)." });
        }

        // tenta encontrar card favorito DO USUÁRIO
        var favCard = await _db.CardsFavoritos
            .AsNoTracking()
            .Include(c => c.Categoria)
            .FirstOrDefaultAsync(c => c.Id == id && c.Categoria.UserId == userId, ct);

        if (favCard != null)
        {
            var stat = new CardClickStat
            {
                CardNome = favCard.Nome,
                Data = DateTime.UtcNow,
                UserId = userId,
                IsHomeCard = false
            };

            _db.CardStats.Add(stat);
            await _db.SaveChangesAsync(ct);
            return Ok(new { message = "Clique registrado (favorite card)." });
        }

        // não encontrou
        return NotFound(new { message = "Card não encontrado (nem home nem favorito do usuário)." });
    }
}
