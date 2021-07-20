namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// The type of a special token.
    /// </summary>
    public enum SpecialTokenType
    {
        Undefined = 0,
        BlockStart = 1,
        BlockEnd = 2,
        VariableMarker = 3,
        ParameterName = 4
    }
}