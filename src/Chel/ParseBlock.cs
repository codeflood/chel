namespace Chel
{
    internal class ParseBlock
    {
        public string Block { get; }

        public bool IsEndOfLine { get; }

        public ParseBlock(string block, bool isEndOfLine = false)
        {
            Block = block;
            IsEndOfLine = isEndOfLine;
        }
    }
}