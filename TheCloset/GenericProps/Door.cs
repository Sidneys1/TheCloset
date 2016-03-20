using System;
using System.Collections.Generic;
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
			To = to;
			_useMsg = useMsg ?? "You use the door to " + To.Name + ".".Reset();
			_lockMsg = lockMsg;
			InternalVerbs.Add(new Verb("Use", GetName, UseAction));

			DoorUsed += DefaultDoorAction;
		}

		private IEnumerable<CommandPart> GetName() {
			yield return new CommandPart(Name);
		}

		public void DefaultDoorAction(Door obj) {
			if (!Locked && To != null) Player.Instance.ChangeLocation(To);
		}

		public Door(Location from, Location to, FormattedString name, int x, int y, FormattedString useMsg = null,
					string lockMsg = "The door is locked.") : base(from, name, x, y) {
			To = to;
			_useMsg = useMsg ?? "You use the door to " + To.Name + ".".Reset();
			_lockMsg = lockMsg;
			InternalVerbs.Add(new Verb("Use", GetName, UseAction));

			DoorUsed += DefaultDoorAction;
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