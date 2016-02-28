using System;
using System.Collections.Generic;
using System.Linq;

namespace TheCloset.TextAdventure {
	public class Player : IVerbable {
		public Player() {
			Instance = this;
		}

		public static Player Instance { get; private set; }
		public Location CurrentLocation { get; private set; }
		public Dictionary<string, string> GlobalVars { get; } = new Dictionary<string, string>();

		public IEnumerable<Verb> Verbs {
			get {
				//				foreach (var internalVerb in _internalVerbs)
				//					yield return internalVerb;
				foreach (var verb in CurrentLocation.Verbs)
					yield return verb;
				foreach (var verb in Items.SelectMany(o => o.Verbs))
					yield return verb;
			}
		}

		public event Action PlayerMoved;
		public event Action LocationChanged;

		public void ChangeLocation(Location l) {
			CurrentLocation = l;
			LocationChanged?.Invoke();
		}

		public void Start() {
			PlayerMoved?.Invoke();
		}

		#region Items

		private readonly List<Item> _items = new List<Item>();
		public IReadOnlyList<Item> Items => _items.AsReadOnly();

		public void AddItem(Item i) => _items.Add(i);

		#endregion

		#region Position

		public int X { get; private set; }

		public int Y { get; private set; }

		public void SetPosition(int x, int y) {
			X = x;
			Y = y;
			PlayerMoved?.Invoke();
		}

		#endregion
	}
}