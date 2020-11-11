namespace Chel.Abstractions.Parsing
{
    public class ContinueResponse : TokenizerStateResponse
    {
        public static ContinueResponse Instance { get; } = new ContinueResponse();
    }
}
