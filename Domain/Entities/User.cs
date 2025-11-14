namespace Voxia.Api.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<CategoriaFavorito> CategoriasFavoritos { get; set; } = new List<CategoriaFavorito>();
    public ICollection<CardClickStat> Stats { get; set; } = new List<CardClickStat>();
}
