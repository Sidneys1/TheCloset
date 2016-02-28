using TheCloset.TextAdventure;

namespace TheCloset.GenericProps {
	internal class Door : Prop {
		private readonly Location _to;
		private readonly string _useMsg, _lockMsg;

		public Door(Location from, Location to, string name, int x, int y, string useMsg = "You use the door.",
			string lockMsg = "The door is locked.") : base(from, name, x, y) {
			_useMsg = useMsg;
			_lockMsg = lockMsg;
			_to = to;
			InternalVerbs.Add(new Verb($"Use the {Name}", UseAction));
		}

		public bool Locked { get; set; } = false;

		private void UseAction(string s) {
			Game.Instace.OutputPane.Write(Locked ? _lockMsg : _useMsg);
		}
	}
}