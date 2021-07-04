namespace Chel
{
    internal static class Texts
    {
        public const string ArgumentCannotContainEmptyElement = "'{0}' cannot contain an empty element";

        public const string CommandDependencyNotRegistered = "Command dependency implementing {0} has not been registered";

        public const string CommandNameAlreadyUsed = "Command name '{0}' on command type {1} is already used on command type {2}";

        public const string CannotDisplayHelpUnknownCommnad = "Cannot display help for unknown command '{0}'";

        public const string CannotRepeatFlagParameter = "Cannot repeat flag parameter '{0}'";

        public const string CannotRepeatNamedParameter = "Cannot repeat named parameter '{0}'";
        
        public const string DescriptorCouldNotBeGenerated = "Descriptor for {0} could not be generated";

        public const string DescriptorForCommandCouldNotBeResolved = "Descriptor for command '{0}' could not be resolved";

        public const string FlagParametersCannotBeRequired = "Flag parameters cannot be marked as required ({0})";

        public const string InvalidParameterValueForNamedParameter = "Invalid parameter value '{0}' for named parameter '{1}'.";

        public const string InvalidParameterValueForNumberedParameter = "Invalid parameter value '{0}' for numbered parameter '{1}'.";

        public const string MissingClosingParenthesis = "Missing closing parenthesis )";

        public const string MissingOpeningParenthesis = "Missing opening parenthesis (";

        public const string MissingParameterName = "Missing parameter name after -";

        public const string MissingRequiredNamedParameter = "Missing required named parameter '{0}'";

        public const string MissingRequiredNumberedParameter = "Missing required numbered parameter '{0}'";

        public const string MissingValueForNamedParameter = "Missing value for named parameter '{0}'";
        
        public const string ParameterDoesNotImplementICommand = "{0} does not implement ICommand";

        public const string ParameterCannotBeEmpty = "Parameter '{0}' cannot be empty";

        public const string PropertyMissingSetter = "Property {0} on command type {1} requires a setter";
        
        public const string ServiceTypeAlreadyRegistered = "The service type {0} has already been registered";

        public const string TextForCultureAlreadyAdded = "Text for culture '{0}' has already been added";

        public const string TypeIsNotACommand = "{0} is not a command";

        public const string TypeRequiresParameterlessConstructor = "Type {0} requires a parameterless constructor";

        public const string UnexpectedNumberedParameter = "Unexpected numbered parameter '{0}'";

        public const string UnknownCommand = "Unknown command";

        public const string UnknownEscapedCharacter = "Unknown escaped character '{0}'";

        public const string UnknownFlagParameter = "Unknown flag parameter '{0}'";

        public const string UnknownNamedParameter = "Unknown named parameter '{0}'";

        public const string UnpairedVariableToken = "Unpaired variable token $";

        public const string VariableIsUnset = "Variable ${0}$ is not set";

        public class PhraseKeys {

            public const string AvailableCommands = "AVAILABLE COMMANDS";

            public const string NoVariablesSet = "NO VARIABLES SET";

            public const string Required = "REQUIRED";

            public const string Usage = "USAGE";

            public const string VariableNotSet = "VARIABLE NOT SET";
        }
    }
}