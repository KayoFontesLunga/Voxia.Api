namespace Voxia.Api.Domain.Entities;

public class CardFavorito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string ImagemPath { get; set; } = string.Empty;  // arquivo físico na pasta Assets
    public string AudioPath { get; set; } = string.Empty;

    // FK
    public Guid CategoriaFavoritoId { get; set; }
    public CategoriaFavorito Categoria { get; set; } = null!;
}
