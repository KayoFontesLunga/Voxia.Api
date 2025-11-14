namespace Voxia.Api.Domain.Entities;

public class CardHome
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string ImagemUrl { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;

    // FK
    public Guid CategoriaHomeId { get; set; }
    public CategoriaHome Categoria { get; set; } = null!;
}
