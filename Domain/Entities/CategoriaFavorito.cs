namespace Voxia.Api.Domain.Entities;

public class CategoriaFavorito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;

    // FK para o usuário
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<CardFavorito> Cards { get; set; } = new List<CardFavorito>();
}
