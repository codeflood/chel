namespace Chel.Abstractions
{
    /// <summary>
    /// Defines an error in the input source.
    /// </summary>
    /// <param name="SourceLocation">The location the error occurred at.</param>
    /// <param name="Message">A message explaining the error.</param>
    public record SourceError(
        SourceLocation SourceLocation,
        string Message
    );
}