# Parser Internals #

The parser is used to parse over the input and extract the tokens which construct the command which will be executed. It does not parse a full abstract syntax tree as the purpose of any given token can depend on the parameter and command to which it is bound.

    command (multi word)

For example, is `multi word` a string literal or a code block? That depends on what `command` does with the first numbered parameter.

The parser uses the chain-of-command pattern over a set of parser states. When a character is consumed from the input it is offered to each of the states in turn. The first to accept the character will take over processing. When the state finishes it steps down and the chain-of-command will run again. If no state is found, a parsing error is thrown.

