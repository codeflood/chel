# Set #

The `set` command is used to set and clear variables.

If executed with no parameters, all variables in the current scope are listed with their values.

    > set
    a : b
    list : 0 1 2 3 4
    map : {
        a : b
    }

To set a variable value, pass the variable name as the first numbered parameter and the value as the second.

    set name value

To clear a variable, use the `-c` flag parameter.

    set -c name

Variables can be made globally accessible by using the `-g` flag parameter.

    set -g list [a b c]
