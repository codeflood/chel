# filter #

Filter the input for matches.

The command accepts a list and matches lines based on the flags set. The default matching strategy is "contains".

    filter $list$ find

The `exact` flag can be used to match the entire element.

    var list [b bb bbb]
    filter $list$ bb -exact
