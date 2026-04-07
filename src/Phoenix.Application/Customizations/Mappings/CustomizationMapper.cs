using Phoenix.Application.Customizations.Dtos;
using Phoenix.Domain.Customizations.Entities;
using Riok.Mapperly.Abstractions;

namespace Phoenix.Application.Customizations.Mappings;

/// <summary>
/// Mapper source-généré (Mapperly) pour le domaine Customization.
/// Toute la logique de mapping est résolue à la compilation — zéro réflexion à l'exécution.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="CustomizationJobDto.LogoSasUrl"/> et
/// <see cref="CustomizationJobDto.PrintCoefficient"/> sont ignorés dans le mapping de base
/// car ils sont calculés / enrichis manuellement après le mapping.
/// Utiliser <see cref="ToDtoEnriched"/> pour obtenir un DTO complet avec le coefficient.
/// </para>
/// <para>
/// <see cref="Phoenix.Domain.Customizations.ValueObjects.LogoPosition"/> est aplati :
/// les propriétés imbriquées (PositionX, PositionY, ScaleX, ScaleY, Rotation) sont
/// mappées directement vers les propriétés de premier niveau du DTO.
/// </para>
/// </remarks>
[Mapper]
public sealed partial class CustomizationMapper
{
    /// <summary>
    /// Mappe un <see cref="CustomizationJob"/> vers un <see cref="CustomizationJobDto"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <c>Status</c>, <c>PrintSide</c>, <c>ColorCount</c> : enum → string via <c>ToString()</c>.
    ///   </item>
    ///   <item>
    ///     <c>LogoPosition</c> est aplati : <c>LogoPosition.PositionX</c> → <c>PositionX</c>, etc.
    ///   </item>
    ///   <item>
    ///     <c>LogoSasUrl</c> et <c>PrintCoefficient</c> sont ignorés — enrichis manuellement.
    ///   </item>
    /// </list>
    /// </remarks>
    [MapProperty("LogoPosition.PositionX", "PositionX")]
    [MapProperty("LogoPosition.PositionY", "PositionY")]
    [MapProperty("LogoPosition.ScaleX",    "ScaleX")]
    [MapProperty("LogoPosition.ScaleY",    "ScaleY")]
    [MapProperty("LogoPosition.Rotation",  "Rotation")]
    [MapperIgnore(nameof(CustomizationJobDto.LogoSasUrl))]
    [MapperIgnore(nameof(CustomizationJobDto.PrintCoefficient))]
    public partial CustomizationJobDto ToDto(CustomizationJob job);

    /// <summary>
    /// Mappe un <see cref="CustomizationJob"/> vers un <see cref="CustomizationJobDto"/>
    /// en enrichissant automatiquement le <c>PrintCoefficient</c> calculé.
    /// </summary>
    /// <remarks>
    /// La <c>LogoSasUrl</c> n'est pas remplie ici — elle doit être enrichie séparément
    /// dans le handler après génération de l'URL SAS via <c>IBlobStorageService</c>.
    /// </remarks>
    /// <param name="job">L'agrégat source à mapper.</param>
    /// <returns>
    /// Un <see cref="CustomizationJobDto"/> avec <c>PrintCoefficient</c> calculé
    /// via <see cref="CustomizationJob.CalculatePrintCoefficient"/>.
    /// </returns>
    public CustomizationJobDto ToDtoEnriched(CustomizationJob job)
    {
        var dto = ToDto(job);
        return dto with { PrintCoefficient = job.CalculatePrintCoefficient() };
    }
}
