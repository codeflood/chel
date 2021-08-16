using System.Collections.Generic;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("list-params")]
	public class ListParameterCommand : ICommand
	{
        [NamedParameter("array", "list")]
        public string[] Array { get; set; }

        [NamedParameter("enumerable", "list")]
        public IEnumerable<string> Enumerable { get; set; }

        [NamedParameter("list", "list")]
        public IList<string> List { get; set; }

        [NamedParameter("collection", "collection")]
        public ICollection<string> Collection { get; set; }

        [NamedParameter("rocollection", "rocollection")]
        public IReadOnlyCollection<string> ReadOnlyCollection { get; set; }

        [NamedParameter("concretelist", "concretelist")]
        public List<string> ConcreteList { get; set; }

        [NamedParameter("dictionary", "dictionary")]
        public Dictionary<string, string> Dictionary { get; set; }

		public CommandResult Execute()
		{
			throw new System.NotImplementedException();
		}
	}
}