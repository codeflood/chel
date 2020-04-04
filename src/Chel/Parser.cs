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
        private int _currentSourceLine = 0;
        private StringReader _reader = null;

        public IList<CommandInput> Parse(string input)
        {
            var parsedLines = new List<CommandInput>();

            if(string.IsNullOrEmpty(input))
                return parsedLines;

            _reader = new StringReader(input);
            _currentSourceLine = 1;

            CommandInput parsedLine = null;

            do
            {
                parsedLine = ParseCommandInput();

                if(parsedLine != null)
                    parsedLines.Add(parsedLine);
            }
            while(_reader.Peek() != -1);

            return parsedLines;
        }

        private CommandInput ParseCommandInput()
        {
            var commandSourceLine = _currentSourceLine;
            var parsedBlock = ParseBlock();

            if(parsedBlock.Block == null)
                return null;

            var commandInputBuilder = new CommandInput.Builder(commandSourceLine, parsedBlock.Block);

            while(!parsedBlock.IsEndOfLine)
            {
                parsedBlock = ParseBlock();
                
                if(parsedBlock.Block != null)
                    commandInputBuilder.AddParameter(parsedBlock.Block);
            }
            
            return commandInputBuilder.Build();
        }

        private ParseBlock ParseBlock()
        {
            var block = new StringBuilder();
            var isEndOfLine = false;
            var ignore = false;
            var openingParenthesisCount = 0;
            var escaping = false;

            while(!isEndOfLine)
            {
                var character = _reader.Read();

                if(character == '\n')
                    _currentSourceLine++;
                
                if(character == -1 || (character == '\n' && openingParenthesisCount == 0))
                {
                    if(openingParenthesisCount > 0)
                        throw new ParserException(_currentSourceLine, Texts.MissingClosingParenthesis);
                    else if(openingParenthesisCount < 0)
                        throw new ParserException(_currentSourceLine, Texts.MissingOpeningParenthesis);

                    isEndOfLine = true;
                    break;
                }
                else
                {
                    if(character == '#' && !escaping)
                        ignore = true;
                    else if(!ignore && openingParenthesisCount == 0 && char.IsWhiteSpace((char)character))
                        break;
                    else if(!ignore && (openingParenthesisCount > 0 || !char.IsWhiteSpace((char)character)))
                    {
                        var c = (char)character;
                        if(c == '\\' && !escaping)
                            escaping = true;
                        else if(c == '(' && !escaping)
                        {
                            openingParenthesisCount++;

                            if(openingParenthesisCount > 1)
                                block.Append(c);
                        }
                        else if(c == ')' && !escaping)
                        {
                            openingParenthesisCount--;
                            if(openingParenthesisCount == 0)
                                break;
                            
                            if(openingParenthesisCount >= 1)
                                block.Append(c);
                        }
                        else
                        {
                            block.Append(c);

                            if(escaping)
                                escaping = false;
                        }
                    }
                }                
            }

            var parsedBlock = block.ToString();

            if(string.IsNullOrEmpty(parsedBlock))
                parsedBlock = null;

            return new ParseBlock(parsedBlock, isEndOfLine: isEndOfLine);
        }
    }
}
