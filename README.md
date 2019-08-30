# chel

A simple scripting environment for .net applications.

> ** Work in Progress:** This project is currently a work in progress and is not ready to be consumed.

Chel is modelled after shell scripting languages like the Windows command prompt, or Bash, to provide a simple and intuative scripting environment.

Chel in a nutshell:

	# comment
	set var value
	command numparam -parameter $value$ -flag
	command << subcommand
	command >> chained-command $~$

	sub rigel (
		command -flag
		command -param value
	)

	set list [1 2 3 4]
	set params {
		key : value
	}
	set moreparams {
		foo : $list$
		bar : $params$
	}
	command @moreparams -flag
	rigel
