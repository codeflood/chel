# Variables #

Variables allow storing some value which can be substituted into subsequent calls.

## Scope ##

By default variables are only accessible within the script or subscript they're defined in. This forces callers to be explicit about what they send to commands, scripts and subscripts. To make a variable accessible to a caller, the variable should be returned from the called script or subscript.

A variable can be make accessible globally by using the `-g` parameter when creating the variable.

## Types ##

Chel includes a very simple type system.

### Single Value ###

This is the simplest kind of variable. It stores a single value. If the value includes any spaces, enclose the value with round brackets.

    var name value
    var itemName (a value with spaces)

#### Boolean Values ####

Boolean values are not a specific variable type, but some special values used when a parameter expects a boolean value.

    if true (
        # do something
    )

    if (not false) (
        # do something
    )

### List ###

A list allows defining multiple ordered values. The list is surrounded with square brackets.

    var mylist [value1 value2]

The values of a list are delimited by spaces. If you wish to include a space in a value, surround the value with round brackets.

    var mylist [(value with space) 42 (other value)]

A list value can be of any variable type, including other lists and maps. Lists can also be entered over multiple lines to make them easier to read.

    var mylist [
        [innerlist1, innerlist2]
        (value with space)
        {
            key1 : value1
            key2 : value2
        }
    ]

### Map ###

A map is like a list, but all values have a name. The map is surrounded with curly brackets. Names and values inside the map are delimited by a colon (`:`) with pairs of names and values delimited by spaces.

    var mymap {key1:value1 key2:value2}

Maps can be entered over multiple lines to make them easier to read.

    var mymap {
        key1 : value1
        key2 : (value with space)
    }

A map value can be any variable type, including a list or another map.

    var mymap {
        key : (simple value)
        akey : [a bb cc]
        bkey: {
            key:value
        }
    }

A value name inside a map must follow the standard [naming rules](naming-rules.md) for Chel.

## Substitution ##

Variables can be substituted in any call and are denoted by wrapping the variable name in dollar signs (`$`).

    command $variable$

To use a single value of a list instead of the whole list, append a colon (`:`) and the 1 based index of the value you want to use.

    command $list:1$

Indexes can also be negative which counts from the end of the list. -1 is the last element, -2 the second last, etc.

    var foo (27 12 35 95)
    command $foo:1$ # 27
    command $foo:-1$ # 95

If the index is larger than the number of values in the list, an error is raised. The number of values inside a list can be accessed through the special name `length`:

    var foo (27 12 35 95)
    echo (length is $foo:length$)

To use just a single value of a map during substitution, append a colon (`:`) and the value name.

    command $map:name$

If the name doesn't exist in the map, an error is raised.

The substitution is a value substitution, not an object substitution. The variable is not "passed" to commands or scripts. The value the variable holds is substituted into the call.

Variables inside other parameter values are substituted verbatim.

    command (hello $name$)

Remember, names in Chel are not case-sensitive. I could use any casing to use the variable `name` in the above example:

    command (hello $NAME$)

Likewise when accessing values in a map:

    command (hello $MYMAP:Name$)

## Well-known Global Variables ##

The command prompt is read from the `prompt` global variable.
