using System;
using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
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
            catch(ParseException ex)
            {
                var commandResult = new FailureResult(ex.SourceLocation.LineNumber, new[] { ex.Message });
                resultHandler(commandResult);
                return;
            }

            foreach(var commandInput in commandInputs)
            {
                var commandResult = Execute(commandInput);
                resultHandler(commandResult);
            }
        }

        private CommandResult Execute(CommandInput commandInput)
        {
            var result = ExecuteSubcommands(commandInput);
            if(!result.Success)
                return result;

            CommandResult commandResult = null;

            try
            {
                var command = _commandFactory.Create(commandInput);
                if(command != null)
                {    
                    var bindingResult = _parameterBinder.Bind(command, commandInput);

                    if(bindingResult.Success)
                    {
                        commandResult = command.Execute();

                        if(commandResult is FailureResult failureResult && failureResult.SourceLine == Constants.CurrentSourceLine)
                            commandResult = new FailureResult(commandInput.SourceLine, failureResult.Messages.ToArray());
                    }
                    else
                        commandResult = new FailureResult(commandInput.SourceLine, bindingResult.Errors.ToArray());
                }
                else
                    commandResult = new FailureResult(commandInput.SourceLine, new[] { string.Format(Texts.UnknownCommand, commandInput.CommandName) });
            }
            catch(Exception ex)
            {
                commandResult = new FailureResult(commandInput.SourceLine, new[] { ex.Message });
            }

            return commandResult;
        }

        private CommandResult ExecuteSubcommands(CommandInput commandInput)
        {
            // Execute subcommands, including those in lists and maps
            for(var i = 0; i < commandInput.Parameters.Count; i++)
            {
                var parameter = commandInput.Parameters[i];

                if(parameter is CommandInput subcommand)
                {
                    var result = Execute(subcommand);
                    if(!result.Success)
                        return result;

                    if(result is ValueResult valueResult)
                        subcommand.SubstituteValue = valueResult.Value;
                    else
                        new FailureResult(commandInput.SourceLine, new[] { Texts.SubcommandResultMustBeChelType });
                } else if(parameter is List listParameter)
                {
                    foreach(var value in listParameter.Values)
                    {
                        if(value is CommandInput listSubcommand)
                        {
                            var result = Execute(listSubcommand);
                            if(!result.Success)
                                return result;

                            if(result is ValueResult valueResult)
                                listSubcommand.SubstituteValue = valueResult.Value;
                            else
                                new FailureResult(commandInput.SourceLine, new[] { Texts.SubcommandResultMustBeChelType });
                        }
                    }
                }
            }

            return new SuccessResult();
        }
    }
}