using System.Collections.Generic;

namespace TheCloset.TextAdventure {
	public abstract class Item :IVerbable {
		public Player Parent { get; }
		public string Name { get; private set; }

		protected Item(Player parent, string name) {
			Parent = parent;
			Name = name;
		}

		protected readonly List<Verb> InnerVerbs = new List<Verb>();
		public IEnumerable<Verb> Verbs => InnerVerbs;
	}
}
