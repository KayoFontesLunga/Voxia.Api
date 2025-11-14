using System.Text.RegularExpressions;

namespace Voxia.Api.Services.Files;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileService> _logger;

    private readonly long _maxImageBytes = 5 * 1024 * 1024;   // 5 MB
    private readonly long _maxAudioBytes = 10 * 1024 * 1024;  // 10 MB
    private readonly string[] _allowedImageExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    private readonly string[] _allowedAudioExt = new[] { ".mp3", ".wav", ".m4a", ".aac" };

    public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<FileSaveResult> SaveImageAsync(IFormFile file, Guid userId, CancellationToken ct = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file.Length == 0) throw new ArgumentException("Arquivo de imagem está vazio.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedImageExt.Contains(ext))
            throw new InvalidOperationException($"Tipo de imagem não permitido. Permitidos: {string.Join(", ", _allowedImageExt)}");

        if (file.Length > _maxImageBytes)
            throw new InvalidOperationException($"Imagem excede o tamanho máximo de {_maxImageBytes / (1024 * 1024)} MB.");

        var safeName = GenerateSafeFileName(file.FileName);
        var folder = Path.Combine(_env.ContentRootPath, "Assets", "users", userId.ToString(), "images");
        Directory.CreateDirectory(folder);

        var physicalPath = Path.Combine(folder, safeName);

        await using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream, ct);
        }

        // caminho relativo que será servido via Static Files: /assets/users/{userId}/images/<file>
        var relative = $"/assets/users/{userId}/images/{safeName}";
        return new FileSaveResult(relative, physicalPath, safeName);
    }

    public async Task<FileSaveResult> SaveAudioAsync(IFormFile file, Guid userId, CancellationToken ct = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file.Length == 0) throw new ArgumentException("Arquivo de áudio está vazio.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedAudioExt.Contains(ext))
            throw new InvalidOperationException($"Tipo de áudio não permitido. Permitidos: {string.Join(", ", _allowedAudioExt)}");

        if (file.Length > _maxAudioBytes)
            throw new InvalidOperationException($"Áudio excede o tamanho máximo de {_maxAudioBytes / (1024 * 1024)} MB.");

        var safeName = GenerateSafeFileName(file.FileName);
        var folder = Path.Combine(_env.ContentRootPath, "Assets", "users", userId.ToString(), "audio");
        Directory.CreateDirectory(folder);

        var physicalPath = Path.Combine(folder, safeName);

        await using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream, ct);
        }

        var relative = $"/assets/users/{userId}/audio/{safeName}";
        return new FileSaveResult(relative, physicalPath, safeName);
    }

    public Task<bool> DeleteFileAsync(string relativePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return Task.FromResult(false);

        // garante que o caminho relativo inicia com /assets
        if (!relativePath.StartsWith("/assets", StringComparison.InvariantCultureIgnoreCase))
            return Task.FromResult(false);

        // Converte para caminho físico
        var trimmed = relativePath.TrimStart('/');
        var physical = Path.Combine(_env.ContentRootPath, trimmed.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(physical))
        {
            try
            {
                File.Delete(physical);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao deletar arquivo físico {Physical}", physical);
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(false);
    }

    public string GenerateSafeFileName(string originalFileName)
    {
        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
        var name = Path.GetFileNameWithoutExtension(originalFileName);

        // remove caracteres não alfanuméricos (deixa hífen e underline)
        name = Regex.Replace(name.ToLowerInvariant(), @"[^a-z0-9\-_]", "-");

        // reduz hífens sequenciais
        name = Regex.Replace(name, @"-+", "-").Trim('-');

        var unique = $"{Guid.NewGuid():N}";
        return $"{name}-{unique}{ext}";
    }
}
