# If #

Evalutes an expression and if it evaluates to true, executes the code block.

    if true (
        # script block
    )

The first numbered parameter is the expression to evaluate. An expression must output one of `true` or `false`.

An optional `-else` parameter can be supplied with a script block which will be executed if the first script block is not.

    if false (
        # script block
    ) -else (
        # script block
    )

In keeping with the principals of Chel, the expression is a boolean parameter. Subcommands can be used to perform comparisons.

    if << (eq $var$ foo) (
        # script block
    )

    if << (not << (eq $var$ foo)) (
        # script block
    )

    if << (gt 4 $val$) (
        # script block
    )
