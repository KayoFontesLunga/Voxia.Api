using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoxiasApp.Infrastructure.Data;
using VoxiasApp.Utils;

namespace Voxia.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public StatsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("7-days")]
    public async Task<IActionResult> GetLast7DaysStats(CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // intervalo: hoje (Data.Date) e 6 dias atrás (inclusivo)
        var end = DateTime.UtcNow.Date; // midnight UTC today
        var start = end.AddDays(-6);    // 6 days before (inclusive)

        // obter clicks do período para o usuário
        var clicks = await _db.CardStats
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.Data.Date >= start && s.Data.Date <= end)
            .Select(s => new
            {
                Day = s.Data.Date,
                s.CardNome,
                s.IsHomeCard
            })
            .ToListAsync(ct);

        // agrupar por dia e card
        var grouped = clicks
            .GroupBy(x => new { x.Day, x.CardNome, x.IsHomeCard })
            .Select(g => new
            {
                Day = g.Key.Day,
                CardNome = g.Key.CardNome,
                IsHomeCard = g.Key.IsHomeCard,
                Count = g.Count()
            })
            .ToList();

        // construir resultado dia a dia (garante ordem cronológica: start -> end)
        var result = new List<object>();

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var dayGroup = grouped.Where(g => g.Day == d).ToList();

            if (!dayGroup.Any())
            {
                result.Add(new
                {
                    Date = d,
                    Top = Array.Empty<object>()
                });
                continue;
            }

            var maxCount = dayGroup.Max(x => x.Count);
            var topCards = dayGroup
                .Where(x => x.Count == maxCount)
                .Select(x => new
                {
                    CardNome = x.CardNome,
                    IsHomeCard = x.IsHomeCard,
                    Count = x.Count
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.CardNome)
                .ToList();

            result.Add(new
            {
                Date = d,
                Top = topCards
            });
        }

        return Ok(new
        {
            From = start,
            To = end,
            Days = result
        });
    }
}
