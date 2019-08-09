# Variables #

Variables allow storing some value which can be substituted into subsequent calls.

## Scope ##

By default variables are only accessible within the script or subscript they're defined in. This forces callers to be explicit about what they send to commands, scripts and subscripts.

A variable can be make accessible globally by using the `-g` parameter when creating the variable.

## Types ##

Chel includes a very simple type system.

### Single Value ###

This is the simplest kind of variable. It stores a single value.

    value name value
    value itemName (a value with spaces)

### List ###

A list allows defining multiple ordered values.

    list name value1 value2

The list can be manipulated using the `list` command. For example, to add an element to the end of an existing list use the `-a` parameter.

    list mylist 1 2 3
    list mylist -a 4

### Map ###

A map is like a list, but all values have a name.

    map name (
        key1 = value1
        key2 = value2
        (key with space)=(value with space)
    )

A map value can be any variable type, including a list or another map.

    map name (
        key = ??
        # how to specify a list or map?
    )

## Substitution ##

Variables can be substituted in any call and are denoted by wrapping the variable name in curly braces (`{}`).

    command {variable}

Even the command name can be substituted from a variable.

    {commandName} param

A command substituted from a variable cannot include parameters in the variable value. Map splatting and subscripts make it redundant to add additional parameters to a variable substituted as a command name.

    # not allowed
    value commandName (command -name value)
    {commandName} # error

To use a single value of a list instead of the whole list, append a colon (`:`) and the 1 based index of the value you want to use.

    command {list:1}

To use just a single value of a map during substitution, append a colon (`:`) and the key.

    command {map:key}

The substitution is a value substitution, not an object substitution. The variable is not "passed" to commands or scripts. The value the variable holds is substituted into the call.

Variables inside other parameter values are substituted verbatim.

    command (hello {name})

## Well-known Global Variables ##

The command prompt is read from the `prompt` global variable.
