using System.Collections.Generic;
using System.Linq;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Abstractions.UnitTests.SampleCommands
{
    [Command("map-params")]
    public class MapParameterCommand : ICommand
    {
        [NamedParameter("dictionary", "dictionary")]
        public Dictionary<string, string>? Dictionary { get; set; }

        [NamedParameter("abstract-dictionary", "abstract dictionary")]
        public IDictionary<string, string>? AbstractDictionary { get; set; }

		[NamedParameter("invalidkeytype", "invalidkeytype")]
		public Dictionary<int, string>? InvalidKeyTypeParam { get; set; }

		[NamedParameter("intdictionary", "intdictionary")]
        public Dictionary<string, int>? IntDictionary { get; set; }

		[NamedParameter("list-dictionary", "list dictionary")]
        public IDictionary<string, Chel.Abstractions.Types.List>? ListDictionary { get; set; }

		public CommandResult Execute()
		{
			var elements = (
                ((IEnumerable<object>?)Dictionary)!.Select(x => new Literal(x.ToString()!)) ??
                ((IEnumerable<object>?)AbstractDictionary)!.Select(x => new Literal(x.ToString()!))
            );

            return new ValueResult(new Chel.Abstractions.Types.List(elements.ToList()));
		}
	}
}