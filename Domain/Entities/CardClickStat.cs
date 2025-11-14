namespace Voxia.Api.Domain.Entities;

public class CardClickStat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CardNome { get; set; } = string.Empty;
    public DateTime Data { get; set; }

    // FK do usuário
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Tipo do card (home ou favorito)
    public bool IsHomeCard { get; set; }
}
