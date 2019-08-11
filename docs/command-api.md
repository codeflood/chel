# Command API #

## Command Lifetime ##

Commands are instantiated when they're invoked. After the invocation they're destroyed.

## Dependency Injection ##

Chel supports constructor dependency injection for commands. The host application can register dependencies through the host API. When the command in instantiated, any constructor parameters are drawn from the registered objects.

    var dep = new SomeObject();
    session.RegisterDependency(dep);

    [Command("mycommand")]
    public class MyCommand : ICommand
    {
        public MyCommand(SomeObject dep)
        {
        }
    }

## Parameters ##

Parameters passed to a command are bound to properties of the command class. Attributes are used to map between the chel parameter and the property.

    [FlagParameter("flag", "description of the parameter")]
    public bool Flag { get; set; }

Parameters can have multiple mappings to account for short and long formats. Use the `ParameterAlias` attribute to add an additional mapping.

    [FlagParameter("flag", "description of the parameter")]
    [ParameterAlias("f")]
    public bool Flag { get; set; }
