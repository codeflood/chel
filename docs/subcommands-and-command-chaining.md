# Subcommand #

A subcommand is a command that is executed and the output of the command is used as a parameter of another command. A subcommand is prefixed by the `<<` operator.

    command << subcommand

If the subcommand uses parameters, then the subcommand must be 1enclosed in round brackets.

    command << (subcommand param1 -named paramvalue)

Scripts and subscripts can also be used as a subcommand.

    command << (script -name value)

Subcommands can even be used inside of lists and maps.

    var list [1 2 << random]
    var map {
        id: << newid
    }

# Command Chaining #

The value of a command can be passed along to the scope of another command using command chaining. The output of the previous command is made available by the `~` variable, but is only available to the chained command.

    command >> othercommand $~$

Another idea: to help reduce the number of esoteric symbols, what if the user specified the name of the variable in which to put the previous commands output?

    command >p> othercommand $p$

This is a little like a ruby pipe.

    command |p| othercommand $p$

    cmd << subcmd >p> outer $p$
    
    cmd << subcmd |p| outer $p$
