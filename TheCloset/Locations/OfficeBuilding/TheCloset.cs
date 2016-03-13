using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.ConsoleHelpers;
using TheCloset.GenericProps;
using TheCloset.Locations.OfficeBuilding.InTheCloset;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding {

	internal class TheCloset : Location {

		#region Fields

		private readonly Verb _walkVerb;

		public Door DoorToHallway { get; }

		#endregion Fields

		#region Constructors

		public TheCloset() {
			Instance = this;
			Props.Add(new Shelf(this));
			Props.Add(new Toolbox(this));
			Props.Add(new BlindFold(this));
			Props.Add(new Rope(this));
			Props.Add(new Box(this));
			DoorToHallway = new Door(this, Hallway.Instance, "the door", 2, 3, lockMsg: "There is no handle.") { Locked = true };
			DoorToHallway.DoorUsed += door => { if (!door.Locked) Player.Instance.ChangeLocation(Hallway.Instance); };
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
				if (near.Length <= 0) return;
				Game.Instace.OutputPane.Write("Not far away is ");
				if (props.Length == 1)
					Game.Instace.OutputPane.Write(near.First() + ". ");
				else {
					Game.Instace.OutputPane.Write(FormattedString.Join(", ", near.WithoutLast()));
					Game.Instace.OutputPane.Write(new FormattedString(", and ") + near.Last() + ". ");
				}
			}));
		}

		#endregion Constructors

		#region Properties

		public static TheCloset Instance { get; private set; }

		#endregion Properties

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

		private class BlindFold : Prop {

			#region Constructors

			public BlindFold(Location parent) : base(parent, "the Blindfold".Cyan(), 0, 0) {
				InternalVerbs.Add(new Verb(new FormattedString("Pick up the ", "Blindfold".Cyan()), s =>
				{
					Parent.Props.Remove(this);
					Player.Instance.AddItem(new BlindFoldItem());
				}));
			}

			#endregion Constructors

			#region Classes

			private class BlindFoldItem : Item {

				#region Fields

				private readonly Verb _putOnVerb;

				#endregion Fields

				#region Constructors

				public BlindFoldItem() : base("Blindfold") {
					_putOnVerb = new Verb(new FormattedString("Put on the ", "Blindfold".Cyan()), s =>
					{
						Game.Instace.OutputPane.Write("No, that would be silly...");
						_putOnVerb.Enabled = false;
					});
					InnerVerbs.Add(_putOnVerb);
				}

				#endregion Constructors
			}

			#endregion Classes
		}

		private class Box : Prop {

			#region Fields

			private readonly OnTheBox _onTheBox;

			#endregion Fields

			#region Constructors

			public Box(TheCloset parent) : base(parent, "the box", 3, 2) {
				_onTheBox = new OnTheBox(parent);
				InternalVerbs.Add(new Verb(new FormattedString("Climb the ", "box".Magenta()), s =>
				{
					Game.Instace.OutputPane.Write(new FormattedString("You can just barely reach a ", "ceiling vent".Magenta(), "."));
					Player.Instance.ChangeLocation(_onTheBox);
				}));
			}

			#endregion Constructors
		}

		private class Rope : Prop {

			#region Constructors

			public Rope(Location parent) : base(parent, "the Rope".Cyan(), 0, 0) {
				InternalVerbs.Add(new Verb(new FormattedString("Pick up the ", "Rope".Cyan()), s =>
				{
					Parent.Props.Remove(this);
					Player.Instance.AddItem(new RopeItem());
				}));
			}

			#endregion Constructors

			#region Classes

			private class RopeItem : Item {

				#region Constructors

				public RopeItem() : base("Longish Rope") {
				}

				#endregion Constructors
			}

			#endregion Classes
		}

		private class Shelf : Prop {

			#region Fields

			private bool _hasBook = true;
			private readonly Verb _pickUpBookVerb;

			#endregion Fields

			#region Constructors

			public Shelf(Location parent) : base(parent, "the shelf", 2, 0) {
				_pickUpBookVerb = new Verb(new FormattedString("Pick up the ", "Notebook".Cyan()), s =>
				{
					_hasBook = false;
					InternalVerbs.Remove(_pickUpBookVerb);
					Player.Instance.AddItem(new NotebookItem());
				}) { Enabled = false };

				InternalVerbs.Add(_pickUpBookVerb);

				InternalVerbs.Add(new Verb(new FormattedString("Inspect ", "the shelf".Magenta()), s =>
				{
					
					if (!_hasBook) {
						Game.Instace.OutputPane.Write("There is nothing here.");
						return;
					}
					Game.Instace.OutputPane.Write(new FormattedString("There is a ", "Notebook".Cyan(), " on the ", "shelf.".Magenta()));

					_pickUpBookVerb.Enabled = true;
				}));
			}

			#endregion Constructors

			#region Classes

			private class NotebookItem : Item {

				private readonly Verb _inspectVerb;

				#region Constructors

				public NotebookItem() : base("Notebook") {
					_inspectVerb = new Verb(new FormattedString("Inspect the ", "Notebook".Cyan()),
						s => {
							Game.Instace.OutputPane.Write("It was written by 'Ricky'. It is full of sorrowful notes.");
							InnerVerbs.Remove(_inspectVerb);
						});
					InnerVerbs.Add(_inspectVerb);
				}

				#endregion Constructors
			}

			#endregion Classes
		}

		private class Toolbox : Prop {

			#region Fields

			private readonly Verb _getScrewdriverVerb;
			private bool _hasScrewdriver = true;

			#endregion Fields

			#region Constructors

			public Toolbox(Location parent) : base(parent, "the toolbox", 0, 2) {
				_getScrewdriverVerb = new Verb(new FormattedString("Pick up the ", "Screwdriver".Cyan()), s =>
				{
					Game.Instace.OutputPane.Write("You stash it in your inventory");
					Player.Instance.AddItem(new ScrewDriver());
					_getScrewdriverVerb.Enabled = false;
					_hasScrewdriver = false;
				}) { Enabled = false };
				InternalVerbs.Add(new Verb(new FormattedString("Inspect ", "the toolbox".Magenta()), s =>
				{
					Game.Instace.OutputPane.Write(_hasScrewdriver ? new FormattedString("There is a ", "Screwdriver".Cyan(), ".") : "It is empty.");
					_getScrewdriverVerb.Enabled = _hasScrewdriver;
				}));
				InternalVerbs.Add(_getScrewdriverVerb);
			}

			#endregion Constructors

			#region Classes

			private class ScrewDriver : Item {

				#region Constructors

				public ScrewDriver() : base("Screwdriver") {
				}

				#endregion Constructors
			}

			#endregion Classes
		}

		#endregion Classes
	}
}