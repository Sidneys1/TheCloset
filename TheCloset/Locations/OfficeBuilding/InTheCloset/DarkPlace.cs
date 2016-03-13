using TheCloset.ConsoleHelpers;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding.InTheCloset {

	internal class DarkPlace : Location {

		#region Fields

		private readonly Verb _blind;
		private readonly Verb _escape;
		private readonly Verb _wiggle;
		private bool _bound = true;

		#endregion Fields


		#region Constructors

		public DarkPlace() {
			_blind = new Verb(new FormattedString("Remove the ", "blindfold".Cyan()), BlindMethod);
			_escape = new Verb("Exert yourself!", EscapeMethod);
			_wiggle = new Verb("Wiggle my arms", WiggleMethod);

			InternalVerbs.Add(new Verb("Look around", LookMethod));
			InternalVerbs.Add(_wiggle);
		}

		#endregion Constructors


		#region Methods

		private void BlindMethod(string s) {
			Game.Instace.OutputPane.Write(new FormattedString("You reach up and remove your ", "blindfold".Cyan(), ". You are in a ", "Closet".Yellow(), "."));
			InternalVerbs.Remove(_blind);
			Player.Instance.ChangeLocation(new TheCloset());
		}

		private void EscapeMethod(string s) {
			Game.Instace.OutputPane.Write(new FormattedString("You wrench the ", "ropes".Cyan(), " with all your strength. They give way."));
			InternalVerbs.Remove(_escape);
			InternalVerbs.Add(_blind);
			_bound = false;
		}

		private void LookMethod(string s) {
			Game.Instace.OutputPane.Write(new FormattedString("You can't see anything. You have a ", "blindfold".Cyan(), " on."));
			if (_bound)
				Game.Instace.OutputPane.Write(" Your wrists are bound behind you back.");
		}
		private void WiggleMethod(string s) {
			Game.Instace.OutputPane.Write("You struggle against your bindings. They loosen a little.");
			InternalVerbs.Remove(_wiggle);
			InternalVerbs.Add(_escape);
		}

		#endregion Methods
	}
}