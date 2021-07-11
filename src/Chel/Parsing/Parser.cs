using System.Collections.Generic;
using System.Text;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Exceptions;

namespace Chel.Parsing
{
	/// <summary>
	/// The default implementation of the <see cref="IParser" />.
	/// </summary>
	public class Parser : IParser
    {
        private ITokenizer _tokenizer = null;
        private SourceLocation _lastTokenLocation = new SourceLocation(1, 1);

        public IList<CommandInput> Parse(string input)
        {
            var parsedLines = new List<CommandInput>();

            if(string.IsNullOrEmpty(input))
                return parsedLines;

            _tokenizer = new Tokenizer(input);

            CommandInput parsedLine = null;

            do
            {
                parsedLine = ParseCommandInput();

                if(parsedLine != null)
                    parsedLines.Add(parsedLine);
            }
            while(_tokenizer.HasMore);

            return parsedLines;
        }

        private CommandInput ParseCommandInput()
        {
            var parsedBlock = ParseBlock();

            if(parsedBlock.Block == null)
                return null;

            var commandInputBuilder = new CommandInput.Builder(parsedBlock.LocationStart.LineNumber, parsedBlock.Block);

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
            var blockStartCount = 0;
            SourceLocation locationStart = null;

            while(!isEndOfLine)
            {
                var token = _tokenizer.GetNextToken();
                if(token != null)
                    _lastTokenLocation = token.Location;

                if(locationStart == null)
                    locationStart = token?.Location;
                
                if(token == null || (token is LiteralToken lt && lt.Value == '\n' && blockStartCount == 0))
                {
                    if(blockStartCount > 0)
                        throw new ParseException(_lastTokenLocation, Texts.MissingBlockEnd);
                    else if(blockStartCount < 0)
                        throw new ParseException(_lastTokenLocation, Texts.MissingBlockStart);

                    isEndOfLine = true;
                    break;
                }

                if(token is SpecialToken specialToken)
                {
                    switch(specialToken.Type)
                    {
                        case SpecialTokenType.BlockStart:
                            blockStartCount++;
                            break;

                        case SpecialTokenType.BlockEnd:
                            blockStartCount--;
                            if(blockStartCount == 0)
                                break;
                            break;

                        default:
                            throw new ParseException(specialToken.Location, string.Format(Texts.UnexpectedToken, specialToken.Type));
                    }
                }
                else if(token is LiteralToken literalToken)
                {
                    if(blockStartCount == 0 && char.IsWhiteSpace(literalToken.Value))
                        break;

                    block.Append(literalToken.Value);
                }       
            }

            var parsedBlock = block.ToString();

            if(string.IsNullOrEmpty(parsedBlock))
                parsedBlock = null;

            var location = locationStart;
            if(location == null)
                location = new SourceLocation(_lastTokenLocation.LineNumber, _lastTokenLocation.CharacterNumber + 1);

            return new ParseBlock(location, parsedBlock, isEndOfLine: isEndOfLine);
        }
    }
}
