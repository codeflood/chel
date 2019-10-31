using System;

namespace Chel
{
    internal class ParseBlock
    {
        public string Block { get; }

        public bool EndOfLine { get; }

        public ParseBlock(string block, bool endOfLine)
        {
            Block = block;
            EndOfLine = endOfLine;
        }
    }
}