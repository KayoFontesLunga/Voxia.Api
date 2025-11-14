namespace Voxia.Api.DTOs.Home;

public class CategoryWithCardsDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public IEnumerable<CardDto> Cards { get; set; } = Enumerable.Empty<CardDto>();
}
