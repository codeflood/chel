# Expressions #

An expression is a compact format of evaluation used in conditionals such as the `if` command.

A basic expression consists of 2 values and a comparison operator.

    value operator value

Not all operators can work with all kinds of input. For example, less than `<` and greater than `>` cannot work on values which are not numbers.

_what about only using commands?_

    5 < 3
    lessthan 5 3

hmm...it looks a bit weird. Like Polish notation.

## Condition commands ##

How should expressions be handled?

One solution would be to create commands for each kind of expression. `eq`, `neq`, `gt`, `lt`, etc. This creates a lot of small commands which will pollute the `help` output.

Another solution is to have a single `cond` command to evaluate conditions.

    # Option 2: Use a single commmand for conditions
    cond -eq $var$ 4
    cond -eq -not $var$ 4
    cond -gt 10 $var$
    cond -lt 10 $var$

    if << (cond -eq $var$ foo) (
        echo (hello $var$)
    )

    # Option 1: Use separate commands per operation
    eq $var$ 4
    not (eq $var$ 4)
    gt 10 $var$
    lt 10 $var$

    if << (eq $var$ foo) (
        echo (hello $var$)
    )

Overall it looks like using separate commands is cleaner and less key strokes. Plus, if users add their own conditions it helps keep things consistent, because new conditions would be implemented as new commands, not extra parameters to the `cond` command.

## Combining conditions ##

Commands to perform boolean logic will also be required. `and`, `or`, etc.

    and (cond -eq $var$ 4) (cond -lt $var$ 10)
    or (cond -lt 100 $var$) (cond -gt 300 $var$)

    and (eq $var$ 4) (lt $var$ 10)
    or (lt 100 $var$) (gt 300 $var$)
