namespace Voxia.Api.Services.Files;

public interface IFileService
{
    /// <summary>
    /// Salva uma imagem (IFormFile) na pasta Assets/users/{userId}/images e retorna o caminho relativo (/assets/...)
    /// </summary>
    Task<FileSaveResult> SaveImageAsync(IFormFile file, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Salva um áudio (IFormFile) na pasta Assets/users/{userId}/audio e retorna o caminho relativo (/assets/...)
    /// </summary>
    Task<FileSaveResult> SaveAudioAsync(IFormFile file, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Deleta um arquivo físico dado o caminho relativo (/assets/...)
    /// </summary>
    Task<bool> DeleteFileAsync(string relativePath, CancellationToken ct = default);

    /// <summary>
    /// Gera um nome de arquivo seguro e único — útil para tests/preview
    /// </summary>
    string GenerateSafeFileName(string originalFileName);
}
