using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.UnitTests.SampleObjects;

namespace Chel.UnitTests.SampleCommands
{
    [Command("alpha")]
    public class SessionObjectCommand : ICommand
    {
        private GoodObject _sessionObject = null;

        public SessionObjectCommand(GoodObject sessionObject)
        {
            if(sessionObject == null)
                throw new ArgumentNullException(nameof(sessionObject));

            _sessionObject = sessionObject;
        }

        public CommandResult Execute()
        {
            return SuccessResult.Instance;
        }
    }
}