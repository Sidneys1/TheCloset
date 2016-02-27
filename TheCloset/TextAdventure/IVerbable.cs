using System.Collections.Generic;

namespace TheCloset.TextAdventure {
	public interface IVerbable {
		IEnumerable<Verb> Verbs { get; }
	}
}