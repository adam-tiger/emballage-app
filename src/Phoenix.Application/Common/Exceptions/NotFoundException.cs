namespace Phoenix.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested entity cannot be found in the data store.
/// </summary>
public sealed class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="NotFoundException"/>.
    /// </summary>
    /// <param name="entityName">The name of the entity type that was not found.</param>
    /// <param name="key">The key that was used to search for the entity.</param>
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.")
    {
    }
}
