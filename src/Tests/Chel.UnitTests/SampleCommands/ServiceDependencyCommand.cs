using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.UnitTests.Services;

namespace Chel.UnitTests.SampleCommands
{
    [Command("alpha", "A command that requires a dependency.")]
    public class ServiceDependencyCommand : ICommand
    {
        private ISampleService _service = null;

        public ServiceDependencyCommand(ISampleService service)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));

            _service = service;
        }

        public CommandResult Execute()
        {
            _service.DoSomething();
            return new SuccessResult();
        }
    }
}