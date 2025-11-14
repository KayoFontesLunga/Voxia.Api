using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VoxiasApp.Utils;

public static class AuthExtensions
{
    /// <summary>
    /// Tenta extrair o UserId (Guid) do ClaimsPrincipal.
    /// Procura por ClaimTypes.NameIdentifier, JwtRegisteredClaimNames.Sub ou "id".
    /// Retorna Guid.Empty se não encontrar ou se for inválido.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        if (user == null) return Guid.Empty;

        string? idStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                        ?? user.FindFirst("id")?.Value;

        if (Guid.TryParse(idStr, out var id)) return id;
        return Guid.Empty;
    }

    /// <summary>
    /// Tenta extrair o email do ClaimsPrincipal.
    /// Procura por ClaimTypes.Email e JwtRegisteredClaimNames.Email.
    /// </summary>
    public static string? GetEmail(this ClaimsPrincipal user)
    {
        if (user == null) return null;

        return user.FindFirst(ClaimTypes.Email)?.Value
               ?? user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
    }

    /// <summary>
    /// Checa se o usuário está autenticado e tem um Guid válido.
    /// </summary>
    public static bool IsAuthenticatedWithId(this ClaimsPrincipal user)
    {
        return user?.Identity?.IsAuthenticated == true && user.GetUserId() != Guid.Empty;
    }
}
