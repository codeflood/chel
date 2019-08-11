# Bind #

`bind` is used to bind a new command into the current session.

To bind a command class currently available in the application, pass the type name to the command.

    bind MyNamespace.MyCommand

To bind all commands found inside an assembly, just pass the name of the assembly to the `-a` parameter.

    bind -a MyAssembly

Commands can also be packaged into a normal zip file, and loaded using the `-z` parameter. The path to the zip file can be a local path (which is accessible to the runtime) or a URL.

    bind -z tmp/commands.zip
    bind -z http://www.codeflood.net/chel/extras.zip
