namespace Phoenix.Application.Products.Dtos;

/// <summary>
/// Represents a product family with its enum value and French label.
/// </summary>
/// <param name="Value">The enum member name used as identifier.</param>
/// <param name="LabelFr">The human-readable French label.</param>
public sealed record ProductFamilyDto(string Value, string LabelFr);
