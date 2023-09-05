using System;
using System.Globalization;

namespace Chel.Abstractions
{
    /// <summary>
    /// A factory to create exceptions with localized messages.
    /// </summary>
    public static class ExceptionFactory
    {
        /// <summary>
        /// Create a new <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="textKey">The key of the ApplicationTexts to set as the message.</param>
        /// <param name="paramName">The name of the argument the exception is for.</param>
        /// <param name="args">Any args for formatting the message text.</param>
        /// <returns>A new exception instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="textKey"/> or <paramref name="paramName"/> are null.</exception>
        /// <exception cref="ArgumentException"><paramref name="textKey"/> or <paramref name="paramName"/> are empty.</exception>
        public static ArgumentException CreateArgumentException(string textKey, string paramName, params object[] args)
        {
            if(textKey == null)
                throw new ArgumentNullException(nameof(textKey));

            if(paramName == null)
                throw new ArgumentNullException(nameof(paramName));

            var emptyArgMessage = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.ArgumentCannotBeEmpty, CultureInfo.CurrentCulture.Name);

            if(textKey == string.Empty)
                throw new ArgumentException(string.Format(emptyArgMessage, nameof(textKey)), nameof(textKey));
            
            if(paramName == string.Empty)
                throw new ArgumentException(string.Format(emptyArgMessage, nameof(paramName)), nameof(paramName));

            var text = ApplicationTextResolver.Instance.Resolve(textKey, CultureInfo.CurrentCulture.Name);

            if(args.Length > 0)
                text = string.Format(text, args);

            return new ArgumentException(text, paramName);
        }

        /// <summary>
        /// Create a new <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="textKey">The key of the ApplicationTexts to set as the message.</param>
        /// <param name="args">Any args for formatting the message text.</param>
        /// <returns>A new exception instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="textKey"/> are null.</exception>
        /// <exception cref="ArgumentException"><paramref name="textKey"/> are empty.</exception>
        public static InvalidOperationException CreateInvalidOperationException(string textKey, params object[] args)
        {
            if(textKey == null)
                throw new ArgumentNullException(nameof(textKey));

            var emptyArgMessage = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.ArgumentCannotBeEmpty, CultureInfo.CurrentCulture.Name);

            if(textKey == string.Empty)
                throw new ArgumentException(string.Format(emptyArgMessage, nameof(textKey)), nameof(textKey));

            var text = ApplicationTextResolver.Instance.Resolve(textKey, CultureInfo.CurrentCulture.Name);

            if(args.Length > 0)
                text = string.Format(text, args);
                
            return new InvalidOperationException(text);
        }
    }
}