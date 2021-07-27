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

            if(parsedBlock.Block.GetType() != typeof(LiteralCommandParameter))
                throw new ParseException(parsedBlock.LocationStart, "Command name must be a literal.");

            var commandInputBuilder = new CommandInput.Builder(parsedBlock.LocationStart.LineNumber, (parsedBlock.Block as LiteralCommandParameter).Value);

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
            var parameters = new List<CommandParameter>();
            var block = new StringBuilder();
            var isEndOfLine = false;
            var isVariable = false;
            var isParameterName = false;
            var isList = false;
            var listValues = new List<CommandParameter>();
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

                        case SpecialTokenType.VariableMarker:
                            var blockValue = block.ToString();

                            if(isVariable)
                            {
                                isVariable = false;

                                if(!string.IsNullOrEmpty(blockValue))
                                    parameters.Add(new VariableCommandParameter(blockValue));
                                    else
                                        throw new ParseException(token.Location, Texts.MissingVariableName);

                                block.Clear();
                            }
                            else
                            {
                                isVariable = true;

                                if(!string.IsNullOrEmpty(blockValue))
                                    parameters.Add(new LiteralCommandParameter(blockValue));

                                block.Clear();
                            }
                            break;

                        case SpecialTokenType.ParameterName:
                            if(blockStartCount == 0 &&block.Length == 0)
                                isParameterName = true;
                            else
                                block.Append("-");
                            break;

                        case SpecialTokenType.ListStart:
                            isList = true;
                            break;

                        case SpecialTokenType.ListEnd:
                            if(!isList)
                                throw new ParseException(token.Location, Texts.MissingListStart);

                            isList = false;

                            if(block.Length > 0 || parameters.Count > 0)
                            {
                                var parsedBlockInner = block.ToString();
                                if(!string.IsNullOrEmpty(parsedBlockInner))
                                    parameters.Add(new LiteralCommandParameter(parsedBlockInner));

                                var value = parameters.Count == 1 ? parameters[0] : new AggregateCommandParameter(parameters);
                                listValues.Add(value);
                            }

                            block.Clear();
                            parameters.Clear();
                            parameters.Add(new ListCommandParameter(listValues));
                            break;

                        default:
                            throw new ParseException(specialToken.Location, string.Format(Texts.UnexpectedToken, specialToken.Type));
                    }
                }
                else if(token is LiteralToken literalToken)
                {
                    if(blockStartCount == 0 && char.IsWhiteSpace(literalToken.Value))
                    {
                        if(isList && (block.Length > 0 || parameters.Count > 0))
                        {
                            // this will need attention
                            var parsedBlockInner = block.ToString();
                            if(!string.IsNullOrEmpty(parsedBlockInner))
                                parameters.Add(new LiteralCommandParameter(parsedBlockInner));

                            var value = parameters.Count == 1 ? parameters[0] : new AggregateCommandParameter(parameters);
                            listValues.Add(value);

                            block.Clear();
                            parameters.Clear();
                        }
                        else
                            break;
                    }
                    else
                        block.Append(literalToken.Value);
                }       
            }

            if(isVariable)
                throw new ParseException(locationStart, Texts.UnpairedVariableToken);

            if(isList)
                throw new ParseException(locationStart, Texts.MissingListEnd);

            var parsedBlock = block.ToString();

            if(isParameterName)
                parameters.Add(new ParameterNameCommandParameter(parsedBlock));
            else if(!string.IsNullOrEmpty(parsedBlock))
                parameters.Add(new LiteralCommandParameter(parsedBlock));

            var location = locationStart;
            if(location == null)
                location = new SourceLocation(_lastTokenLocation.LineNumber, _lastTokenLocation.CharacterNumber + 1);

            if(parameters.Count == 0)
                return new ParseBlock(location, null, isEndOfLine: isEndOfLine);

            if(parameters.Count == 1)
                return new ParseBlock(location, parameters[0], isEndOfLine: isEndOfLine);

            return new ParseBlock(location, new AggregateCommandParameter(parameters), isEndOfLine: isEndOfLine);
        }
    }
}
