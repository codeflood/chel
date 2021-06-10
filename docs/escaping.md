# Escaping #

To pass any of the built-in symbols as a literal, surround the symbol in parentheses (). For example, to pass a dash as a parameter rather than have it interpretted as a named parameter indicator:

    command -param (-)

Same applies to the comment symbol (`#`). It can be passed as a parameter if it's inside of a parenthesised block:

    command (parameter which passes # to the command)

This works because if the `#` was interpretted as a comment, it would cause the trailing parentheses to be ignored which would make the above command syntactically incorrect.

    command (<< parameter)
    command (>> parameter)
    command (input with (parentheses))

## Escaping Variables ##

Variables can appear anywhere inside a parameter, even if that parameter is surrounded with parentheses. Therefore, variable symbols can be escaped by using 2 variable symbols. `^^` is an escaped `^` symbol.

    command (input using ^variable^ inside it)
    command (input using ^^escaped^^ variable, so it's treated as a literal)
