using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel
{
    public class Session : ISession
    {
        private IParser _parser = null;
        private ICommandFactory _commandFactory = null;

        public Session(IParser parser, ICommandFactory commandFactory)
        {   
            if(parser == null)
                throw new ArgumentNullException(nameof(parser));

            if(commandFactory == null)
                throw new ArgumentNullException(nameof(commandFactory));

            _commandFactory = commandFactory;
            _parser = parser;
        }

        public void Execute(string input, Action<CommandResult> resultHandler)
        {
            if(resultHandler == null)
                throw new ArgumentNullException(nameof(resultHandler));

            var commandInputs = _parser.Parse(input);

            foreach(var commandInput in commandInputs)
            {
                CommandResult result = new UnknownCommandResult();

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