using System;
using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Variables;
using Chel.Exceptions;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="ISession" />.
    /// </summary>
    public class Session : ISession
    {
        private IParser _parser = null;
        private ICommandFactory _commandFactory = null;
        private ICommandParameterBinder _parameterBinder = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="parser">The <see cref="IParser" /> used to parse input.</param>
        /// <param name="commandFactory">The <see cref="ICommandFactory" /> used to instantiate commands.</param>
        /// <param name="parameterBinder">The binder used to bind parameters to command properties.</summary>
        public Session(IParser parser, ICommandFactory commandFactory, ICommandParameterBinder parameterBinder)
        {   
            if(parser == null)
                throw new ArgumentNullException(nameof(parser));

            if(commandFactory == null)
                throw new ArgumentNullException(nameof(commandFactory));

            if(parameterBinder == null)
                throw new ArgumentNullException(nameof(parameterBinder));

            _commandFactory = commandFactory;
            _parser = parser;
            _parameterBinder = parameterBinder;
        }

        /// <summary>
        /// Executes input.
        /// </summary>
        /// <param name="input">The input to execute.</param>
        /// <param name="resultHandler">The handler to execute for the results of the input.</param>
        public void Execute(string input, Action<CommandResult> resultHandler)
        {
            if(resultHandler == null)
                throw new ArgumentNullException(nameof(resultHandler));

            IList<CommandInput> commandInputs = null;

            try
            {
                commandInputs = _parser.Parse(input);
            }
            catch(ParserException ex)
            {
                var commandResult = new FailureResult(ex.SourceLocation.LineNumber, new[] { ex.Message });
                resultHandler(commandResult);
                return;
            }

            foreach(var commandInput in commandInputs)
            {
                CommandResult commandResult = null;

                try
                {
                    var command = _commandFactory.Create(commandInput);
                    if(command != null)
                    {
                        var bindingResult = _parameterBinder.Bind(command, commandInput);

                        if(bindingResult.Success)
                            commandResult = command.Execute();
                        else
                            commandResult = new FailureResult(commandInput.SourceLine, bindingResult.Errors.ToArray());
                    }
                    else
                        commandResult = new FailureResult(commandInput.SourceLine, new[] { Texts.UnknownCommand });
                }
                catch(Exception ex)
                {
                    commandResult = new FailureResult(commandInput.SourceLine, new[] { ex.Message });
                }

                resultHandler(commandResult);
            }
            
        }
    }
}