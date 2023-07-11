namespace Chel
{
    internal static class Texts
    {
        public const string CannotBindCommandInputWithoutSubstituteValue = "Cannot bind CommandInput with no SubstituteValue";

        public const string CannotBindListToNonListParameter = "Cannot bind a list to a non-list parameter '{0}'";

        public const string CannotBindMapToNonMapParameter = "Cannot bind a map to a non-map parameter '{0}'";

        public const string CannotBindNonListToListParameter = "Cannot bind a non-list to a list parameter '{0}'";

        public const string CannotBindNonMapToMapParameter = "Cannot bind a non-map to a map parameter '{0}'";

        public const string CannotDisplayHelpUnknownCommnad = "Cannot display help for unknown command '{0}'";

        public const string CannotRepeatFlagParameter = "Cannot repeat flag parameter '{0}'";

        public const string CannotRepeatNamedParameter = "Cannot repeat named parameter '{0}'";

        public const string CommandDependencyNotRegistered = "Command dependency implementing {0} has not been registered";

        public const string CommandNameAlreadyUsed = "Command name '{0}' on command type {1} is already used on command type {2}";

        public const string CommandParametersMustBeChelType = "Command parameters must only be of type ChelType";
        
        public const string DescriptorCouldNotBeGenerated = "Descriptor for {0} could not be generated";

        public const string DescriptorForCommandCouldNotBeResolved = "Descriptor for command '{0}' could not be resolved";

        public const string ExpectedLiteral = "Expected a literal";

        public const string FlagParametersCannotBeRequired = "Flag parameters cannot be marked as required ({0})";

        public const string InternalErrorUnknownChelType = "Internal error: Unknown ChelType.";

        public const string InvalidCharacterInMapEntryName = "Invalid character in map entry name '{0}'.";

        public const string InvalidCharacterInVariableName = "Invalid character in variable name '{0}'.";

        public const string InvalidParameterValueForNamedParameter = "Invalid parameter value '{0}' for named parameter '{1}'. {2}";

        public const string InvalidParameterValueForNumberedParameter = "Invalid parameter value '{0}' for numbered parameter '{1}'. {2}";

        public const string KeyTypeMustBeString = "Key type of property '{0}' must be 'string'.";

        public const string ListValuesMustBeChelType = "List values must only be of type ChelType";

        public const string MapValuesMustBeChelType = "Map values must only be of type ChelType";

        public const string MissingBlockEnd = "Missing block end )";

        public const string MissingBlockStart = "Missing block start (";

        public const string MissingCommentBlockEnd = "Missing comment block end.";

        public const string MissingCommentBlockStart = "Missing comment block start.";

        public const string MissingListEnd = "Missing list end ]";

        public const string MissingListStart = "Missing list start [";

        public const string MissingMapEnd = "Missing map end {";

        public const string MissingMapEntryName = "Missing map entry name";

        public const string MissingMapEntryValue = "Missing map entry value";

        public const string MissingMapStart = "Missing map start {";

        public const string MissingParameterName = "Missing parameter name after -";

        public const string MissingRequiredNamedParameter = "Missing required named parameter '{0}'";

        public const string MissingRequiredNumberedParameter = "Missing required numbered parameter '{0}'";

        public const string MissingSubreferenceForVariable = "Missing subreference for variable ${0}$";

        public const string MissingSubcommand = "Missing subcommand";

        public const string MissingValueForNamedParameter = "Missing value for named parameter '{0}'";

        public const string MissingVariableName = "Missing variable name";
        
        public const string ParameterDoesNotImplementICommand = "{0} does not implement ICommand";

        public const string ParameterCannotBeEmpty = "Parameter '{0}' cannot be empty";

        public const string PropertyMissingSetter = "Property {0} on command type {1} requires a setter";
        
        public const string ServiceTypeAlreadyRegistered = "The service type {0} has already been registered";

        public const string SubcommandResultMustBeChelType = "Subcommand result must be of type ChelType";

        public const string TextForCultureAlreadyAdded = "Text for culture '{0}' has already been added";

        public const string TypeIsNotACommand = "{0} is not a command";

        public const string TypeRequiresParameterlessConstructor = "Type {0} requires a parameterless constructor";

        public const string UnexpectedNumberedParameter = "Unexpected numbered parameter '{0}'";

        public const string UnexpectedToken = "Unexpected token '{0}'";

        public const string UnknownCommand = "Unknown command '{0}'";

        public const string UnknownEscapedCharacter = "Unknown escaped character '\\{0}'";

        public const string UnknownFlagParameter = "Unknown flag parameter '{0}'";

        public const string UnknownNamedParameter = "Unknown named parameter '{0}'";

        public const string UnpairedVariableToken = "Unpaired variable token $";

        public const string ValueOfMapEntryNotValue = "Value of map '{0}' entry '{1}' is not a SourceValueCommandParameter.";

        public const string VariableIsNotMap = "Variable '{0}' is not a map and cannot be expanded";

        public const string VariableIsUnset = "Variable ${0}$ is not set";

        public const string VariableNameIsMissing = "Variable name is missing";

        public const string VariableSubreferenceIsInvalid = "Variable ${0}$ subreference '{1}' is invalid";

        public const string VariableSubreferenceIsMissing = "Variable ${0}$ subreference is missing";

        public class PhraseKeys {

            public const string AvailableCommands = "AVAILABLE COMMANDS";

            public const string MissingVariableName = "MISSING VARIABLE NAME";

            public const string NoVariablesSet = "NO VARIABLES SET";

            public const string Required = "REQUIRED";

            public const string Usage = "USAGE";

            public const string VariableHasBeenCleared = "VARIABLE HAS BEEN CLEARED";

            public const string VariableNotSet = "VARIABLE NOT SET";
        }
    }
}