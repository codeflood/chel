# Structures #

Chel is case-insensitive for input other than values (parameter or variable). Commands, scritps, subscripts and parameter names are not case sensitive. The following commands are all equivalent.

    command -param value
    COMMAND -Param value
    Command -paRAM value

## Command ##

Chel is a space delimited language.

    command parameter1 parameter2

To include a space in a parameter, enclose the parameter in brackets.

    command (parameter with space)

Each line starts with a command and is followed by parameters.

## Script ##

A script is a collection of commands which can be invoked by some name. All semantics about how commands operate are also applicable to scripts. A script can accept parameters and also return values.

Scripts are supplied by the host applicable. They may originate from a text file, or other storage the host applicaiton offers.

todo: how to define help in the script file.

## Subscript ##

A subscript is a script defined inside another script. It's like a function in other languages.

To define a subscript, use the `sub` command:

    sub subname body

It's highly likely the body of the subscript will include spaces and span multiple lines, so surround the body with brackets.

    sub get-id (

    )

By default the subscript is only accessible within the script that defined it. To make the subscript available outside the defining script, use the `-g` parameter with the `sub` command to define it as a global subscript.

ex

### Return Values ###

To return a value from a script or subscript, use the `return` command.

    sub get-name (
        value num << math:random
        return text:join name {num}
    )

### Parameters ###

Use the `param` command to read parameters that were passed to the script or subscript.

    param name -numbered 1 -optional
    param name -named inputname
