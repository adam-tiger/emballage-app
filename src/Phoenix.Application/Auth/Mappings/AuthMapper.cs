using Riok.Mapperly.Abstractions;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Identity;

namespace Phoenix.Application.Auth.Mappings;

/// <summary>
/// Mapper Mapperly pour les entités et DTOs de la couche Auth.
/// </summary>
/// <remarks>
/// La propriété <c>Roles</c> ne peut pas être mappée automatiquement depuis
/// <see cref="ApplicationUser"/> (elle vient de UserManager, pas de l'entité).
/// Elle est ignorée dans le mapping de base et enrichie via
/// <see cref="ToUserProfileDtoWithRoles"/> dans les handlers.
/// </remarks>
[Mapper]
public sealed partial class AuthMapper
{
    /// <summary>
    /// Mappe un <see cref="ApplicationUser"/> vers un <see cref="UserProfileDto"/>
    /// avec un tableau de rôles vide — à enrichir via <see cref="ToUserProfileDtoWithRoles"/>.
    /// </summary>
    [MapProperty(nameof(ApplicationUser.Id),       nameof(UserProfileDto.Id))]
    [MapProperty(nameof(ApplicationUser.Email),    nameof(UserProfileDto.Email))]
    [MapProperty(nameof(ApplicationUser.FullName), nameof(UserProfileDto.FullName))]
    [MapProperty(nameof(ApplicationUser.IsActive), nameof(UserProfileDto.IsActive))]
    [MapperIgnore(nameof(UserProfileDto.Roles))]
    [MapperIgnore(nameof(UserProfileDto.Segment))]
    public partial UserProfileDto ToUserProfileDto(ApplicationUser user);

    /// <summary>
    /// Construit un <see cref="UserProfileDto"/> complet en ajoutant les rôles et le segment
    /// qui ne peuvent pas être mappés automatiquement depuis <see cref="ApplicationUser"/>.
    /// </summary>
    /// <param name="user">L'utilisateur applicatif source.</param>
    /// <param name="roles">Rôles récupérés via <c>UserManager.GetRolesAsync</c>.</param>
    /// <returns>Le DTO complet prêt à être sérialisé.</returns>
    public UserProfileDto ToUserProfileDtoWithRoles(ApplicationUser user, string[] roles)
    {
        var dto = ToUserProfileDto(user);
        return dto with
        {
            Roles   = roles,
            Segment = user.Segment.ToString()
        };
    }
}
