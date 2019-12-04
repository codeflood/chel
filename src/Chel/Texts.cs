namespace Chel
{
    internal static class Texts
    {
        public const string ArgumentCannotBeEmpty = "{0} cannot be empty";

        public const string ArgumentIsNotACommand = "{0} is not a command";

        public const string AvailableCommands = "Available commands";

        public const string CommandNameAlreadyUsed = "Command name '{0}' on command type {1} is already used on command type {2}";

        public const string CommandServiceNotRegistered = "Command service implementing {0} has not been registered";

        public const string DescriptorCouldNotBeGenerated = "Descriptor for {0} could not be generated";
        
        public const string ParameterDoesNotImplementICommand = "{0} does not implement ICommand";

        public const string ParameterIsNotAttributedWithCommandAttribute = "{0} is not attributed with CommandAttribute";

        public const string ServiceTypeAlreadyRegistered = "The service type {0} has already been registered";

        public const string TextForCultureAlreadyAdded = "Text for culture {0} has already been added";

        public const string UnknownCommand = "Unknown command";
        
        public const string UnknownNumberedParameter = "Unknown numbered parameter {0}";
    }
}