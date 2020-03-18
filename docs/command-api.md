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

### Required Parameters ###

A parameter can be marked as required by annotating it with the `Required` attribute.

    [Required]
    [NumberedParameter(1, "param")]
    public string NumberedParameter { get; set; }

Required parameters must be provided when the command is invoked, otherwise an error will occur.

## Descriptions ##

To support the help system, command implementations can add the `Description` attribute to the class and all parameters.

    [Command("mycommand")]
    [Description("This command does something")]
    public class MyCommand : ICommand
    {
    }

Descriptions can be added for specific cultures by specifying the culture name as the second parameter in the `Description` attribute.

    [Command("mycommand")]
    [Description("This command does something")]
    [Description("This command does something for en", "en")]
    [Description("This command does something for en-AU", "en-AU")]
    public class MyCommand : ICommand
    {
    }

When resolving descriptions for members, if a specific culture doesn't have a description provided, the parent culture of the requested culture will be tried instead. If no descriptions are found for the culture, the description with no culture specified (invariant culture) will be used.
