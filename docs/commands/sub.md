# Sub #

Create and manage subscripts.

To create a new subscript, pass the name of the subscript and the script block as numbered parameters.

    sub getid (
        // commands
    )

The `return` command can be used to return a single variable from the subscript.

    sub getid (
        return (random 1 5)
    )

A subscript can be deleted by using the `-d` parameter.

    sub -d getid
