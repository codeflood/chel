# If #

Evalutes an expression and if it evaluates to true, executes the code block.

    if ($var$ = foo) (
        # script block
    )

The first numbered parameter is the expression to evaluate.

An optional `-else` parameter can be supplied with a script block which will be executed if the first script block is not.

    if ($var$ = foo) (
        # script block
    ) -else (
        # script block
    )
