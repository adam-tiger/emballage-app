namespace Phoenix.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when the current user does not have permission to perform an action.
/// </summary>
public sealed class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="ForbiddenException"/> with the default message.
    /// </summary>
    public ForbiddenException()
        : base("Access denied.")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ForbiddenException"/> with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the access violation.</param>
    public ForbiddenException(string message)
        : base(message)
    {
    }
}
