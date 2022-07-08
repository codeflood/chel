using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Abstractions.UnitTests.SampleCommands
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

        [NamedParameter("intlist", "intlist")]
        public IList<int> IntList { get; set; }

        [NamedParameter("collection", "collection")]
        public ICollection<string> Collection { get; set; }

        [NamedParameter("rocollection", "rocollection")]
        public IReadOnlyCollection<string> ReadOnlyCollection { get; set; }

        [NamedParameter("concretelist", "concretelist")]
        public List<string> ConcreteList { get; set; }

        [NamedParameter("maplist", "maplist")]
        public List<Map> MapList { get; set; }

		public CommandResult Execute()
		{
			var elements = (
                (IEnumerable<object>)Array ??
                (IEnumerable<object>)Enumerable ??
                (IEnumerable<object>)List ??
                (IEnumerable<object>)IntList ??
                (IEnumerable<object>)Collection ??
                (IEnumerable<object>)ReadOnlyCollection ??
                (IEnumerable<object>)ConcreteList
            );

            return new ValueResult(new Chel.Abstractions.Types.List(elements.Select(x => new Literal(x.ToString())).ToList()));
		}
	}
}