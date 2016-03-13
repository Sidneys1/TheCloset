using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding {

	internal class OfficeSupplyCloset : Location {
		public static OfficeSupplyCloset Instance { get; set; }

		public OfficeSupplyCloset() {
			Instance = this;
		}
	}
}