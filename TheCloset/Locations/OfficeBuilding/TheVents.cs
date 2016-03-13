using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.ConsoleHelpers;
using TheCloset.Locations.OfficeBuilding.InTheCloset;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding {

	internal class TheVents : Location {


		#region Constructors

		public TheVents(OnTheBox closet) {
			Props.Add(new Vent(this, closet, "first vent", false, 3, 2,
				new FormattedString("You are in a ", "Utility Closet".Yellow(), ".")));
			Props.Add(new Vent(this, new Hallway(), "second vent", false, 3, 5,
				new FormattedString("You are in a ", "Hallway".Yellow(), ".")));
			InternalVerbs.Add(new Verb("Crawl to the", GetDistantPropNames, WalkToThe));
		}

		#endregion Constructors


		#region Methods

		private IEnumerable<CommandPart> GetDistantPropNames()
			=> Props.Except(AdjacentProps).Select(o => new CommandPart(o.Name));

		private void WalkToThe(string s) {
			var p = Props.First(o => s.EndsWith(o.Name.ToString(), StringComparison.InvariantCultureIgnoreCase));
			Player.Instance.SetPosition(p.X, p.Y);
			Game.Instace.OutputPane.Write($"You crawl to the {p.Name}");
		}

		#endregion Methods


		#region Classes

		public class Vent : Prop {

			#region Fields

			private readonly FormattedString _post;
			private readonly Location _to;
			private readonly bool _up;

			#endregion Fields


			#region Constructors

			public Vent(Location from, Location to, string name, bool up, int x, int y, FormattedString post = null)
				: base(from, name, x, y) {
				_up = up;
				_to = to;
				_post = post;
				InternalVerbs.Add(new Verb(_up ? $"Enter the {Name}" : $"Exit the {Name}", EnterVentVerb));
			}

			#endregion Constructors


			#region Methods

			private void EnterVentVerb(string s) {
				Game.Instace.OutputPane.Write(_up
					? $"It takes some effort, but you hoist yourself up into the {Name}."
					: $"You lower yourself from the {Name}. ");
				if (_post != null)
					Game.Instace.OutputPane.Write(_post);
				Player.Instance.ChangeLocation(_to);
			}

			#endregion Methods
		}

		#endregion Classes
	}
}