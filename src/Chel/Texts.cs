namespace Chel
{
    internal static class Texts
    {
        public const string AvailableCommands = "Available commands";

        public const string CommandNameAlreadyUsed = "Command name '{0}' on command type {1} is already used on command type {2}";

        public const string CommandServiceNotRegistered = "Command service implementing {0} has not been registered";

        public const string ParameterDoesNotImplementICommand = "{0} does not implement ICommand";

        public const string ParameterIsNotAttributedWithCommandAttribute = "{0} is not attributed with CommandAttribute";

        public const string ServiceTypeAlreadyRegistered = "The service type {0} has already been registered";

        public const string UnknownCommand = "Unknown command";
        
        public const string UnknownNumberedParameter = "Unknown numbered parameter {0}";
    }
}