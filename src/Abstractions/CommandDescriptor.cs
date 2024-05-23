using System;
using System.Collections.Generic;

namespace Chel.Abstractions;

/// <summary>
/// Describes a command.
/// </summary>
public class CommandDescriptor
{
    private readonly ITextResolver _descriptions;

    internal readonly List<NumberedParameterDescriptor> _numberedParameters = new();
    internal readonly Dictionary<string, NamedParameterDescriptor> _namedParameters = new Dictionary<string, NamedParameterDescriptor>(StringComparer.OrdinalIgnoreCase);
    internal readonly List<FlagParameterDescriptor> _flagParameters = new();

    /// <summary>
    /// Gets the <see cref="Type"/> implementing the command.
    /// </summary>
    public Type ImplementingType { get; }

    /// <summary>
    /// Gets the identifier of the command.
    /// </summary>
    public ExecutionTargetIdentifier CommandIdentifier { get; }

    /// <summary>
    /// Gets the numbered parameters of the command.
    /// </summary>
    public IReadOnlyList<NumberedParameterDescriptor> NumberedParameters => _numberedParameters;

    /// <summary>
    /// Gets the named parameters of the command.
    /// </summary>
    public IReadOnlyDictionary<string, NamedParameterDescriptor> NamedParameters => _namedParameters;

    /// <summary>
    /// Gets the flag parameters of the command.
    /// </summary>
    public IReadOnlyList<FlagParameterDescriptor> FlagParameters => _flagParameters;

    private CommandDescriptor(ExecutionTargetIdentifier commandIdentifier, Type implementingType, ITextResolver descriptions)
    {
        CommandIdentifier = commandIdentifier;
        ImplementingType = implementingType;
        _descriptions = descriptions;
    }

    /// <summary>
    /// Gets a description in the specified culture.
    /// </summary>
    public string? GetDescription(string cultureName)
    {
        if(cultureName == null)
            throw new ArgumentNullException(nameof(cultureName));

        if(_descriptions == null)
            return string.Empty;

        return _descriptions.GetText(cultureName);
    }

    /// <summary>
    /// Builds instances of <see cref="CommandDescriptor" />.
    /// </summary>
    public class Builder
    {
        private readonly CommandDescriptor _commandDescriptor;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="implementingType">The <see cref="Type"/> implementing the command.</param>
        /// <param name="descriptions">The localised descriptions for the command.</param>
        public Builder(ExecutionTargetIdentifier commandIdentifier, Type implementingType, ITextResolver descriptions)
        {
            if(implementingType == null)
                throw new ArgumentNullException(nameof(implementingType));

            if(descriptions == null)
                throw new ArgumentNullException(nameof(descriptions));

            _commandDescriptor = new CommandDescriptor(commandIdentifier, implementingType, descriptions);
        }

        /// <summary>
        /// Adds a numbered parameter descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor to add.</param>
        public void AddNumberedParameter(NumberedParameterDescriptor descriptor)
        {
            if(descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var existing = _commandDescriptor._numberedParameters.Find(x => x.Number == descriptor.Number);
            if(existing != null)
                throw ExceptionFactory.CreateInvalidOperationException(ApplicationTexts.DescriptorAlreadyAdded, existing.Number);

            _commandDescriptor._numberedParameters.Add(descriptor);
        }

        /// <summary>
        /// Adds a named parameter descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor to add.</param>
        public void AddNamedParameter(NamedParameterDescriptor descriptor)
        {
            if(descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if(_commandDescriptor._namedParameters.ContainsKey(descriptor.Name))
                throw ExceptionFactory.CreateInvalidOperationException(ApplicationTexts.DescriptorAlreadyAdded, descriptor.Name);

            _commandDescriptor._namedParameters.Add(descriptor.Name, descriptor);
        }

        /// <summary>
        /// Adds a flag parameter descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor to add.</param>
        public void AddFlagParameter(FlagParameterDescriptor descriptor)
        {
            if(descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var found = _commandDescriptor._flagParameters.Exists(x => string.Equals(x.Name, descriptor.Name, StringComparison.OrdinalIgnoreCase));
            if(found)
                throw ExceptionFactory.CreateInvalidOperationException(ApplicationTexts.DescriptorAlreadyAdded, descriptor.Name);

            _commandDescriptor._flagParameters.Add(descriptor);
        }

        /// <summary>
        /// Builds a <see cref="CommandDescriptor" /> instance from the set data.
        /// </summary>
        public CommandDescriptor Build()
        {
            return _commandDescriptor;
        }
    }
}
