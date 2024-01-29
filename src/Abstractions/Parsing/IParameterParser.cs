using System;
using Chel.Abstractions.Types;

namespace Chel.Abstractions.Parsing
{
    /// <summary>
    /// Defines a parser for parameter values.
    /// </summary>
    public interface IParameterParser
    {
        /// <summary>
        /// Parse a double type from the input.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="parameterName">The name of the parameter being parsed.</param>
        /// <returns>The result of parsing the input.</returns>
        public ParameterParsingResult<double> ParseDouble(ChelType input, string parameterName);

        /// <summary>
        /// Parse a DateTime type from the input.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="parameterName">The name of the parameter being parsed.</param>
        /// <returns>The result of parsing the input.</returns>
        public ParameterParsingResult<DateTime> ParseDateTime(ChelType input, string parameterName);

        /// <summary>
        /// Parse a GUID type from the input.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="parameterName">The name of the parameter being parsed.</param>
        /// <returns>The result of parsing the input.</returns>
        public ParameterParsingResult<Guid> ParseGuid(ChelType input, string parameterName);
    }
}
