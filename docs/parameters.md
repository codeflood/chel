# Parameters #

Parameters can be passed to commands, scripts and subscripts.

All parameters are passed as values.

Any unknown parameters cause an error.

## Numbered Parameter ##

Numbered parameters are passed to the command with no name. The first numbered parameter is parameter 1, the second is 2, etc.

    command param1 param2

## Named Parameter ##

Named parameters are passed by prefixing the parameter name with a dash (`-`), followed by the value of the parameter.

    command -parama value -paramb value

A named parameter cannot be named only with numbers.

    # Illegal parameter name. Not allowed.
    command -2 value

## Flag Parameter ##

A flag parameter is like a named parameter, but no value is passed. It is a boolean parameter with it's presence meaning true and it's absence meaning false.

    command -flag1 -flag2

## Map Splatting ##

Multiple parameters can be added to a map and passed all at once.

    map p (
        param1 : value
        param2 : value2
        1 : value3
        flagparam : true
    )

    command @p

The keys will be mapped to the named parameters of the target. Keys that are only numbers will be passed as numbered parameters. Numbered parameters must start at 1 and be seqential.

Additional parameters can be passed in addition to the map.

    command @p -named value
