using System.Collections.Generic;

namespace TheCloset.TextAdventure {

	public interface IVerbable {
		#region Properties

		IEnumerable<Verb> Verbs { get; }

		#endregion Properties
	}
}