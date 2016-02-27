﻿using System.Collections.Generic;

namespace TheCloset.TextAdventure {
	public abstract class Prop : IVerbable {
		protected Prop(Location parent, string name, int x, int y) {
			Name = name;
			X = x;
			Y = y;
			Parent = parent;
		}

		public Location Parent { get; }

		public IEnumerable<Verb> Verbs => InternalVerbs;

		protected List<Verb> InternalVerbs { get; } = new List<Verb>();

		public int X { get; set; }
		public int Y { get; set; }
		
		public string Name;

		public override string ToString() {
			return Name.Replace('_', ' ');
		}
	}
}
