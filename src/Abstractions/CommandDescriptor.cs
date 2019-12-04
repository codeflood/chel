using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Chel.Abstractions
{
    /// <summary>
    /// Describes a command.
    /// </summary>
    public class CommandDescriptor
    {
        private ITextResolver _descriptions = null;

        /// <summary>
        /// Gets the <see cref="Type"/> implementing the command.
        /// </summary>
        public Type ImplementingType { get; }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Gets the numbered parameters of the command.
        /// </summary>
        public IReadOnlyList<NumberedParameterDescriptor> NumberedParameters { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="implementingType">The <see cref="Type"/> implementing the command.</param>
        /// <param name="descriptions">The localised descriptions for the command.</param>
        /// <param name="parameterDescriptors">The descriptors for the parameters for the command.</param>
        public CommandDescriptor(string commandName, Type implementingType, ITextResolver descriptions, IReadOnlyList<ParameterDescriptor> parameterDescriptors)
        {
            if(commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if(commandName == string.Empty)
                throw new ArgumentException(string.Format(Texts.ArgumentCannotBeEmpty, nameof(commandName)), nameof(commandName));

            if(implementingType == null)
                throw new ArgumentNullException(nameof(implementingType));

            if(descriptions == null)
                throw new ArgumentNullException(nameof(descriptions));

            CommandName = commandName;
            ImplementingType = implementingType;
            _descriptions = descriptions;

            if(parameterDescriptors == null)
                NumberedParameters = new List<NumberedParameterDescriptor>();
            else
                NumberedParameters = parameterDescriptors.OfType<NumberedParameterDescriptor>().ToList();
        }

        /// <summary>
        /// Gets a description in the specified culture.
        /// </summary>
        public string GetDescription(string cultureName)
        {
            if(cultureName == null)
                throw new ArgumentNullException(nameof(cultureName));

            if(_descriptions == null)
                return string.Empty;

            return _descriptions.GetText(cultureName);
        }
    }
}