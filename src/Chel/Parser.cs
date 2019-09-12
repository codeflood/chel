using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default parser.
    /// </summary>
    public class Parser : IParser
    {
        public IList<CommandInput> Parse(string input)
        {
            var parsedLines = new List<CommandInput>();

            if(string.IsNullOrEmpty(input))
                return parsedLines;

            var reader = new StringReader(input);

            CommandInput parsedLine = null;

            do
            {
                parsedLine = ParseSingleLine(reader);

                if(parsedLine != null)
                    parsedLines.Add(parsedLine);
            }
            while(parsedLine != null);

            return parsedLines;
        }

        private CommandInput ParseSingleLine(StringReader reader)
        {
            var commandName = new StringBuilder();
            var stop = false;

            while(!stop)
            {
                var character = reader.Read();
                
                if(character == -1)
                    stop = true;
                else
                {
                    if(character == '\n')
                        stop = true;
                    else if(!char.IsWhiteSpace((char)character))
                        commandName.Append((char)character);
                }                
            }

            var parsedCommandName = commandName.ToString();

            // Trim last \r in case it was a Windows line ending
            if(parsedCommandName.Length > 0 &&
                parsedCommandName[parsedCommandName.Length - 1] == '\r')
                parsedCommandName = parsedCommandName.Substring(0, parsedCommandName.Length - 1);
            
            if(string.IsNullOrEmpty(parsedCommandName))
                return null;

            return new CommandInput(parsedCommandName);
        }
    }
}
