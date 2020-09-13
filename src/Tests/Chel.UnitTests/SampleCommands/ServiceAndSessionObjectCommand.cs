using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.UnitTests.SampleObjects;
using Chel.UnitTests.Services;

namespace Chel.UnitTests.SampleCommands
{
    [Command("alpha")]
    public class ServiceAndSessionObjectCommand : ICommand
    {
        public ServiceAndSessionObjectCommand(ISampleService service, GoodObject sessionObject)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));

            if(sessionObject == null)
                throw new ArgumentNullException(nameof(sessionObject));
        }

        public CommandResult Execute()
        {
            return new SuccessResult();
        }
    }
}