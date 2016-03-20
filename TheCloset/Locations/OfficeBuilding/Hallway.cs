using System;
using System.Collections.Generic;
using TheCloset.ConsoleHelpers;
using TheCloset.GenericProps;
using TheCloset.Locations.OfficeBuilding.Generic;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding {

	internal class Hallway : Location {
		#region Fields

		public static Hallway Instance;
		public Door[] H1, H2, H3;
		private readonly CommandPart[] _sectionCommands;
		private int _currSec = 1;

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

			var mugO = Game.Random.Next(5);
			var keyO = mugO;
			while (keyO == mugO)
				keyO = Game.Random.Next(5);

			H2 = new Door[6];

			for (int i = 0; i < H2.Length; i++) {
				var type = i == mugO ? OfficeType.Mug : (i == keyO ? OfficeType.Key : OfficeType.Empty);
				var x = new[] { 5, 5, 9, 9, 13, 13 };
				var y = new[] { 3, 5, 3, 5, 3, 5 };
				var l = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };

				H2[i] = new Door(this, new Office(type, i % 2 == 0, x[i], y[i]), new FormattedString("door to ", $"Office {l[i]}".Yellow()), x[i], y[i]);
				H2[i].DoorUsed += door => Player.Instance.ChangeLocation(door.To);
			}

			closetDoor.DoorUsed += door =>
			{
				if (TheCloset.Instance.DoorToHallway.Locked) {
					TheCloset.Instance.DoorToHallway.Locked = false;
					TheCloset.Instance.DoorToHallway.Name = new FormattedString("the door to ", "the Hallway".Yellow());
				}
				Player.Instance.ChangeLocation(TheCloset.Instance);
			};
			Props.AddRange(H1);

			InternalVerbs.Add(new Verb("Walk to the", GetSections, WalkToSection));
		}

		#endregion Constructors

		#region Methods

		private IEnumerable<CommandPart> GetSections() {
			switch (_currSec) {
				case 1:
					yield return _sectionCommands[1];
					break;

				case 2:
					yield return _sectionCommands[0];
					yield return _sectionCommands[2];
					break;

				case 3:
					yield return _sectionCommands[1];
					break;
			}
		}

		private void WalkToSection(string s) {
			Props.Clear();
			if (s.EndsWith("first section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H1);
				_currSec = 1;
			} else if (s.EndsWith("second section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H2);
				_currSec = 2;
			} else if (s.EndsWith("third section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H3);
				_currSec = 3;
			}
		}

		#endregion Methods
	}
}