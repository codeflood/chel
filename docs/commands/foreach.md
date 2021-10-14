# foreach #

?? should this command be called `foreach`? Or is there a better descriptive term for it?

This command iterates a list or map and allows a block of script to be executed for each. The name of the variable to set on each element of the input is passed as the first numbered parameter.

    foreach n $listvar$ (
        command $n$
    )

It would have been nice to use a constant variable name like tilda (`~`), but that token is already used to refer to the result of the previous command, and is used in command chaining. Allowing the user to specify the variable name makes it more apparent where the variable came from and makes for one less thing users need to keep in their head.

In the case of a map, the element in the variable contains a `key` property which is the key of the element, and a `value` property which is the value.

    var mymap {
        a:b
        c:d
    }

    foreach n $mymap$ (
        command $n:key$ $n:value$
    )
