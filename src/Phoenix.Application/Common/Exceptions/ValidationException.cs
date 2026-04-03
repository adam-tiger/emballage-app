using FluentValidation.Results;

namespace Phoenix.Application.Common.Exceptions;

/// <summary>
/// Exception thrown by <c>ValidationBehavior</c> when one or more FluentValidation
/// validators return errors. Contains a dictionary of field-level error messages.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Gets a read-only dictionary mapping property names to their validation error messages.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationException"/> from a collection
    /// of <see cref="ValidationFailure"/> instances.
    /// </summary>
    /// <param name="failures">The validation failures produced by FluentValidation.</param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
