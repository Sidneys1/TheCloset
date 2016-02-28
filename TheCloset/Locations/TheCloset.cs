using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.GenericProps;
using TheCloset.Locations.InTheCloset;
using TheCloset.TextAdventure;

namespace TheCloset.Locations {
	internal class TheCloset : Location {
		private readonly Verb _walkVerb;

		public TheCloset() {
			Instance = this;
			Props.Add(new Shelf(this));
			Props.Add(new Toolbox(this));
			Props.Add(new BlindFold(this));
			Props.Add(new Rope(this));
			Props.Add(new Box(this));
			Props.Add(new Door(this, Hallway.Instance, "door to the hallway", 2, 3, lockMsg: "There is no handle.") {
				Locked = true
			});

			Player.Instance.PlayerMoved += PlayerMoved;
			_walkVerb = new Verb("Walk to the", GetDistantPropNames, WalkToThe) {Enabled = Props.Except(AdjacentProps).Any()};
			InternalVerbs.Add(_walkVerb);

			InternalVerbs.Add(new Verb("Look around", s => {
				var props = AdjacentProps.Select(o => o.Name).ToArray();
				var near =
					Props.Where(o => Math.Abs(Player.Instance.X - o.X) <= 3 && Math.Abs(Player.Instance.Y - o.Y) <= 3)
						.Except(AdjacentProps)
						.Select(o => o.Name)
						.ToArray();
				if (props.Length > 0)
					Game.Instace.OutputPane.Write(
						$"You are next to {(props.Length == 1 ? "" : string.Join(", ", props.WithoutLast().Select(o => $"the {o}")) + " and ")}the {props.Last()}. ");
				if (near.Length > 0)
					Game.Instace.OutputPane.Write(
						$"Not far away is {(near.Length == 1 ? "" : string.Join(", ", near.WithoutLast().Select(o => $"the {o}")) + " and ")}the {near.Last()}. ");
			}));
		}

		public static TheCloset Instance { get; private set; }

		private IEnumerable<CommandPart> GetDistantPropNames()
			=> Props.Except(AdjacentProps).Select(o => new CommandPart(o.Name));

		private void WalkToThe(string s) {
			var p = Props.First(o => s.EndsWith(o.Name, StringComparison.InvariantCultureIgnoreCase));
			Player.Instance.SetPosition(p.X, p.Y);
			Game.Instace.OutputPane.Write($"You walk to the {p.Name}");
		}

		private void PlayerMoved() => _walkVerb.Enabled = Props.Except(AdjacentProps).Any();

		#region Props

		private class BlindFold : Prop {
			public BlindFold(Location parent) : base(parent, "Blindfold", 0, 0) {
				InternalVerbs.Add(new Verb("Pick up the Blindfold", s => {
					Parent.Props.Remove(this);
					Player.Instance.AddItem(new BlindFoldItem());
				}));
			}

			private class BlindFoldItem : Item {
				private readonly Verb _putOnVerb;

				public BlindFoldItem() : base("Blindfold") {
					_putOnVerb = new Verb("Put on the blindfold", s => {
						Game.Instace.OutputPane.Write("No, that would be silly...");
						_putOnVerb.Enabled = false;
					});
					InnerVerbs.Add(_putOnVerb);
				}
			}
		}

		private class Rope : Prop {
			public Rope(Location parent) : base(parent, "Rope", 0, 0) {
				InternalVerbs.Add(new Verb("Pick up the Rope", s => {
					Parent.Props.Remove(this);
					Player.Instance.AddItem(new RopeItem());
				}));
			}

			private class RopeItem : Item {
				public RopeItem() : base("Frayed Rope") {}
			}
		}

		private class Shelf : Prop {
			private readonly bool _hasBook = true;
			private readonly Verb _pickUpBookVerb;

			public Shelf(Location parent) : base(parent, "shelf", 2, 0) {
				_pickUpBookVerb = new Verb("Pick up the Notebook", s => {
					InternalVerbs.Remove(_pickUpBookVerb);
					Player.Instance.AddItem(new NotebookItem());
				});
				InternalVerbs.Add(new Verb("Inspect the shelf", s => {
					Game.Instace.OutputPane.Write("There is a book on the shelf.");
					if (_hasBook)
						InternalVerbs.Add(_pickUpBookVerb);
				}));
			}

			private class NotebookItem : Item {
				public NotebookItem() : base("Notebook") {
					InnerVerbs.Add(new Verb("Inspect the notebook",
						s => Game.Instace.OutputPane.Write("It was written by 'Ricky'. It is full of sorrowful notes.")));
				}
			}
		}

		private class Toolbox : Prop {
			private readonly Verb _getScrewdriverVerb;
			private bool _hasScrewdriver = true;

			public Toolbox(Location parent) : base(parent, "toolbox", 0, 2) {
				_getScrewdriverVerb = new Verb("Pick up the screwdriver", s => {
					Game.Instace.OutputPane.Write("You stash it in your inventory");
					Player.Instance.AddItem(new ScrewDriver());
					_getScrewdriverVerb.Enabled = false;
					_hasScrewdriver = false;
				}) {Enabled = false};
				InternalVerbs.Add(new Verb("Inspect the toolbox", s => {
					Game.Instace.OutputPane.Write(_hasScrewdriver ? "There is a screwdriver." : "It is empty.");
					_getScrewdriverVerb.Enabled = _hasScrewdriver;
				}));
				InternalVerbs.Add(_getScrewdriverVerb);
			}

			private class ScrewDriver : Item {
				public ScrewDriver() : base("Screwdriver") {}
			}
		}

		private class Box : Prop {
			private readonly OnTheBox _onTheBox;

			public Box(TheCloset parent) : base(parent, "box", 3, 2) {
				_onTheBox = new OnTheBox(parent);
				InternalVerbs.Add(new Verb("Climb the box", s => {
					Game.Instace.OutputPane.Write("You can just barely reach a ceiling vent.");
					Player.Instance.ChangeLocation(_onTheBox);
				}));
			}
		}

		#endregion
	}
}