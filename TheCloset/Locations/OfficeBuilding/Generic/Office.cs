using System;
using System.Collections.Generic;
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
		#region Fields

		private readonly Verb _walkVerb;

		public Door DoorToHallway { get; }

		#endregion Fields


		#region Constructors

		public Office(OfficeType type, bool top, int x, int y) {
			Props.Add(new Desk(this, type, x - 1, top ? y - 3 : y + 3));

			DoorToHallway = new Door(this, Hallway.Instance, "the door", x, y);
			DoorToHallway.DoorUsed += door => Player.Instance.ChangeLocation(door.To);
			Props.Add(DoorToHallway);

			Player.Instance.PlayerMoved += PlayerMoved;
			_walkVerb = new Verb("Walk to", GetDistantPropNames, WalkToThe) { Enabled = Props.Except(AdjacentProps).Any() };
			InternalVerbs.Add(_walkVerb);

			InternalVerbs.Add(new Verb("Look around", s =>
			{
				var props = AdjacentProps.Select(o => o.Name).ToArray();
				var near =
					Props.Where(o => Math.Abs(Player.Instance.X - o.X) <= 3 && Math.Abs(Player.Instance.Y - o.Y) <= 3)
						.Except(AdjacentProps)
						.Select(o => o.Name)
						.ToArray();
				if (props.Length > 0) {
					Game.Instace.OutputPane.Write("You are next to ");
					if (props.Length == 1)
						Game.Instace.OutputPane.Write(props.First() + ". ");
					else {
						Game.Instace.OutputPane.Write(FormattedString.Join(", ", props.WithoutLast()));
						Game.Instace.OutputPane.Write(new FormattedString(", and ") + props.Last() + ". ");
					}
				}
				if (near.Length > 0) {
					Game.Instace.OutputPane.Write("Not far away is ");
					if (props.Length == 1)
						Game.Instace.OutputPane.Write(near.First() + ". ");
					else {
						Game.Instace.OutputPane.Write(FormattedString.Join(", ", near.WithoutLast()));
						Game.Instace.OutputPane.Write(new FormattedString(", and ") + near.Last() + ". ");
					}
				}
			}));
		}

		#endregion Constructors


		#region Methods

		private IEnumerable<CommandPart> GetDistantPropNames() =>
			Props.Except(AdjacentProps).Select(o => new CommandPart(o.Name));

		private void PlayerMoved() =>
			_walkVerb.Enabled = Props.Except(AdjacentProps).Any();

		private void WalkToThe(string s) {
			var p = Props.First(o => s.EndsWith(o.Name.ToString(), StringComparison.InvariantCultureIgnoreCase));
			Player.Instance.SetPosition(p.X, p.Y);
			Game.Instace.OutputPane.Write(new FormattedString("You walk to ") + p.Name);
		}

		#endregion Methods


		#region Classes

		private class Desk : Prop {
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

			private class KeyItem : Item {

				#region Constructors

				public KeyItem() : base("Key labelled 'T1'") { }

				#endregion Constructors
			}

			private class MugItem : Item {

				#region Constructors

				public MugItem() : base("Mug") { }

				#endregion Constructors
			}
			#endregion Classes
		}

		#endregion Classes
	}
}
