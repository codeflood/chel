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
            var commandName = new StringBuilder();
            var stop = false;
            var ignore = false;

            while(!stop)
            {
                var character = reader.Read();
                
                if(character == -1)
                    stop = true;
                else
                {
                    if(character == '\n')
                        stop = true;
                    else if(character == '#')
                        ignore = true;
                    else if(!ignore && !char.IsWhiteSpace((char)character))
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

            return new CommandInput(sourceLine, parsedCommandName);
        }
    }
}
