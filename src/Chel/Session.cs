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
        private readonly IParser _parser;
        private readonly ICommandFactory _commandFactory;
        private readonly ICommandParameterBinder _parameterBinder;
        private readonly IScriptProvider _scriptProvider;
        private readonly Action<CommandResult> _resultHandler;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="parser">The <see cref="IParser" /> used to parse input.</param>
        /// <param name="commandFactory">The <see cref="ICommandFactory" /> used to instantiate commands.</param>
        /// <param name="parameterBinder">The binder used to bind parameters to command properties.</summary>
        /// <param name="resultHandler">The handler to execute for the results of the input.</param>
        public Session(
            IParser parser,
            ICommandFactory commandFactory,
            ICommandParameterBinder parameterBinder,
            IScriptProvider scriptProvider,
            Action<CommandResult> resultHandler)
        {   
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
            _parameterBinder = parameterBinder ?? throw new ArgumentNullException(nameof(parameterBinder));
            _scriptProvider = scriptProvider ?? throw new ArgumentNullException(nameof(scriptProvider));
            _resultHandler = resultHandler ?? throw new ArgumentNullException(nameof(resultHandler));
        }

        /// <summary>
        /// Executes input.
        /// </summary>
        /// <param name="input">The input to execute.</param>
        public void Execute(string? input)
        {
            if(input == null)
                return;
                
            IList<CommandInput> commandInputs;

            try
            {
                commandInputs = _parser.Parse(input);
            }
            catch(ParseException ex)
            {
                var commandResult = new FailureResult(ex.SourceLocation, ex.Message);
                _resultHandler(commandResult);
                return;
            }

            foreach(var commandInput in commandInputs)
            {
                var commandResult = Execute(commandInput);

                if(commandResult is AggregateFailureResult aggregateFailureResult)
                {
                    foreach(var result in aggregateFailureResult.InnerResults)
                    {
                        _resultHandler(result);
                    }
                }
                else if(commandResult != null)
                    _resultHandler(commandResult);
            }
        }

        private CommandResult Execute(CommandInput commandInput)
        {
            var result = ExecuteSubcommands(commandInput);
            if(!result.Success)
                return result;

            CommandResult commandResult = SuccessResult.Instance;

            try
            {
                var command = _commandFactory.Create(commandInput);
                if(command != null)
                {    
                    var bindingResult = _parameterBinder.Bind(command, commandInput);

                    if(bindingResult.Success)
                    {
                        commandResult = command.Execute();

                        if(commandResult is FailureResult failureResult && failureResult.SourceLocation.Equals(SourceLocation.CurrentLocation))
                            commandResult = new FailureResult(commandInput.SourceLocation, failureResult.Message);
                    }
                    else
                    {
                        var innerResults = bindingResult.Errors.Select(x => new FailureResult(x.SourceLocation, x.Message)).ToArray();
                        commandResult = new AggregateFailureResult(innerResults);
                    }
                }
                else
                {
                    var script = _scriptProvider.GetScriptSource(commandInput.CommandIdentifier.Module, commandInput.CommandIdentifier.Name);
                    if(script != null)
                    {
                        Execute(script);
                    }
                    else
                        commandResult = new FailureResult(commandInput.SourceLocation, ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.UnknownCommand, commandInput.CommandIdentifier));
                }
            }
            catch(Exception ex)
            {
                commandResult = new FailureResult(commandInput.SourceLocation, ex.Message);
            }

            return commandResult;
        }

        private CommandResult ExecuteSubcommands(CommandInput commandInput)
        {
            // Execute subcommands, including those in lists and maps
            for(var i = 0; i < commandInput.Parameters.Count; i++)
            {
                var parameter = commandInput.Parameters[i];

                if(parameter is SourceValueCommandParameter sourceValueCommandParameter)
                    parameter = sourceValueCommandParameter.Value;

                if(parameter is CommandInput subcommand)
                {
                    var result = Execute(subcommand);
                    if(result is FailureResult failureResult)
                        return CreateSubcommandFailureResult(subcommand, failureResult);

                    if(result is ValueResult valueResult)
                        subcommand.SubstituteValue = valueResult.Value;
                    else
                        new FailureResult(subcommand.SourceLocation, ApplicationTextResolver.Instance.Resolve(ApplicationTexts.SubcommandResultMustBeChelType));
                }
                else if(parameter is List listParameter)
                {
                    foreach(var value in listParameter.Values)
                    {
                        if(value is CommandInput listSubcommand)
                        {
                            var result = Execute(listSubcommand);
                            if(result is FailureResult failureResult)
                                return CreateSubcommandFailureResult(listSubcommand, failureResult);

                            if(result is ValueResult valueResult)
                                listSubcommand.SubstituteValue = valueResult.Value;
                            else
                                new FailureResult(listSubcommand.SourceLocation, ApplicationTextResolver.Instance.Resolve(ApplicationTexts.SubcommandResultMustBeChelType));
                        }
                    }
                }
            }

            return SuccessResult.Instance;
        }

        private CommandResult CreateSubcommandFailureResult(CommandInput subcommand, FailureResult subcommandResult)
        {
            if(!subcommandResult.SourceLocation.Equals(SourceLocation.CurrentLocation))
                return subcommandResult;

            var result = new FailureResult(subcommand.SourceLocation, subcommandResult.Message);
            return result;
        }
    }
}