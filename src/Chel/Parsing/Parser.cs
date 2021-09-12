using System;
using System.Collections.Generic;
using System.Text;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;
using Chel.Exceptions;

namespace Chel.Parsing
{
	/// <summary>
	/// The default implementation of the <see cref="IParser" />.
	/// </summary>
    /// <remarks>This class is not thread-safe.</remarks>
	public class Parser : IParser
    {
        private const char Newline = '\n';

        private ITokenizer _tokenizer = null;
        private SourceLocation _lastTokenLocation = new SourceLocation(1, 1);
        private Token _nextToken = null;
        private StringBuilder _buffer = new StringBuilder();

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
            var token = SkipWhiteSpace();
            if(token == null)
                return null;

            var commandName = ParseName(token);

            if(commandName == null)
                return null;

            var commandInputBuilder = new CommandInput.Builder(commandName.LocationStart.LineNumber, (commandName.Block as Literal).Value);

            var parsedBlock = commandName;

            while(!parsedBlock.IsEndOfLine)
            {
                parsedBlock = ParseParameters();
                
                if(parsedBlock.Block != null)
                    commandInputBuilder.AddParameter(parsedBlock.Block);
            }
            
            return commandInputBuilder.Build();
        }

        private ParseBlock ParseParameters()
        {
            var block = new StringBuilder();
            var isEndOfLine = false;
            SourceLocation locationStart = null;

            var token = SkipWhiteSpace();
            if(token == null)
                return new ParseBlock(_lastTokenLocation, null, isEndOfLine: true);

            while(!isEndOfLine)
            {
                if(locationStart == null)
                    locationStart = token?.Location;
                
                if(token == null || (token is LiteralToken lt && lt.Value == Newline))
                {
                    isEndOfLine = true;
                    break;
                }

                if(token is SpecialToken specialToken)
                {
                    switch(specialToken.Type)
                    {
                        case SpecialTokenType.ParameterName:
                            return ParseParameterName(token);

                        case SpecialTokenType.BlockEnd:
                            throw new ParseException(locationStart, Texts.MissingBlockStart);

                        case SpecialTokenType.ListEnd:
                            throw new ParseException(locationStart, Texts.MissingListStart);

                        default:
                            return ParseParameterValue(token);
                    }
                }
                else if(token is LiteralToken literalToken)
                {
                    if(char.IsWhiteSpace(literalToken.Value))
                    {
                        break;
                    }
                    else
                        return ParseParameterValue(token);
                }       
            }

            var parsedBlock = block.ToString();

            var location = locationStart;
            if(location == null)
                location = new SourceLocation(_lastTokenLocation.LineNumber, _lastTokenLocation.CharacterNumber + 1);

            return new ParseBlock(location, null, isEndOfLine: isEndOfLine);
        }

        private ParseBlock ParseName(Token token)
        {
            _buffer.Clear();

            var isEndOfLine = false;
            var locationStart = _lastTokenLocation;

            while(token != null)
            {
                if(token is SpecialToken specialToken)
                    throw new ParseException(_lastTokenLocation, string.Format(Texts.UnexpectedToken, specialToken.Type));

                var literalToken = token as LiteralToken;

                if(literalToken.Value == Newline)
                {
                    isEndOfLine = true;
                    break;
                }
                
                if(char.IsWhiteSpace(literalToken.Value))
                    break;

                _buffer.Append(literalToken.Value);

                token = GetNextToken();
            }

            var name = _buffer.ToString();

            if(string.IsNullOrWhiteSpace(name))
                return null;

            var parameter = new Literal(name);
            return new ParseBlock(locationStart, parameter, isEndOfLine: isEndOfLine);
        }

        private ParseBlock ParseBlock(Token token)
        {
            if(!(token is SpecialToken specialTokenGuard) || specialTokenGuard.Type != SpecialTokenType.BlockStart)
                throw new ParseException(_lastTokenLocation, Texts.MissingBlockStart);

            _buffer.Clear();
            var blockStartCount = 1;
            var locationStart = _lastTokenLocation;

            while(blockStartCount > 0)
            {
                token = GetNextToken();

                if(token == null)
                    throw new ParseException(_lastTokenLocation, Texts.MissingBlockEnd);

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
                            _buffer.Append("$");
                            break;

                        case SpecialTokenType.ParameterName:
                            _buffer.Append("-");
                            break;

                        case SpecialTokenType.ListStart:
                            _buffer.Append("[");
                            break;

                        case SpecialTokenType.ListEnd:
                            _buffer.Append("]");
                            break;

                        default:
                            // todo: Change SpecialTokenType to be a class which we can call ToString() on to get the token.
                            // This would avoid all the cases above.
                            throw new ParseException(specialToken.Location, string.Format(Texts.UnexpectedToken, specialToken.Type));
                    }
                }
                else if(token is LiteralToken literalToken)
                {
                    _buffer.Append(literalToken.Value);
                }
            }

            var parameter = new Literal(_buffer.ToString());
            return new ParseBlock(locationStart, parameter, isEndOfLine: false);
        }

        private ParseBlock ParseParameterName(Token token)
        {
            if(!(token is SpecialToken specialTokenGuard) || specialTokenGuard.Type != SpecialTokenType.ParameterName)
                throw new ParseException(_lastTokenLocation, string.Format(Texts.UnexpectedToken, token));

            var locationStart = _lastTokenLocation;

            token = GetNextToken();
            if(token == null)
                throw new ParseException(_lastTokenLocation, Texts.MissingParameterName);

            var name = ParseName(token);
            if(name == null)
                throw new ParseException(_lastTokenLocation, Texts.MissingParameterName);

            var parameter = new ParameterNameCommandParameter((name.Block as Literal).Value);
            return new ParseBlock(locationStart, parameter, isEndOfLine: name.IsEndOfLine);
        }

        private ParseBlock ParseParameterValue(Token token)
        {
            if(token is SpecialToken specialToken)
            {
                switch(specialToken.Type)
                {
                    case SpecialTokenType.BlockStart:
                        return ParseBlock(token);

                    case SpecialTokenType.VariableMarker:
                        return ParseCompoundValue(token);

                    case SpecialTokenType.ListStart:
                        return ParseList(token);

                    case SpecialTokenType.BlockEnd:
                    case SpecialTokenType.ParameterName:
                    case SpecialTokenType.ListEnd:
                        throw new ParseException(_lastTokenLocation, string.Format(Texts.UnexpectedToken, token));

                    default:
                        throw new ParseException(specialToken.Location, string.Format(Texts.UnexpectedToken, specialToken.Type));
                }
            }

            return ParseCompoundValue(token);
        }

        private ParseBlock ParseCompoundValue(Token token)
        {
            _buffer.Clear();

            var isEndOfLine = false;
            var locationStart = _lastTokenLocation;
            var parameters = new List<ChelType>();
            var done = false;
            
            while(token != null && !done)
            {
                if(token is SpecialToken specialToken)
                {
                    switch(specialToken.Type)
                    {
                        case SpecialTokenType.VariableMarker:
                            var variableReference = ParseVariableReference(token);
                            parameters.Add(variableReference);
                            break;

                        case SpecialTokenType.ParameterName:
                            _buffer.Append("-");
                            break;

                        default:
                            PushNextToken(token);
                            done = true;
                            break;
                    }
                }
                else if(token is LiteralToken literalToken)
                {
                    if(literalToken.Value == Newline)
                    {
                        isEndOfLine = true;
                        break;
                    }

                    if(char.IsWhiteSpace(literalToken.Value))
                        break;

                    _buffer.Append(literalToken.Value);
                }

                if(!done)
                    token = GetNextToken();
            }

            var value = _buffer.ToString();
            if(!string.IsNullOrEmpty(value))
                parameters.Add(new Literal(value));

            if(parameters.Count == 0)
                return new ParseBlock(locationStart, null, isEndOfLine: isEndOfLine);

            if(parameters.Count == 1)
                return new ParseBlock(locationStart, parameters[0], isEndOfLine: isEndOfLine);

            return new ParseBlock(locationStart, new CompoundValue(parameters), isEndOfLine: isEndOfLine);
        }

        private VariableReference ParseVariableReference(Token token)
        {
            if(!(token is SpecialToken specialTokenGuard) || specialTokenGuard.Type != SpecialTokenType.VariableMarker)
                throw new ParseException(_lastTokenLocation, string.Format(Texts.UnexpectedToken, token));

            _buffer.Clear();
            var locationStart = _lastTokenLocation;
            string variableName = null;
            var subreferences = new List<string>();

            token = GetNextToken();

            while(token != null)
            {
                var specialToken = token as SpecialToken;
                var literalToken = token as LiteralToken;

                if(
                    (specialToken != null && specialToken.Type == SpecialTokenType.VariableMarker) ||
                    (literalToken != null && literalToken.Value.Equals(Symbol.SubName))
                )
                {
                    var name = _buffer.ToString();
                    if(name.Length == 0)
                    {
                        if(variableName == null)
                            throw new ParseException(_lastTokenLocation, Texts.MissingVariableName);
                        else
                            throw new ParseException(token.Location, string.Format(Texts.MissingSubreferenceForVariable, variableName));
                    }

                    if(variableName == null)
                        variableName = name;
                    else
                        subreferences.Add(name);

                    _buffer.Clear();

                    if(specialToken != null && specialToken.Type == SpecialTokenType.VariableMarker)
                        break;
                }
                else if(specialToken != null)
                    throw new ParseException(locationStart, Texts.UnpairedVariableToken);
                else if(literalToken != null)
                {
                    if(literalToken.Value == Newline || char.IsWhiteSpace(literalToken.Value))
                        throw new ParseException(locationStart, Texts.UnpairedVariableToken);
                    else
                        _buffer.Append(literalToken.Value);
                }

                token = GetNextToken();
            }

            if(variableName == null)
                throw new ParseException(locationStart, Texts.UnpairedVariableToken);

            return new VariableReference(variableName, subreferences);
        }

        private ParseBlock ParseList(Token token)
        {
            if(token == null)
                return null;

            if(!(token is SpecialToken specialTokenGuard) || specialTokenGuard.Type != SpecialTokenType.ListStart)
                throw new ParseException(_lastTokenLocation, string.Format(Texts.MissingListStart, token));

            token = GetNextToken();
            var startLocation = _lastTokenLocation;

            var listValues = new List<ChelType>();
            var listCompleted = false;

            while(token != null)
            {
                if(token is SpecialToken specialToken && specialToken.Type == SpecialTokenType.ListEnd)
                {
                    listCompleted = true;
                    break;
                }

                var value = ParseParameterValue(token);
                if(value?.Block != null)
                    listValues.Add(value.Block);                
                
                token = GetNextToken();
            }

            if(!listCompleted)
                throw new ParseException(startLocation, Texts.MissingListEnd);

            var parameter = new List(listValues);
            return new ParseBlock(startLocation, parameter);
        }

        private Token GetNextToken()
        {
            Token token = null;

            if(_nextToken != null)
            {
                token = _nextToken;
                _nextToken = null;
            }
            else
                token = _tokenizer.GetNextToken();

            if(token != null)
                _lastTokenLocation = token.Location;
            
            return token;
        }

        private void PushNextToken(Token token)
        {
            if(_nextToken != null)
                throw new InvalidOperationException("Internal error. A token has already been pushed.");

            _nextToken = token;
        }

        private Token SkipWhiteSpace()
        {
            var token = GetNextToken();
            while(token != null)
            {
                if(token is SpecialToken)
                    return token;

                if(token is LiteralToken literalToken &&
                    (!char.IsWhiteSpace(literalToken.Value) || literalToken.Value == Newline))
                {
                    return token;
                }

                token = GetNextToken();
            }

            return null;
        }
    }
}
