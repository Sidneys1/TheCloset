using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.ConsoleHelpers;
using TheCloset.GenericProps;
using TheCloset.TextAdventure;

namespace TheCloset.Locations {

	internal class Hallway : Location {

		#region Fields

		public static Hallway Instance;
		public Door[] H1, H2, H3;
		private readonly CommandPart[] _sectionCommands;
		private int _currSec = 1;
		private readonly Verb _walkVerb;

		#endregion Fields


		#region Constructors

		public Hallway() {
			Instance = this;

			_sectionCommands = new[] {
				new CommandPart("first section of the hallway".Yellow()),
				new CommandPart("second section of the hallway".Yellow()),
				new CommandPart("third section of the hallway".Yellow())
			};

			var closetDoor = new Door(this, TheCloset.Instance, new FormattedString("door to the ", "Utility Closet".Yellow()), 2, 3);

			H1 = new[] {
				closetDoor,
				new Door(this, new Exit(), "Exit", 0, 4) {Locked = true},
				new Door(this, new OfficeSupplyCloset(), new FormattedString("door to ","Office Supply Closet".Yellow()), 2, 5)
			};
			closetDoor.DoorUsed += door => { if (TheCloset.Instance.DoorToHallway.Locked) TheCloset.Instance.DoorToHallway.Locked = false; Player.Instance.ChangeLocation(TheCloset.Instance); };
			Props.AddRange(H1);

			InternalVerbs.Add(new Verb("Walk to the", GetSections, WalkToSection));

			_walkVerb = new Verb("Walk to the", GetDistantPropNames, WalkToThe) { Enabled = Props.Except(AdjacentProps).Any() };
			InternalVerbs.Add(_walkVerb);

			Player.Instance.PlayerMoved += PlayerMoved;
		}

		#endregion Constructors


		#region Methods

		private IEnumerable<CommandPart> GetSections() {
			switch (_currSec) {
				case 1:
					yield return _sectionCommands[1];
					yield return _sectionCommands[2];
					break;

				case 2:
					yield return _sectionCommands[0];
					yield return _sectionCommands[1];
					break;

				case 3:
					yield return _sectionCommands[0];
					yield return _sectionCommands[1];
					break;
			}
		}

		private void WalkToSection(string s) {
			Props.Clear();
			if (s.EndsWith("first section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H1);
				_currSec = 1;
				Player.Instance.SetPosition(3, 4);
			} else if (s.EndsWith("second section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H2);
				_currSec = 2;
				Player.Instance.SetPosition(9, 4);
			} else if (s.EndsWith("third section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H3);
				_currSec = 3;
				Player.Instance.SetPosition(16, 4);
			}
		}

		private IEnumerable<CommandPart> GetDistantPropNames() =>
			Props.Except(AdjacentProps).Select(o => new CommandPart(o.Name));

		private void WalkToThe(string s) {
			var p = Props.First(o => s.EndsWith(o.Name.ToString(), StringComparison.InvariantCultureIgnoreCase));
			Player.Instance.SetPosition(p.X, p.Y);
			Game.Instace.OutputPane.Write(new FormattedString("You walk to the ") + p.Name);
		}

		private void PlayerMoved() =>
			_walkVerb.Enabled = Props.Except(AdjacentProps).Any();

		#endregion Methods
	}
}