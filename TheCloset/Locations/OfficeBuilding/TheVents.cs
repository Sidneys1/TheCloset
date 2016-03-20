using TheCloset.ConsoleHelpers;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding {

	internal class TheVents : Location {
		#region Constructors

		public TheVents(TheCloset.OnTheBox closet) : base("the Vents") {
			Props.Add(new Vent(this, closet, "first vent", false, 3, 2,
				new FormattedString("You are in a ", "Utility Closet".Yellow(), ".")));
			Props.Add(new Vent(this, Hallway.Instance, "second vent", false, 3, 5,
				new FormattedString("You are in a ", "Hallway".Yellow(), ".")));
		}

		#endregion Constructors

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