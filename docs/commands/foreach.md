# foreach #

This command iterates a list or map and allows a block of script to be executed for each. The element from the input is made available in the tilde (`~`) variable.

    foreach $listvar$ (
        command $~$
    )

In the case of a map, the element in the `~` variable contains a `key` property which is the key of the element, and a `value` property which is the value.

    set mymap {
        a:b
        c:d
    }

    foreach $mymap$ (
        command $~:key$ $~:value$
    )