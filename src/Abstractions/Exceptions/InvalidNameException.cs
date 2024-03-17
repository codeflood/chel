using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// An exception indicating a given name is not valid.
    /// </summary>
    public class InvalidNameException : Exception
    {
        /// <summary>
        /// Gets the name that is not valid.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="name">The name that is not valid.</param>
        public InvalidNameException(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            if(Name == null)
                return ApplicationTextResolver.Instance.Resolve(ApplicationTexts.InvalidNameNull);

            var text = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.InvalidNameWithParameter);
            return string.Format(text, Name);
        }
    }
}