using System;
using TheCloset.ConsoleHelpers;
using TheCloset.TextAdventure;

namespace TheCloset.GenericProps {

	public class Door : Prop {
		#region Fields

		public readonly Location To;
		private readonly FormattedString _useMsg, _lockMsg;

		#endregion Fields

		#region Properties

		public bool Locked { get; set; } = false;

		#endregion Properties

		#region Events

		public event Action<Door> DoorUsed;

		#endregion Events

		#region Constructors

		public Door(Location from, Location to, string name, int x, int y, FormattedString useMsg = null,
					string lockMsg = "The door is locked.") : base(from, name, x, y) {
			_useMsg = useMsg ?? "You use the door.";
			_lockMsg = lockMsg;
			To = to;
			InternalVerbs.Add(new Verb("Use the " + Name, UseAction));
		}

		public Door(Location from, Location to, FormattedString name, int x, int y, FormattedString useMsg = null,
					string lockMsg = "The door is locked.") : base(from, name, x, y) {
			_useMsg = useMsg ?? "You use the door.";
			_lockMsg = lockMsg;
			To = to;
			InternalVerbs.Add(new Verb("Use the " + Name, UseAction));
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