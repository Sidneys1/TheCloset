using System;
using System.Collections.Generic;
using TheCloset.GenericProps;
using TheCloset.TextAdventure;

namespace TheCloset.Locations {
	internal class Hallway : Location {
		public static Hallway Instance;
		private readonly CommandPart[] _sectionCommands;
		private int _currSec = 1;
		public Door[] H1, H2, H3;

		public Hallway() {
			Instance = this;

			_sectionCommands = new[] {
				new CommandPart("first section of the hallway"),
				new CommandPart("second section of the hallway"),
				new CommandPart("third section of the hallway")
			};

			H1 = new[] {
				new Door(this, TheCloset.Instance, "door to the Utility Closet", 2, 3),
				new Door(this, new Exit(), "Exit", 0, 4)
				//new Door(this, new Exit(Player), "door to Office Supply closet", 2, 5),
			};
			Props.AddRange(H1);

			InternalVerbs.Add(new Verb("Walk to the", GetSections, WalkToSection));
		}

		private void WalkToSection(string s) {
			Props.Clear();
			if (s.EndsWith("first section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H1);
				_currSec = 1;
				Player.Instance.SetPosition(3, 4);
			}
			else if (s.EndsWith("second section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H2);
				_currSec = 2;
				Player.Instance.SetPosition(9, 4);
			}
			else if (s.EndsWith("third section of the hallway", StringComparison.InvariantCultureIgnoreCase)) {
				Props.AddRange(H3);
				_currSec = 3;
				Player.Instance.SetPosition(16, 4);
			}
		}

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
	}
}