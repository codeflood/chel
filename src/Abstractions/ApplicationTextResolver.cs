using System.Collections.Generic;
using System.Globalization;

namespace Chel.Abstractions
{
    /// <summary>
    /// Resolves application texts by key.
    /// </summary>
    public class ApplicationTextResolver
    {
        private static object _instanceCreationLock = new();
        private static ApplicationTextResolver _instance;

        private Dictionary<string, string> _defaultTexts;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ApplicationTextResolver Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_instanceCreationLock)
                    {
                        if(_instance == null)
                            _instance = new ApplicationTextResolver();
                    }
                }

                return _instance;
            }
        }

        private ApplicationTextResolver()
        {
            _defaultTexts = new()
            {
                { ApplicationTexts.ArgumentCannotBeEmpty, "'{0}' cannot be empty" },
                { ApplicationTexts.ArgumentCannotBeEmptyOrWhitespace, "'{0}' cannot be empty or whitespace" },
                { ApplicationTexts.ArgumentCannotBeNull, "'{0}' cannot be null" },
                { ApplicationTexts.ArgumentNotValidCulture, "'{0}' is not a valid culture" },
                { ApplicationTexts.ArgumentMustBeGreaterThanZero, "'{0}' must be greater than 0" },
                { ApplicationTexts.ArgumentCannotBeNullOrEmpty, "'{0}' cannot be null or empty" },
                { ApplicationTexts.AvailableCommands, "Available commands" },
                { ApplicationTexts.CannotBindCommandInputWithoutSubstituteValue, "Cannot bind CommandInput with no SubstituteValue" },
                { ApplicationTexts.CannotBindListToNonListParameter, "Cannot bind a list to a non-list parameter '{0}'" },
                { ApplicationTexts.CannotBindMapToNonMapParameter, "Cannot bind a map to a non-map parameter '{0}'" },
                { ApplicationTexts.CannotBindNonListToListParameter, "Cannot bind a non-list to a list parameter '{0}'" },
                { ApplicationTexts.CannotBindNonMapToMapParameter, "Cannot bind a non-map to a map parameter '{0}'" },
                { ApplicationTexts.CannotDisplayHelpUnknownCommnad, "Cannot display help for unknown command '{0}'" },
                { ApplicationTexts.CannotRepeatFlagParameter, "Cannot repeat flag parameter '{0}'" },
                { ApplicationTexts.CannotRepeatNamedParameter, "Cannot repeat named parameter '{0}'" },
                { ApplicationTexts.CannotSetMultipleFlags, "Cannot set multiple flags '{0}' at once." },
                { ApplicationTexts.CommandDependencyNotRegistered, "Command dependency implementing {0} has not been registered" },
                { ApplicationTexts.CommandNameAlreadyUsed, "Command name '{0}' on command type {1} is already used on command type {2}" },
                { ApplicationTexts.CommandParametersMustBeChelType, "Command parameters must only be of type ChelType" },
                { ApplicationTexts.CompoundValueOnlyConsistsLiteralsAndVariables, "Compound values can only consist of literals and variable references." },
                { ApplicationTexts.CouldNotParseDate, "Could not parse '{0}' as a date." },
                { ApplicationTexts.CouldNotParseGuid, "Could not parse '{0}' as a GUID." },
                { ApplicationTexts.CouldNotParseNumber, "Could not parse '{0}' as a number." },
                { ApplicationTexts.DescriptorAlreadyAdded, "Descriptor '{0}' has already been added." },
                { ApplicationTexts.DescriptorCouldNotBeGenerated, "Descriptor for {0} could not be generated" },
                { ApplicationTexts.DescriptorForCommandCouldNotBeResolved, "Descriptor for command '{0}' could not be resolved" },
                { ApplicationTexts.ErrorAtLocation, "ERROR (line {0}, character {1})" },
                { ApplicationTexts.ExpectedLiteral, "Expected a literal" },
                { ApplicationTexts.FlagParametersCannotBeRequired, "Flag parameters cannot be marked as required ({0})" },
                { ApplicationTexts.InternalErrorUnknownChelType, "Internal error: Unknown ChelType." },
                { ApplicationTexts.InvalidCharacterInMapEntryName, "Invalid character in map entry name '{0}'." },
                { ApplicationTexts.InvalidCharacterInVariableName, "Invalid character in variable name '{0}'." },
                { ApplicationTexts.InvalidCommandNameNull, "The command name null is invalid" },
                { ApplicationTexts.InvalidCommandNameWithParameter, "The command name '{0}' is invalid" },
                { ApplicationTexts.InvalidParameterValueForNamedParameter, "Invalid parameter value '{0}' for named parameter '{1}'. {2}" },
                { ApplicationTexts.InvalidParameterValueForNumberedParameter, "Invalid parameter value '{0}' for numbered parameter '{1}'. {2}" },
                { ApplicationTexts.KeyTypeMustBeString, "Key type of property '{0}' must be 'string'." },
                { ApplicationTexts.ListValuesMustBeChelType, "List values must only be of type ChelType" },
                { ApplicationTexts.MapValuesMustBeChelType, "Map values must only be of type ChelType" },
                { ApplicationTexts.MissingBlockEnd, "Missing block end )" },
                { ApplicationTexts.MissingBlockStart, "Missing block start (" },
                { ApplicationTexts.MissingCommentBlockEnd, "Missing comment block end." },
                { ApplicationTexts.MissingCommentBlockStart, "Missing comment block start." },
                { ApplicationTexts.MissingListEnd, "Missing list end ]" },
                { ApplicationTexts.MissingListStart, "Missing list start [" },
                { ApplicationTexts.MissingMapEnd, "Missing map end {" },
                { ApplicationTexts.MissingMapEntryName, "Missing map entry name" },
                { ApplicationTexts.MissingMapEntryValue, "Missing map entry value" },
                { ApplicationTexts.MissingMapStart, "Missing map start {" },
                { ApplicationTexts.MissingParameterName, "Missing parameter name after -" },
                { ApplicationTexts.MissingRequiredNamedParameter, "Missing required named parameter '{0}'" },
                { ApplicationTexts.MissingRequiredNumberedParameter, "Missing required numbered parameter '{0}'" },
                { ApplicationTexts.MissingSubreferenceForVariable, "Missing subreference for variable ${0}$" },
                { ApplicationTexts.MissingSubcommand, "Missing subcommand" },
                { ApplicationTexts.MissingValueForNamedParameter, "Missing value for named parameter '{0}'" },
                { ApplicationTexts.MissingVariableName, "Missing variable name." },
                { ApplicationTexts.NoVariablesSet, "No variables set." },
                { ApplicationTexts.NumberedParameterNotSet, "Numbered parameter {0} is not set." },
                { ApplicationTexts.ParameterDoesNotImplementICommand, "{0} does not implement ICommand" },
                { ApplicationTexts.ParameterCannotBeEmpty, "Parameter '{0}' cannot be empty" },
                { ApplicationTexts.PropertyMissingSetter, "Property {0} on command type {1} requires a setter" },
                { ApplicationTexts.Required, "Required" },
                { ApplicationTexts.ServiceTypeAlreadyRegistered, "The service type {0} has already been registered" },
                { ApplicationTexts.SpecificCommandHelp, "For help on a specific command pass the name of the command to the 'help' command." },
                { ApplicationTexts.SubcommandResultMustBeChelType, "Subcommand result must be of type ChelType" },
                { ApplicationTexts.TextForCultureAlreadyAdded, "Text for culture '{0}' has already been added" },
                { ApplicationTexts.TypeIsNotACommand, "{0} is not a command" },
                { ApplicationTexts.TypeRequiresParameterlessConstructor, "Type {0} requires a parameterless constructor" },
                { ApplicationTexts.UnexpectedParameterType, "Unexpected parameter type" },
                { ApplicationTexts.UnexpectedNumberedParameter, "Unexpected numbered parameter '{0}'" },
                { ApplicationTexts.UnexpectedToken, "Unexpected token '{0}'" },
                { ApplicationTexts.UnknownCommand, "Unknown command '{0}'" },
                { ApplicationTexts.UnknownEscapedCharacter, "Unknown escaped character '\\{0}'" },
                { ApplicationTexts.UnknownFlagParameter, "Unknown flag parameter '{0}'" },
                { ApplicationTexts.UnknownNamedParameter, "Unknown named parameter '{0}'" },
                { ApplicationTexts.UnpairedVariableToken, "Unpaired variable token $" },
                { ApplicationTexts.Usage, "usage" },
                { ApplicationTexts.ValueOfMapEntryNotValue, "Value of map '{0}' entry '{1}' is not a SourceValueCommandParameter." },
                { ApplicationTexts.VariableHasBeenCleared, "Variable '{0}' has been cleared." },
                { ApplicationTexts.VariableIsNotMap, "Variable '{0}' is not a map and cannot be expanded" },
                { ApplicationTexts.VariableIsUnset, "Variable ${0}$ is not set" },
                { ApplicationTexts.VariableNameIsMissing, "Variable name is missing" },
                { ApplicationTexts.VariableNotSet, "Variable '{0}' is not set." },
                { ApplicationTexts.VariableSubreferenceIsInvalid, "Variable ${0}$ subreference '{1}' is invalid" },
                { ApplicationTexts.VariableSubreferenceIsMissing, "Variable ${0}$ subreference is missing" }
            };
        }

        /// <summary>
        /// Resolve text by key for a given culture.
        /// </summary>
        /// <param name="key">The key of the text to resolve.</param>
        /// <returns>The resolved text, or the key if the text could not be resolved.</returns>
        public string Resolve(string key)
        {
            if(string.IsNullOrEmpty(key))
                return string.Empty;
            
            // todo: Add culture overloading from files
            var effectiveCultureName = CultureInfo.CurrentCulture.Name;

            if(_defaultTexts.ContainsKey(key))
                return _defaultTexts[key];
            
            return key;
        }

        /// <summary>
        /// Resolve text by key for a given culture and format the result with the provided arguments.
        /// </summary>
        /// <param name="key">The key of the text to resolve.</param>
        /// <param name="args">The arguments to format the resolved text with.</param>
        /// <returns>The resolved and formatted text, or the key if the text could not be resolved.</returns>
        public string ResolveAndFormat(string key, params object[] args)
        {
            var text = Resolve(key);
            return string.Format(text, args);
        }
    }
}
