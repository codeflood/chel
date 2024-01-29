using System;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Types;

namespace Chel.Parsing
{
    /// <summary>
    /// The default implementation of the <see cref="IParameterParser"/>.
    /// </summary>
    public class ParameterParser : IParameterParser
    {
        public ParameterParsingResult<DateTime> ParseDateTime(ChelType input, string parameterName)
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseDate);
            var parseText = parameterName;

            if(input is Literal literalInput)
            {
                if(DateTime.TryParse(literalInput.Value, out var parsedValue))
                    return new ParameterParsingResult<DateTime>(parsedValue);
                
                parseText = literalInput.Value;
            }
            
            return new ParameterParsingResult<DateTime>(string.Format(message, parseText));
        }

        public ParameterParsingResult<double> ParseDouble(ChelType input, string parameterName)
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseNumber);
            var parseText = parameterName;

            if(input is Literal literalInput)
            {
                if(double.TryParse(literalInput.Value, out var parsedValue))
                    return new ParameterParsingResult<double>(parsedValue);

                parseText = literalInput.Value;
            }
            
            return new ParameterParsingResult<double>(string.Format(message, parseText));
        }

        public ParameterParsingResult<Guid> ParseGuid(ChelType input, string parameterName)
        {
            var message = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.CouldNotParseGuid);
            var parseText = parameterName;

            if(input is Literal literalInput)
            {
                if(Guid.TryParse(literalInput.Value, out var parsedValue))
                    return new ParameterParsingResult<Guid>(parsedValue);

                parseText = literalInput.Value;
            }
            
            return new ParameterParsingResult<Guid>(string.Format(message, parseText));
        }
    }
}
