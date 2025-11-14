namespace Voxia.Api.DTOs.Home;

public class CardDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string ImagemUrl { get; set; } = string.Empty; // /assets/...
    public string AudioUrl { get; set; } = string.Empty;  // /assets/...
}
