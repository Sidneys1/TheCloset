using TheCloset.TextAdventure;

namespace TheCloset.Locations.OfficeBuilding {

	internal class OfficeSupplyCloset : Location {
		#region Properties

		public static OfficeSupplyCloset Instance { get; set; }

		#endregion Properties

		#region Constructors

		public OfficeSupplyCloset() {
			Instance = this;
		}

		#endregion Constructors
	}
}