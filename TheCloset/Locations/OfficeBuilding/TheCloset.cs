using System.Linq;
using TheCloset.ConsoleHelpers;
using TheCloset.GenericProps;
using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding {

	internal class TheCloset : Location {
		#region Properties

		public static TheCloset Instance { get; private set; }

		public Door DoorToHallway { get; }

		#endregion Properties

		#region Constructors

		public TheCloset() : base("the Closet") {
			Instance = this;
			Props.Add(new Shelf(this));
			Props.Add(new Toolbox(this));
			Props.Add(new BlindFold(this));
			Props.Add(new Rope(this));
			Props.Add(new Box(this));
			DoorToHallway = new Door(this, new Hallway(), "the door", 2, 3, lockMsg: "There is no handle. You could probably open it from the other side...") { Locked = true };
			//DoorToHallway.DoorUsed += door => { if (!door.Locked) Player.Instance.ChangeLocation(Hallway.Instance); };
			Props.Add(DoorToHallway);

			InternalVerbs.Add(new Verb("Look around", s =>
			{
				var props = Props.Select(o => o.Name).ToArray();
				if (props.Length > 0) {
					Game.Instace.OutputPane.Write("You are next to ");
					if (props.Length == 1)
						Game.Instace.OutputPane.Write(props.First() + ". ");
					else {
						Game.Instace.OutputPane.Write(FormattedString.Join(", ", props.WithoutLast()));
						Game.Instace.OutputPane.Write(new FormattedString(", and ") + props.Last() + ". ");
					}
				}
			}));
		}

		#endregion Constructors

		#region Classes

		internal class DarkPlace : Location {
			#region Fields

			private readonly Verb _blind;
			private readonly Verb _escape;
			private readonly Verb _wiggle;
			private bool _bound = true;

			#endregion Fields

			#region Constructors

			public DarkPlace() : base("a Dark Place") {
				_blind = new Verb(new FormattedString("Remove the ", "blindfold".Cyan()), BlindMethod);
				_escape = new Verb("Exert yourself!", EscapeMethod);
				_wiggle = new Verb("Wiggle my arms", WiggleMethod);

				InternalVerbs.Add(new Verb("Look around", LookMethod));
				InternalVerbs.Add(_wiggle);
			}

			#endregion Constructors

			#region Methods

			private void BlindMethod(string s) {
				Game.PrintUserInterface();
				var c = new TheCloset();
				Game.Instace.OutputPane.Write("You reach up and remove your " + "blindfold".Cyan() + ". You are in " + c.Name + ".");
				InternalVerbs.Remove(_blind);
				Player.Instance.ChangeLocation(c);
			}

			private void EscapeMethod(string s) {
				Game.Instace.OutputPane.Write(new FormattedString("You wrench the ", "ropes".Cyan(), " with all your strength. They give way."));
				InternalVerbs.Remove(_escape);
				InternalVerbs.Add(_blind);
				_bound = false;
			}

			private void LookMethod(string s) {
				Game.Instace.OutputPane.Write(new FormattedString("You can't see anything. You have a ", "blindfold".Cyan(), " on."));
				if (_bound)
					Game.Instace.OutputPane.Write(" Your wrists are bound behind you back.");
			}

			private void WiggleMethod(string s) {
				Game.Instace.OutputPane.Write("You struggle against your bindings. They loosen a little.");
				InternalVerbs.Remove(_wiggle);
				InternalVerbs.Add(_escape);
			}

			#endregion Methods
		}

		internal class OnTheBox : Location {
			#region Fields

			private readonly TheCloset _closet;
			private readonly Verb _unscrewVerb;

			#endregion Fields

			#region Constructors

			public OnTheBox(TheCloset closet): base("the Box") {
				_closet = closet;
				_unscrewVerb = new Verb("Unscrew the vent", UnscrewVerb);
				InternalVerbs.Add(new Verb("Climb down", ClimbDownVerb));
				InternalVerbs.Add(_unscrewVerb);
			}

			#endregion Constructors

			#region Methods

			private void ClimbDownVerb(string s) {
				Player.Instance.ChangeLocation(_closet);
			}

			private void UnscrewVerb(string s) {
				if (Player.Instance.Items.All(o => o.Name.ToString() != "Screwdriver")) {
					Game.Instace.OutputPane.Write(new FormattedString("You do not have a ",
						"Screwdriver".Cyan(), "."));
					return;
				}
				Game.Instace.OutputPane.Write(new FormattedString("You use the ", "Screwdriver".Cyan(),
					" to open the ", "vent".Magenta(), "."));
				InternalVerbs.Remove(_unscrewVerb);
				Props.Add(new TheVents.Vent(this, new TheVents(this), "vent", true, 3, 2));
			}

			#endregion Methods
		}

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

			private readonly Verb _pickUpBookVerb;
			private bool _hasBook = true;

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
				#region Fields

				private readonly Verb _inspectVerb;

				#endregion Fields

				#region Constructors

				public NotebookItem() : base("Notebook") {
					_inspectVerb = new Verb(new FormattedString("Inspect the ", "Notebook".Cyan()),
						s =>
						{
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