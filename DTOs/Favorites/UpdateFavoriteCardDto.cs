namespace Voxia.Api.DTOs.Favorites;

public class UpdateFavoriteCardDto
{
    public string Nome { get; set; } = string.Empty;

    // Arquivos opcionais na edição
    public IFormFile? Imagem { get; set; }
    public IFormFile? Audio { get; set; }
}
