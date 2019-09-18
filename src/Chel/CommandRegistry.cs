using System;
using System.Linq;
using Chel.Abstractions;

namespace Chel
{
    /// <summary>
    /// The default command registry.
    /// </summary>
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Type ICommandType = typeof(ICommand);

        public void Register(Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            var implementsInterface = DoesImplementICommand(type);
            if(!implementsInterface)
                throw new ArgumentException("Type does not implement ICommand.");

            var attribute = ExtractCommandAttribute(type);
            if(attribute == null)
                throw new ArgumentException("Type is not attributed with CommandAttribute.");
        }

        private bool DoesImplementICommand(Type type)
        {
            var interfaces = type.GetInterfaces();

            foreach(var i in interfaces)
            {
                if(i == ICommandType)
                    return true;
            }

            return false;
        }

        private CommandAttribute ExtractCommandAttribute(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(CommandAttribute), true);
            return (CommandAttribute)attributes.FirstOrDefault();
        }

        public Type Resolve(string commandName)
        {
            throw new NotImplementedException();
        }
    }
}