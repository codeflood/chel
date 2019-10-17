using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="INameValidator" />.
    /// </summary>
    public class NameValidator : INameValidator
    {
        /// <summary>
        /// Determines whether the name is valid.
        /// </summary>
        /// <param name="name">The name to check.</param>
        public bool IsValid(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return false;

            if(name.StartsWith("-"))
                return false;

            return true;
        }
    }
}