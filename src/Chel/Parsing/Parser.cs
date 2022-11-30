using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chel.Abstractions;
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
        private INameValidator _nameValidator = null;
        private SourceLocation _lastTokenLocation = new SourceLocation(1, 1);
        private Token _nextToken = null;
        private StringBuilder _buffer = new StringBuilder();

        public Parser(INameValidator nameValidator)
        {
            _nameValidator = nameValidator ?? throw new ArgumentNullException(nameof(nameValidator));
        }

        public IList<CommandInput> Parse(string input)
        {
            var parsedLines = new List<CommandInput>();

            if(string.IsNullOrEmpty(input))
                return parsedLines;

            _tokenizer = new Tokenizer(input);

            CommandInput parsedLine = null;

            do
            {
                parsedLine = ParseCommandInput(true);

                if(parsedLine != null)
                    parsedLines.Add(parsedLine);
                
                // If the next token is a newline, skip over it.
                // todo: It looks like newline should be a special token. Sometimes newlines are special, and other times they're literals.
                if(_nextToken != null && _nextToken is LiteralToken literalNextToken && literalNextToken.Value == Newline)
                    _nextToken = null;

                // Any hanging tokens means the input wasn't formed properly
                if(_nextToken != null)
                {
                    var message = string.Format(Texts.UnexpectedToken, _nextToken);

                    if(_nextToken is SpecialToken specialToken)
                    {
                        switch(specialToken.Type)
                        {
                            case SpecialTokenType.ListEnd:
                                message = Texts.MissingListStart;
                                break;

                            case SpecialTokenType.MapEnd:
                                message = Texts.MissingMapStart;
                                break;

                            case SpecialTokenType.BlockEnd:
                                message = Texts.MissingBlockStart;
                                break;

                            default:
                                message = string.Format(Texts.UnexpectedToken, specialToken.Type);
                                break;
                        }
                    }

                    throw new ParseException(_lastTokenLocation, message);
                }
            }
            while(_tokenizer.HasMore);

            return parsedLines;
        }

        private CommandInput ParseCommandInput(bool parseParameters)
        {
            var token = SkipWhiteSpace();
            if(token == null)
                return null;

            var commandName = ParseName(token);

            if(commandName == null)
                return null;

            var commandInputBuilder = new CommandInput.Builder(commandName.SourceLocation, commandName.Name);

            if(parseParameters)
            {
                ICommandParameter parsedBlock = null;

                do
                {
                    parsedBlock = ParseParameters();
                    
                    if(parsedBlock != null)
                        commandInputBuilder.AddParameter(parsedBlock);

                } while(parsedBlock != null);
            }
            
            return commandInputBuilder.Build();
        }

        private ICommandParameter ParseParameters()
        {
            var token = SkipWhiteSpace();
            if(token == null || token is LiteralToken lt && lt.Value == Newline)
                return null;

            if(token is SpecialToken specialToken)
            {
                switch(specialToken.Type)
                {
                    case SpecialTokenType.ParameterName:
                        return ParseParameterName(token);

                    case SpecialTokenType.BlockEnd:
                    case SpecialTokenType.ListEnd:
                    case SpecialTokenType.MapEnd:
                        PushNextToken(token);
                        return null;
                }
            }

            return ParseParameterValue(token);
        }

        private SourceNameCommandParameter ParseName(Token token, params char[] additionalDelimiters)
        {
            _buffer.Clear();
            var locationStart = _lastTokenLocation;

            while(token != null)
            {
                var character = '\0';

                if(token is SpecialToken specialToken)
                {
                    // Names cannot start with a dash as that indicates a named parameter.
                    if(specialToken.Type == SpecialTokenType.ParameterName && _buffer.Length > 0)
                        character = '-';
                    else
                    {
                        PushNextToken(token);
                        break;
                    }
                }
                else
                {
                    var literalToken = token as LiteralToken;
                    character = literalToken.Value;
                }

                if(character == Newline || additionalDelimiters.Contains(character))
                {
                    PushNextToken(token);
                    break;
                }
                
                if(char.IsWhiteSpace(character))
                    break;

                _buffer.Append(character);

                token = GetNextToken();
            }

            var name = _buffer.ToString();

            if(string.IsNullOrWhiteSpace(name))
                return null;

            return new SourceNameCommandParameter(locationStart, name);
        }

        private SourceValueCommandParameter ParseBlock(Token token)
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

                        case SpecialTokenType.MapStart:
                            _buffer.Append("{");
                            break;

                        case SpecialTokenType.MapEnd:
                            _buffer.Append("}");
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
            return new SourceValueCommandParameter(locationStart, parameter);
        }

        private ParameterNameCommandParameter ParseParameterName(Token token)
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

            return new ParameterNameCommandParameter(locationStart, name.Name);
        }

        private ICommandParameter ParseParameterValue(Token token)
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

                    case SpecialTokenType.MapStart:
                        return ParseMap(token);

                    case SpecialTokenType.Subcommand:
                        return ParseSubcommand();

                    case SpecialTokenType.MapEnd:
                        throw new ParseException(_lastTokenLocation, Texts.MissingMapEntryValue);

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

        private SourceValueCommandParameter ParseCompoundValue(Token token)
        {
            _buffer.Clear();

            var compoundLocationStart = _lastTokenLocation;
            var literalLocationStart = compoundLocationStart;
            var parameters = new List<SourceValueCommandParameter>();
            var done = false;
            
            while(token != null && !done)
            {
                if(token is SpecialToken specialToken)
                {
                    switch(specialToken.Type)
                    {
                        case SpecialTokenType.VariableMarker:
                            var variableLocationStart = _lastTokenLocation;
                            var variableReference = ParseVariableReference(token);
                            parameters.Add(new SourceValueCommandParameter(variableLocationStart, variableReference));
                            literalLocationStart = _lastTokenLocation;
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
                        PushNextToken(token);
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
                parameters.Add(new SourceValueCommandParameter(literalLocationStart, new Literal(value)));

            if(parameters.Count == 0)
                return null;

            if(parameters.Count == 1)
                return parameters[0];

            return new SourceValueCommandParameter(compoundLocationStart, new CompoundValue(parameters));
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

        private SourceValueCommandParameter ParseList(Token token)
        {
            if(token == null)
                return null;

            if(!(token is SpecialToken specialTokenGuard) || specialTokenGuard.Type != SpecialTokenType.ListStart)
                throw new ParseException(_lastTokenLocation, string.Format(Texts.MissingListStart, token));

            var startLocation = _lastTokenLocation;
            token = GetNextToken();

            var listValues = new List<ICommandParameter>();
            var listCompleted = false;

            while(token != null)
            {
                if(token is SpecialToken specialToken && specialToken.Type == SpecialTokenType.ListEnd)
                {
                    listCompleted = true;
                    break;
                }

                var value = ParseParameterValue(token);
                if(value != null)
                    listValues.Add(value);
                
                token = GetNextToken();
            }

            if(!listCompleted)
                throw new ParseException(startLocation, Texts.MissingListEnd);

            var parameter = new List(listValues);
            return new SourceValueCommandParameter(startLocation, parameter);
        }

        private SourceValueCommandParameter ParseMap(Token token)
        {
            if(token == null)
                return null;

            if(!(token is SpecialToken specialTokenGuard) || specialTokenGuard.Type != SpecialTokenType.MapStart)
                throw new ParseException(_lastTokenLocation, string.Format(Texts.MissingMapStart, token));

            var startLocation = _lastTokenLocation;
            token = SkipWhiteSpace();

            var mapEntries = new Dictionary<string, ICommandParameter>();
            var mapCompleted = false;

            while(token != null)
            {
                var currentToken = token;

                if(token is SpecialToken specialToken && specialToken.Type == SpecialTokenType.MapEnd)
                {
                    mapCompleted = true;
                    break;
                }

                var keyBlock = ParseName(token, Symbol.SubName);
                if(keyBlock != null)
                {
                    token = SkipWhiteSpace();
                    if(!(token is LiteralToken literalToken && literalToken.Value == Symbol.SubName))
                            throw new ParseException(token.Location, Texts.MissingMapEntryName);

                    var key = keyBlock.Name;

                    if(string.IsNullOrEmpty(key))
                        throw new ParseException(token.Location, Texts.MissingMapEntryName);

                    if(!_nameValidator.IsValid(key))
                        throw new ParseException(token.Location, string.Format(Texts.InvalidCharacterInMapEntryName, key));

                    token = SkipWhiteSpace();

                    var valueBlock = ParseParameterValue(token);
                    if(valueBlock == null)
                        throw new ParseException(token.Location, Texts.MissingMapEntryValue);

                    mapEntries.Add(key, valueBlock);
                }

                token = SkipWhiteSpace(ignoreNewlines: true);

                if(token == currentToken)
                    throw new ParseException(token.Location, Texts.MissingMapEntryName);
            }

            if(!mapCompleted)
                throw new ParseException(startLocation, Texts.MissingMapEnd);

            var parameter = new Map(mapEntries);
            return new SourceValueCommandParameter(startLocation, parameter);
        }

        private CommandInput ParseSubcommand()
        {
            var token = SkipWhiteSpace();
            var expectBlockEnd = false;
            var startLocation = _lastTokenLocation;

            if(token is SpecialToken specialToken && specialToken.Type == SpecialTokenType.BlockStart)
                expectBlockEnd = true;
            else
                PushNextToken(token);

            CommandInput subcommand = null;
            var lastLocation = token?.Location;

            while(subcommand == null && _tokenizer.HasMore)
            {
                subcommand = ParseCommandInput(expectBlockEnd);
                if(lastLocation.Value.Equals(_lastTokenLocation))
                    break;

                // todo: It looks like newline should be a special token. Sometimes newlines are special, and other times they're literals.
                if(_nextToken != null && _nextToken is LiteralToken literalNextToken && literalNextToken.Value == Newline)
                    _nextToken = null;
            }

            if(subcommand == null)
                throw new ParseException(startLocation, Texts.MissingSubcommand);

            var block = subcommand;

            if(expectBlockEnd)
            {
                token = SkipWhiteSpace();
                if(token is SpecialToken specialToken2 && specialToken2.Type == SpecialTokenType.BlockEnd)
                    return block;
                else
                    throw new ParseException(startLocation, Texts.MissingBlockEnd);
            }

            return block;
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

        private Token SkipWhiteSpace(bool ignoreNewlines = false)
        {
            var token = GetNextToken();
            while(token != null)
            {
                if(token is SpecialToken)
                    return token;

                if(token is LiteralToken literalToken &&
                    (!char.IsWhiteSpace(literalToken.Value) || (literalToken.Value == Newline && !ignoreNewlines)))
                {
                    return token;
                }

                token = GetNextToken();
            }

            return null;
        }
    }
}
