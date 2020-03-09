using System.Collections.Generic;
using System.IO;
using System.Text;
using Chel.Abstractions;
using Chel.Exceptions;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="IParser" />.
    /// </summary>
    public class Parser : IParser
    {
        public IList<CommandInput> Parse(string input)
        {
            var parsedLines = new List<CommandInput>();

            if(string.IsNullOrEmpty(input))
                return parsedLines;

            var reader = new StringReader(input);
            var sourceLine = 1;

            CommandInput parsedLine = null;

            do
            {
                parsedLine = ParseSingleLine(reader, sourceLine);
                sourceLine++;

                if(parsedLine != null)
                    parsedLines.Add(parsedLine);
            }
            while(reader.Peek() != -1);

            return parsedLines;
        }

        private CommandInput ParseSingleLine(StringReader reader, int sourceLine)
        {
            var parsedBlock = ParseBlock(reader, sourceLine);

            if(parsedBlock.Block == null)
                return null;

            var commandInputBuilder = new CommandInput.Builder(sourceLine, parsedBlock.Block);

            while(!parsedBlock.EndOfLine)
            {
                parsedBlock = ParseBlock(reader, sourceLine);
                if(parsedBlock.Block != null)
                    commandInputBuilder.AddNumberedParameter(parsedBlock.Block);
            }
            
            return commandInputBuilder.Build();
        }

        private ParseBlock ParseBlock(StringReader reader, int sourceLine)
        {
            var block = new StringBuilder();
            var endOfLine = false;
            var ignore = false;
            var capturing = false;
            var insideParentheses = false;
            var escaping = false;

            while(!endOfLine)
            {
                var character = reader.Read();
                
                if(character == -1 || (!insideParentheses && character == '\n'))
                {
                    if(insideParentheses)
                        throw new ParserException(sourceLine, "Missing )");

                    endOfLine = true;
                    break;
                }
                else
                {
                    if(character == '#' && !escaping)
                        ignore = true;
                    else if(capturing && !insideParentheses && char.IsWhiteSpace((char)character))
                        break;
                    else if(!ignore && (insideParentheses || !char.IsWhiteSpace((char)character)))
                    {
                        var c = (char)character;
                        if(c == '\\')
                            escaping = true;
                        else if(c == '(' && !escaping)
                            insideParentheses = true;
                        else if(c == ')' && !escaping)
                            break;
                        else
                        {
                            block.Append(c);

                            if(escaping)
                                escaping = false;
                        }

                        capturing = true;
                    }
                }                
            }

            var parsedBlock = block.ToString();

            // Trim last \r in case it was a Windows line ending
            if(parsedBlock.Length > 0 &&
                parsedBlock[parsedBlock.Length - 1] == '\r')
                parsedBlock = parsedBlock.Substring(0, parsedBlock.Length - 1);
            
            if(string.IsNullOrEmpty(parsedBlock))
                return new ParseBlock(null, endOfLine);

            return new ParseBlock(parsedBlock, endOfLine);
        }
    }
}
