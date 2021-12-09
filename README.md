# chel

A simple scripting environment for .net applications.

> :warning: **Work in Progress:** This project is currently a work in progress and is not ready to be consumed. Any aspect of the code or language specification may change during initial development.

Chel is modelled after shell scripting languages like the Windows command prompt, or Bash, to provide a simple and intuative scripting environment.

Chel in a nutshell:

	# comment
	var varname value
	command numparam -parameter $varname$ -flag
	command << subcommand
	command >> chained-command $~$

	(#
		block comment
		block comment
	#)
	sub rigel (
		command -flag
		command -param value
	)

	var list [1 2 3 4]
	var params {
		key1 : value1
		key2 : value2
	}
	var moreParams {
		foo : $list$
		bar : $params$
	}
	command $*moreparams$ -flag -param $list:2$
	rigel
