using System;
using System.Collections.Generic;
using System.Linq;

namespace TheCloset.TextAdventure {
	public abstract class Location : IVerbable {
		public Player Player { get; }
		protected Location(Player player) {
			Player = player;
		}
		
		protected List<Verb> InternalVerbs { get; } = new List<Verb>();
		public IEnumerable<Verb> Verbs {
			get {
				foreach (var internalVerb in InternalVerbs)
					yield return internalVerb;
				foreach (var verb in AdjacentProps.SelectMany(o => o.Verbs))
					yield return verb;
			}
		}

		public List<Prop> Props { get; } = new List<Prop>();

		public IEnumerable<Prop> AdjacentProps => Props.Where(o => Player.X == o.X && Player.Y == o.Y);
	}
}
