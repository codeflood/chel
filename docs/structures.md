# Structures #

## Identifiers ##

Identifiers of commands, scripts, subscripts and named parameters are case-insensitive. Any characters can be used in an identifier, as long as it's not reserved, such as those used for grouping (brackets), delimits (spaces) or other special purposes (subcommands).

The following commands are all equivalent.

    command -param value
    COMMAND -Param value
    Command -paRAM value

## Comment ##

Comments are preceeded by a hash (`#`) character. Anything after the character is ignored.

    # this is a comment
    command # all this text is ignored.

A comment block is started with the `#>` token and continues until the `<#` token is encountered.

    #> ignore all this
    and all this, up to
    this <#

## Command ##

A command line always starts with a command name and is followed by any parameters for the command. A single command line is delimited but a newline.

Chel is a space delimited language.

    command parameter1 parameter2

To include a space in a parameter, enclose the parameter in parentheses.

    command (parameter with space)

Parentheses can also span multiple lines, to allow multi-line parameters.

    command (
        parameter
        with line breaks
    )

## Script ##

A script is a collection of commands which can be invoked by some name. All semantics about how commands operate are also applicable to scripts. A script can accept parameters and also return values.

Scripts are supplied by the host applicable. They may originate from a text file, or other storage the host application offers.

todo: how to define help in the script file.

## Subscript ##

A subscript is a script defined inside another script. It's like a function in other languages.

To define a subscript, use the `sub` command:

    sub subname body

It's highly likely the body of the subscript will include spaces and span multiple lines, so surround the body with brackets.

    sub get-id (

    )

By default the subscript is only accessible within the script that defined it. To make the subscript available outside the defining script, use the `-g` parameter with the `sub` command to define it as a global subscript.

    sub -g reset (

    )

Trying to create multiple subscritps with the same name will result in an error. To redefine a subscript, first delete the subscript as follows:

    sub -d reset

To check if a subscript exists, use the `isdefined` command.

### Return Values ###

To return a value from a script or subscript, use the `return` command.

    sub get-name (
        set num << math:random
        return (name-$num$)
    )

### Parameters ###

Use the `param` command to read parameters that were passed to the script or subscript.

    param name -numbered 1 -optional
    param name -named inputname

Parameters are required by default. To make a parameter optional, use the `-optional` flag parameter on the `param` command.

Parameters are immutable. Variables cannot be created with the same name as a parameter.
