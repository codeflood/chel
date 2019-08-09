# chel

A simple scripting environment for .net applications.

> ** Work in Progress:** This project is currently a work in progress and is not ready to be consumed.

Chel is modelled after a command prompt, to provide a simple and intuative scripting environment.

Chel in a nutshell:

	# This is a comment
	# Execute a simple command without parameters
	command

	# A command with a numbered parameter
	command value

	# A command with a named parameter
	command -parameter value

	# A command with a flag parameter
	command -flag

	# Set a single value variable and use it in a command invoationb
	value myvar (lorem ipsum)
	command -param {myvar}

	# Pass the output of one command to be used by another
	command >> othercommand {~}

	# Use the output of one command as a parameter to another
	command << subcommand
