using System;
using TheCloset.ConsoleHelpers;
using TheCloset.TextAdventure;

namespace TheCloset.GenericProps {

	internal class Door : Prop {

		#region Fields

		private readonly Location _to;
		private readonly FormattedString _useMsg, _lockMsg;

		#endregion Fields

		public event Action<Door> DoorUsed;

		#region Properties

		public bool Locked { get; set; } = false;

		#endregion Properties


		#region Constructors

		public Door(Location from, Location to, string name, int x, int y, FormattedString useMsg = null,
					string lockMsg = "The door is locked.") : base(from, name, x, y) {
			_useMsg = useMsg ?? "You use the door.";
			_lockMsg = lockMsg;
			_to = to;
			InternalVerbs.Add(new Verb($"Use the {Name}", UseAction));
		}

		public Door(Location from, Location to, FormattedString name, int x, int y, FormattedString useMsg = null,
					string lockMsg = "The door is locked.") : base(from, name, x, y) {
			_useMsg = useMsg ?? "You use the door.";
			_lockMsg = lockMsg;
			_to = to;
			InternalVerbs.Add(new Verb($"Use the {Name}", UseAction));
		}

		#endregion Constructors


		#region Methods

		private void UseAction(string s) {
			Game.Instace.OutputPane.Write(Locked ? _lockMsg : _useMsg);
			DoorUsed?.Invoke(this);
		}

		#endregion Methods
	}
}