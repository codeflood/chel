# Subcommand #

A subcommand is a command that is executed and the output of the command is used as a parameter of another command. A subcommand is prefixed by the `<<` operator.

    command << subcommand

If the subcommand uses parameters, then the subcommand must be wrapped in parentheses.

    command << (subcommand param1 -named paramvalue)

Scripts and subscripts can also be used as a subcommand.

    command << (script -name value)

# Command Chaining #

The value of a command can be passed along to the scope of another command using command chaining. The output of the previous command is made available by the `~` variable, but is only available to the chained command.

    command >> othercommand $~$
