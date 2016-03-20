using System;
using System.Collections.Generic;
using System.Linq;

namespace TheCloset.TextAdventure {

	public class Player : IVerbable {
		#region Fields

		private readonly List<Item> _items = new List<Item>();

		#endregion Fields

		#region Properties

		public static Player Instance { get; private set; }

		public Location CurrentLocation { get; private set; }

		public Dictionary<string, string> GlobalVars { get; } = new Dictionary<string, string>();

		public IReadOnlyList<Item> Items => _items.AsReadOnly();

		public IEnumerable<Verb> Verbs
		{
			get
			{
				foreach (var verb in CurrentLocation.Verbs)
					yield return verb;
				foreach (var verb in Items.SelectMany(o => o.Verbs))
					yield return verb;
			}
		}

		#endregion Properties

		#region Events

		public event Action LocationChanged;

		#endregion Events

		#region Constructors

		public Player() {
			Instance = this;
		}

		#endregion Constructors

		#region Methods

		public void AddItem(Item i) => _items.Add(i);

		public void ChangeLocation(Location l) {
			CurrentLocation = l;
			LocationChanged?.Invoke();
		}

		#endregion Methods
	}
}