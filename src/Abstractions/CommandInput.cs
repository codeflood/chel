using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// A single command input to be executed.
    /// </summary>
    public class CommandInput
    {
        /// <summary>
        /// Gets the name of the command to execute.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Gets the line number the command was parsed from.
        /// </summary>
        public int SourceLine { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="sourceLine">The line number the command was parsed from.</param>
        /// <param name="commandName">The name of the command to execute.</param>
        public CommandInput(int sourceLine, string commandName)
        {
            if(sourceLine <= 0)
                throw new ArgumentException(string.Format(Texts.ArgumentMustBeGreaterThanZero, nameof(sourceLine)), nameof(sourceLine));

            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            SourceLine = sourceLine;
            CommandName = commandName;
        }

        public override bool Equals(object obj)
        {
            if(obj is CommandInput)
            {
                var other = obj as CommandInput;

                return
                    SourceLine == other.SourceLine && 
                    string.Compare(CommandName, other.CommandName, true) == 0;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return
                SourceLine.GetHashCode() + 
                CommandName.ToLower().GetHashCode();
        }
    }
}