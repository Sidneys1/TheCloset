using System.Collections.Generic;
using TheCloset.ConsoleHelpers;

namespace TheCloset.TextAdventure {

	public abstract class Item : IVerbable {

		#region Fields

		protected readonly List<Verb> InnerVerbs = new List<Verb>();

		#endregion Fields


		#region Properties

		public FormattedString Name { get; private set; }

		public IEnumerable<Verb> Verbs => InnerVerbs;

		#endregion Properties


		#region Constructors

		protected Item(string name) {
			Name = name.Cyan();
		}

		protected Item(FormattedString name) {
			Name = name;
		}

		#endregion Constructors
	}
}