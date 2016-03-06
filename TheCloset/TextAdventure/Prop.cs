using System.Collections.Generic;
using TheCloset.ConsoleHelpers;

namespace TheCloset.TextAdventure {

	public abstract class Prop : IVerbable {


		#region Fields

		public FormattedString Name;

		#endregion Fields


		#region Properties

		public Location Parent { get; }

		public IEnumerable<Verb> Verbs => InternalVerbs;

		public int X { get; set; }

		public int Y { get; set; }

		protected List<Verb> InternalVerbs { get; } = new List<Verb>();

		#endregion Properties


		#region Constructors

		protected Prop(Location parent, FormattedString name, int x, int y) {
			Name = name;
			X = x;
			Y = y;
			Parent = parent;
		}

		protected Prop(Location parent, string name, int x, int y) {
			Name = name.Magenta();
			X = x;
			Y = y;
			Parent = parent;
		}

		#endregion Constructors


		#region Methods

		public override string ToString() => Name.ToString();

		#endregion Methods

	}
}