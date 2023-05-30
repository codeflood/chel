# chel

A simple scripting environment for .net applications.

> :warning: **Work in Progress:** This project is currently a work in progress and is not ready to be consumed. Any aspect of the code or language specification may change during initial development.

Chel is modelled after shell scripting languages like the Windows command prompt, or Bash, to provide a simple and intuative scripting environment.

Chel in a nutshell:

    # This is a comment

    # Set a variable
    var varname value

    # Execute commands
    command numparam -parameter $varname$ -flag
    command << subcommand
    command >> chained-command $~$

    (#
        block comment
        block comment
    #)

    # Set a list variable
    var list [1 2 3 4]

    # Set a map variable
    var params {
        key1 : value1
        key2 : value2
    }

    var moreParams {
        foo : $list$
        bar : $params$
    }

    # Bind map entries to command parameters with map expansion
    command $*moreparams$ -flag -param $list:2$

    # Define a subcommand
    sub mysub (
        command -flag
        command -param value
    )

    # Call the subcommand
    rigel
