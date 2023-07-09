using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
	[Command("fail")]
	public class FailureCommand : ICommand
	{
		public CommandResult Execute()
		{
			return new FailureResult(SourceLocation.CurrentLocation, "nothin'");
		}
	}
}
