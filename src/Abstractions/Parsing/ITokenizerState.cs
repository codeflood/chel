namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// An internal state of the <see cref="ITokenizer"/>.
    /// </summary>
    public interface ITokenizerState
    {
        TokenizerStateResponse Process(char input);
    }
}
