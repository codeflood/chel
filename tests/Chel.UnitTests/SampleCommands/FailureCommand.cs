using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
	[Command("fail")]
	public class FailureCommand : ICommand
	{
		public CommandResult Execute()
		{
			var location = new SourceLocation(-1, -1);
			return new FailureResult(location, new []{ "nothin'" });
		}
	}
}
