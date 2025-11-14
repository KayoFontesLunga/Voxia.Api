namespace Voxia.Api.Domain.Entities;

public class CategoriaHome
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;

    // Relação 1:N com Cards
    public ICollection<CardHome> Cards { get; set; } = new List<CardHome>();
}
