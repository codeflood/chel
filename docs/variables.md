# Variables #

Variables allow storing some value which can be substituted into subsequent calls.

## Scope ##

By default variables are only accessible within the script or subscript they're defined in. This forces callers to be explicit about what they send to commands, scripts and subscripts. To make a variable accessible to a caller, the variable should be returned from the called script or subscript.

A variable can be make accessible globally by using the `-g` parameter when creating the variable.

## Types ##

Chel includes a very simple type system.

### Single Value ###

This is the simplest kind of variable. It stores a single value. If the value includes any spaces, surround the value with parentheses.

    set name value
    set itemName (a value with spaces)

#### Boolean Values ####

Boolean values are not a specific variable type, but some special values used when a parameter expects a boolean value.

if true (

)

if (not false) (
    
)

### List ###

A list allows defining multiple ordered values. The list is surrounded with square brackets.

    set name [value1 value2]

### Map ###

A map is like a list, but all values have a name. The map is surrounded with curly brackets. Names and values inside the map are delimited by a colon (`:`).

    map name {
        key1 : value1
        key2 : value2
        (key with space):(value with space)
    }

A map value can be any variable type, including a list or another map.

    map name {
        key : (simple value)
        akey : [a bb cc]
        bkey: {
            key:value
        }
    }

## Substitution ##

Variables can be substituted in any call and are denoted by wrapping the variable name in dollar signs (`$`).

    command $variable$

Even the command name can be substituted from a variable.

    $commandName$ param

A command substituted from a variable cannot include parameters in the variable value. Map splatting and subscripts make it redundant to add additional parameters to a variable substituted as a command name.

    # not allowed
    set commandName (command -name value)
    $commandName$ # error

To use a single value of a list instead of the whole list, append a colon (`:`) and the 1 based index of the value you want to use.

    command $list:1$

Indexes can also be negative which counts from the end of the list. -1 is the last element, -2 the second last, etc.

    set foo (27 12 35 95)
    $foo:1$ # 27
    $foo:-1$ 95

To use just a single value of a map during substitution, append a colon (`:`) and the key.

    command $map:key$

The substitution is a value substitution, not an object substitution. The variable is not "passed" to commands or scripts. The value the variable holds is substituted into the call.

Variables inside other parameter values are substituted verbatim.

    command (hello $name$)

## Well-known Global Variables ##

The command prompt is read from the `prompt` global variable.
