using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.TextAdventure;

namespace TheCloset.Locations {
	class TheCloset : Location {
		#region Props
		private class BlindFold : Prop {
			class BlindFoldItem : Item{
				readonly Verb putOnVerb;
				public BlindFoldItem(Player p) : base(p, "Blindfold") {
					putOnVerb = new Verb("Put on the blindfold.", s => { Console.Write("No, that would be silly..."); putOnVerb.Enabled = false; });
                    InnerVerbs.Add(putOnVerb);
				}
			}
			public BlindFold(Location parent) : base(parent, "Blindfold", 0, 0) {
				InternalVerbs.Add(new Verb("Pick up the Blindfold.", s => {
					Parent.Props.Remove(this);
					Parent.Player.AddItem(new BlindFoldItem(parent.Player));
				}));
			}
		}
		private class Rope : Prop {
			class RopeItem : Item {
				public RopeItem(Player p) : base(p, "Frayed Rope") { }
			}
			public Rope(Location parent) : base(parent, "Rope", 0, 0) {
				InternalVerbs.Add(new Verb("Pick up the Rope.", s => {
					Parent.Props.Remove(this);
					Parent.Player.AddItem(new RopeItem(Parent.Player));
				}));
			}
		}
		private class Shelf : Prop {
			class NotebookItem : Item {
				public NotebookItem(Player parent) : base(parent, "Notebook") {
					InnerVerbs.Add(new Verb("Inspect the notebook.", s => Console.Write("It was written by 'Ricky'. It is full of sorrowful notes.")));
				}
			}
			readonly Verb _pickUpBookVerb;
			bool _hasBook = true;
			public Shelf(Location parent) : base(parent, "shelf", 1, 1) {
				_pickUpBookVerb = new Verb("Pick up the Notebook.", s => {
					InternalVerbs.Remove(_pickUpBookVerb);
					Parent.Player.AddItem(new NotebookItem(Parent.Player));
				});
				InternalVerbs.Add(new Verb("Inspect the shelf.", s => {
					Console.Write("There is a book on the shelf.");
					if (_hasBook)
						InternalVerbs.Add(_pickUpBookVerb);
				}));
			}
		}
		private class Toolbox : Prop {
			private class ScrewDriver : Item {
				public ScrewDriver(Player p) : base(p, "Screwdriver") {}
			}
			bool _hasScrewdriver=true;
			readonly Verb _getScrewdriverVerb;
			public Toolbox(Location parent) : base(parent, "toolbox", -2, -2) {
				_getScrewdriverVerb = new Verb("Pick up the screwdriver.", s => {
					Console.Write("You stash it in your inventory");
					Parent.Player.AddItem(new ScrewDriver(parent.Player));
					_getScrewdriverVerb.Enabled = false;
					_hasScrewdriver = false;
				}) {Enabled = false};
				InternalVerbs.Add(new Verb("Inspect the toolbox.", s =>
				{
					Console.Write(_hasScrewdriver ? "There is a screwdriver." : "It is empty.");
					_getScrewdriverVerb.Enabled = _hasScrewdriver;
				}));
				InternalVerbs.Add(_getScrewdriverVerb);
			}
		}
		#endregion

		readonly Verb _walkVerb;
		public TheCloset(Player p) : base(p) {
			Props.Add(new Shelf(this));
			Props.Add(new Toolbox(this));
			Props.Add(new BlindFold(this));
			Props.Add(new Rope(this));
			p.PlayerMoved += PlayerMoved;
			_walkVerb = new Verb("Walk to the", GetAdjacentPropsNames, WalkToThe) { Enabled = Props.Except(AdjacentProps).Any() };
			InternalVerbs.Add(_walkVerb);

			InternalVerbs.Add(new Verb("Look around.", s =>
			{
				var props = AdjacentProps.Select(o => o.Name).ToArray();
				var near = Props.Where(o => Math.Abs(Player.X - o.X) <= 3 && Math.Abs(Player.Y - o.Y) <= 3).Except(AdjacentProps).Select(o => o.Name).ToArray();
				if (props.Length > 0)
					Console.Write($"You are next to {(props.Length == 1 ? "" : string.Join(", ", props.WithoutLast().Select(o => $"the {o}")) + " and ")}the {props.Last()}. ");
				if (near.Length > 0)
					Console.Write($"Not far away is {(near.Length == 1 ? "" : string.Join(", ", near.WithoutLast().Select(o => $"the {o}")) + " and ")}the {near.Last()}. ");
			}));
		}

		private IEnumerable<CommandPart> GetAdjacentPropsNames() {
			return Props.Except(AdjacentProps).Select(o => new CommandPart(o.Name + "."));
		}

		private void WalkToThe(string s) {
			var x = s.Split(' ').Last();
			x = x.Substring(0, x.Length - 1);
			var p = Props.First(o => o.Name.Equals(x, StringComparison.InvariantCultureIgnoreCase));
			Player.SetPosition(p.X, p.Y);
			Console.Write("You walk to the {0}", x);
		}

		private void PlayerMoved() {
			_walkVerb.Enabled = Props.Except(AdjacentProps).Any();
		}
	}
}
