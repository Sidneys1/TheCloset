using TheCloset.TextAdventure;

namespace TheCloset.Locations {
	internal class DarkPlace : Location {
		private readonly Verb _blind;
		private readonly Verb _escape;
		private readonly Verb _wiggle;
		private bool _bound = true;

		public DarkPlace() {
			_blind = new Verb("Remove the blindfold", BlindMethod);
			_escape = new Verb("Exert yourself!", EscapeMethod);
			_wiggle = new Verb("Wiggle my arms", WiggleMethod);

			InternalVerbs.Add(new Verb("Look around", LookMethod));
			InternalVerbs.Add(_wiggle);
		}

		private void LookMethod(string s) {
			Game.Instace.OutputPane.Write("You can't see anything. You have a blindfold on.");
			if (_bound)
				Game.Instace.OutputPane.Write(" Your wrists are bound behind you back.");
		}

		private void BlindMethod(string s) {
			Game.Instace.OutputPane.Write("You reach up and remove your blindfold. You are in a Closet.");
			InternalVerbs.Remove(_blind);
			Player.Instance.ChangeLocation(new TheCloset());
		}

		private void EscapeMethod(string s) {
			Game.Instace.OutputPane.Write("You wrench the rope with all your strength. They give way.");
			InternalVerbs.Remove(_escape);
			InternalVerbs.Add(_blind);
			_bound = false;
		}

		private void WiggleMethod(string s) {
			Game.Instace.OutputPane.Write("You struggle against your bindings. They loosen a little.");
			InternalVerbs.Remove(_wiggle);
			InternalVerbs.Add(_escape);
		}
	}
}