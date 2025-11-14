using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voxia.Api.DTOs.Home;
using VoxiasApp.Infrastructure.Data;

namespace Voxia.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _db.CategoriasHome
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Nome = c.Nome
            })
            .ToListAsync();

            return Ok(categories);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("categories/{id:guid}/cards")]
    public async Task<IActionResult> GetCardsByCategory([FromRoute] Guid id)
    {
        try
        {
            // Verifica se a categoria existe
            var exists = await _db.CategoriasHome.AsNoTracking().AnyAsync(c => c.Id == id);
            if (!exists)
                return NotFound(new { message = "Categoria não encontrada." });

            var cards = await _db.CardsHome
                .AsNoTracking()
                .Where(card => card.CategoriaHomeId == id)
                .OrderBy(card => card.Nome)
                .Select(card => new CardDto
                {
                    Id = card.Id,
                    Nome = card.Nome,
                    ImagemUrl = card.ImagemUrl,
                    AudioUrl = card.AudioUrl
                })
                .ToListAsync();

            return Ok(cards);
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}