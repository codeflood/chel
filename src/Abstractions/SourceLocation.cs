using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// A location in the source input.
    /// </summary>
    public struct SourceLocation
    {
        /// <summary>
        /// A SourceLocation indicating the current location.
        /// </summary>
        public static SourceLocation CurrentLocation = new SourceLocation
        {
            LineNumber = -1,
            CharacterNumber = -1
        };

        /// <summary>
        /// Gets the line number for the location.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the character position for the location.
        /// </summary>
        public int CharacterNumber { get; private set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="lineNumber">The line number for the location.</param>
        /// <param name="characterNumber">The character position for the location.</param>
        public SourceLocation(int lineNumber, int characterNumber)
        {
            if(lineNumber <= 0)
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentMustBeGreaterThanZero, nameof(lineNumber), nameof(lineNumber));

            if(characterNumber <= 0)
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentMustBeGreaterThanZero, nameof(characterNumber), nameof(characterNumber));

            LineNumber = lineNumber;
            CharacterNumber = characterNumber;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SourceLocation))
            {
                return false;
            }

            var other = (SourceLocation)obj;
            
            return
                other.LineNumber.Equals(LineNumber) &&
                other.CharacterNumber.Equals(CharacterNumber);
        }
        
        public override int GetHashCode()
        {
            return (LineNumber, CharacterNumber).GetHashCode();
        }

        public override string ToString()
        {
            return $"({LineNumber}, {CharacterNumber})";
        }
    }
}
