using System;
using System.Globalization;

namespace Chel.Abstractions
{
    public class InvalidCommandNameException : Exception
    {
        public string CommandName { get; private set; }

        public InvalidCommandNameException(string commandName)
        {
            CommandName = commandName;
        }

        public override string ToString()
        {
            if(CommandName == null)
                return ApplicationTextResolver.Instance.Resolve(ApplicationTexts.InvalidCommandNameNull, CultureInfo.CurrentCulture.Name);

            var text = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.InvalidCommandNameWithParameter, CultureInfo.CurrentCulture.Name);
            return string.Format(text, CommandName);
        }
    }
}