using System;
using System.Linq;
using TheCloset.ConsoleHelpers;
using TheCloset.GenericProps;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding.Generic {

	public enum OfficeType {
		Mug,
		Key,
		Empty
	}

	public class Office : Location {
		#region Properties

		public Door DoorToHallway { get; }

		#endregion Properties

		#region Constructors

		public Office(string name, OfficeType type, bool top, int x, int y) : base(name) {
			Props.Add(new Desk(this, type, x - 1, top ? y - 3 : y + 3));

			DoorToHallway = new Door(this, Hallway.Instance, "the door", x, y);
			//DoorToHallway.DoorUsed += door => Player.Instance.ChangeLocation(door.To);
			Props.Add(DoorToHallway);

			InternalVerbs.Add(new Verb("Look around", s =>
			{
				var props = Props.Select(o => o.Name).ToArray();
				if (props.Length <= 0) return;
				Game.Instace.OutputPane.Write("You are next to ");
				if (props.Length == 1)
					Game.Instace.OutputPane.Write(props.First() + ".");
				else {
					Game.Instace.OutputPane.Write(FormattedString.Join(", ", props.WithoutLast()));
					Game.Instace.OutputPane.Write(new FormattedString(", and ") + props.Last() + ".");
				}
			}));
		}

		#endregion Constructors

		#region Classes

		public class Desk : Prop {
			#region Fields

			private readonly Item _item;
			private readonly Verb _pickUpItemVerb;
			private bool _hasItem;

			#endregion Fields

			#region Constructors

			public Desk(Location parent, OfficeType type, int x, int y) : base(parent, "the desk", x, y) {
				_item = null;
				switch (type) {
					case OfficeType.Mug:
						_item = new MugItem();
						break;

					case OfficeType.Key:
						_item = new KeyItem();
						break;
				}
				_hasItem = _item != null;
				InternalVerbs.Add(new Verb(new FormattedString("Inspect ", "the desk".Magenta()), s =>
				{
					Game.Instace.OutputPane.Write(
							new FormattedString("There is nothing here but ",
							"a phone".Magenta(),
							" with no connection, ",
							"a computer".Magenta(),
							" with no power, and ",
							"various knick-knacks".Magenta(),
							" and ",
							"paperweights".Magenta(),
							". "));
					if (!_hasItem) return;
					Game.Instace.OutputPane.Write(new FormattedString("There is a ", _item.Name.Sections[0], " on ",
						"the desk.".Magenta()));
					_pickUpItemVerb.Enabled = true;
				}));

				if (_item == null)
					return;

				_pickUpItemVerb = new Verb("Pick up the " + _item.Name, s =>
				{
					_hasItem = false;
					InternalVerbs.Remove(_pickUpItemVerb);
					Player.Instance.AddItem(_item);
				}) { Enabled = false };
				InternalVerbs.Add(_pickUpItemVerb);
			}

			#endregion Constructors

			#region Classes

			public class MugItem : LiquidContainerItem {
				#region Fields

				private readonly Verb _drinkVerb;

				#endregion Fields

				#region Constructors

				public MugItem() : base("Mug", new LiquidDef("Coffee", "oz", 8)) {
					LiquidStorage["Coffee"] = (float)(Game.Random.NextDouble() * 8);
					_drinkVerb = new Verb("Drink some", () => LiquidStorage.Where(o => o.Value > 0).Select(o => new CommandPart(o.Key)), s =>
					{
						var t = s.Split(' ').Last();
						var k = LiquidStorage.First(o => o.Key.Equals(t, StringComparison.InvariantCultureIgnoreCase)).Key;
						if (k == null) return;
						LiquidStorage[k] -= Math.Min(LiquidStorage[k], 0.5f);
						_drinkVerb.Enabled = LiquidStorage.Any(o => o.Value > 0);
					});
					InnerVerbs.Add(_drinkVerb);
				}

				#endregion Constructors
			}

			private class KeyItem : Item {
				#region Constructors

				public KeyItem() : base("Key labelled 'T1'") {
				}

				#endregion Constructors
			}

			#endregion Classes
		}

		#endregion Classes
	}
}