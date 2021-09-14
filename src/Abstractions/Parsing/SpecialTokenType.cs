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
        ParameterName = 4,
        ListStart = 5,
        ListEnd = 6,
        MapStart = 7,
        MapEnd = 8,
        Subcommand = 9
    }
}