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