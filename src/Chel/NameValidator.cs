using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="INameValidator" />.
    /// </summary>
    public class NameValidator : INameValidator
    {
        private static char[] s_invalidCharacters = {
            Symbol.Escape,
            Symbol.BlockStart,
            Symbol.BlockEnd,
            Symbol.Comment,
            Symbol.Variable,
            Symbol.ListStart,
            Symbol.ListEnd,
            Symbol.MapStart,
            Symbol.MapEnd,
            Symbol.SubName
        };

        /// <summary>
        /// Determines whether the name is valid.
        /// </summary>
        /// <param name="name">The name to check.</param>
        public bool IsValid(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return false;

            if(name[0] == Symbol.ParameterName || name[0] == Symbol.Expansion)
                return false;

            if(name.IndexOfAny(s_invalidCharacters) >= 0)
                return false;

            foreach(var c in name)
            {
                if(char.IsWhiteSpace(c))
                    return false;
            }

            return true;
        }
    }
}