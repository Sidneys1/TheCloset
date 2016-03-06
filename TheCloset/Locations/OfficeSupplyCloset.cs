using System.Dynamic;
using TheCloset.TextAdventure;

namespace TheCloset.Locations {

	internal class OfficeSupplyCloset : Location {
		public static OfficeSupplyCloset Instance { get; set; }

		public OfficeSupplyCloset() {
			Instance = this;
		}
	}
}