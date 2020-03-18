namespace Chel
{
    internal class ParseBlock
    {
        public string Block { get; }

        public bool IsEndOfLine { get; }

        public bool IsName { get; }

        public ParseBlock(string block, bool isEndOfLine = false, bool isName = false)
        {
            Block = block;
            IsEndOfLine = isEndOfLine;
            IsName = isName;
        }
    }
}