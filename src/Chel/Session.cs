using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel
{
    /// <summary>
    /// The default implementation of the <see cref="ISession" />.
    /// </summary>
    public class Session : ISession
    {
        private IParser _parser = null;
        private ICommandFactory _commandFactory = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="parser">The <see cref="IParser" /> used to parse input.</param>
        /// <param name="commandFactory">The <see cref="ICommandFactory" /> used to instantiate commands.</param>
        public Session(IParser parser, ICommandFactory commandFactory)
        {   
            if(parser == null)
                throw new ArgumentNullException(nameof(parser));

            if(commandFactory == null)
                throw new ArgumentNullException(nameof(commandFactory));

            _commandFactory = commandFactory;
            _parser = parser;
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

            var commandInputs = _parser.Parse(input);

            foreach(var commandInput in commandInputs)
            {
                CommandResult result = new UnknownCommandResult(1);

                var command = _commandFactory.Create(commandInput);
                if(command != null)
                {
                    result = command.Execute();
                }

                resultHandler(result);
            }
        }
    }
}