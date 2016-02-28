﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.Locations.InTheCloset;
using TheCloset.TextAdventure;

namespace TheCloset.Locations {
	internal class TheVents : Location {
		public TheVents(OnTheBox closet) {
			Props.Add(new Vent(this, closet, "first vent", false, 3, 2, "You are in the Utility Closet."));
			Props.Add(new Vent(this, new Hallway(), "second vent", false, 3, 5, "You are in a Hallway."));
			InternalVerbs.Add(new Verb("Crawl to the", GetDistantPropNames, WalkToThe));
		}

		private IEnumerable<CommandPart> GetDistantPropNames()
			=> Props.Except(AdjacentProps).Select(o => new CommandPart(o.Name));

		private void WalkToThe(string s) {
			var p = Props.First(o => s.EndsWith(o.Name, StringComparison.InvariantCultureIgnoreCase));
			Player.Instance.SetPosition(p.X, p.Y);
			Game.Instace.OutputPane.Write($"You crawl to the {p.Name}");
		}

		public class Vent : Prop {
			private readonly string _post;
			private readonly Location _to;
			private readonly bool _up;

			public Vent(Location from, Location to, string name, bool up, int x, int y, string post = "")
				: base(from, name, x, y) {
				_up = up;
				_to = to;
				_post = post;
				InternalVerbs.Add(new Verb(_up ? $"Enter the {Name}." : $"Exit the {Name}.", EnterVentVerb));
			}

			private void EnterVentVerb(string s) {
				Console.Write(_up
					? $"It takes some effort, but you hoist yourself up into the {Name}."
					: $"You lower yourself from the {Name}. {_post}");
				Player.Instance.ChangeLocation(_to);
			}
		}
	}
}