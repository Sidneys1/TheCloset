using System.Collections.Generic;

namespace TheCloset.TextAdventure {
	public abstract class Item : IVerbable {
		protected readonly List<Verb> InnerVerbs = new List<Verb>();

		protected Item(string name) {
			Name = name;
		}

		public string Name { get; private set; }
		public IEnumerable<Verb> Verbs => InnerVerbs;
	}
}