using System;

namespace Chel.Exceptions
{
    /// <summary>
    /// An <see cref="Exception" /> indicating a provided type is not a command implementation.
    /// </summary>
    public class TypeNotACommandException : Exception
    {
        /// <summary>
        /// Gets the <see cref="Type" /> that was attempted to be used as a command implementation.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that was attempted to be used as a command implementation.</param>
        public TypeNotACommandException(Type type)
            : base(string.Format(Texts.TypeIsNotACommand, type?.FullName))
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
        }
    }
}