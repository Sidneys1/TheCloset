using System;
using System.Linq;
using TheCloset.ConsoleHelpers;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.InTheCloset {
	internal class OnTheBox : Location {
		private readonly TheCloset _closet;
		private readonly Verb _unscrewVerb;

		public OnTheBox(TheCloset closet) {
			_closet = closet;
			_unscrewVerb = new Verb("Unscrew the vent", UnscrewVerb);
			InternalVerbs.Add(new Verb("Climb down", ClimbDownVerb));
			InternalVerbs.Add(_unscrewVerb);
		}

		private void ClimbDownVerb(string s) {
			Player.Instance.ChangeLocation(_closet);
		}

		private void UnscrewVerb(string s) {
			if (Player.Instance.Items.All(o => o.Name != "Screwdriver")) {
				Game.Instace.OutputPane.Write(new FormattedString("You do not have a ",
					new FormattedText("Screwdriver", ConsoleColor.Cyan), "."));
				return;
			}
			var pre = Console.ForegroundColor;
			Game.Instace.OutputPane.Write(new FormattedString("You use the ", new FormattedText("Screwdriver", ConsoleColor.Cyan),
				" to open the vent."));
			InternalVerbs.Remove(_unscrewVerb);
			Props.Add(new TheVents.Vent(this, new TheVents(this), "vent", true, 3, 2));
		}
	}
}