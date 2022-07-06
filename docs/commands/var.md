# Var #

The `var` command is used to interact with variables.

If executed with no parameters, all variables in the current scope are listed with their values.

    > var
    a : b
    list : 0 1 2 3 4
    map : {
        a : b
    }

To retrieve the value of a single variable, pass the name of the variable as the first numbered parameter.

    var name

To set a variable value, pass the variable name as the first numbered parameter and the value as the second.

    var name value

To clear a variable, use the `-clear` flag parameter.

    var -clear name

Variables can be made globally accessible by using the `-g` flag parameter.

    var -g list [a b c]
