namespace Voxia.Api.DTOs.Favorites;

public class CreateFavoriteCardDto
{
    public string Nome { get; set; } = string.Empty;
    public Guid CategoriaFavoritoId { get; set; } // categoria do usuário
    public IFormFile? Imagem { get; set; }  // form-data
    public IFormFile? Audio { get; set; }   // form-data
}
