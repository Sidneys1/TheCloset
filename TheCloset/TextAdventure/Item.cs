using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.ConsoleHelpers;
using LiquidAmounts = System.Collections.Generic.Dictionary<string, float>;
using LiquidUnits = System.Collections.Generic.Dictionary<string, string>;
using ReadOnlyLiquidAmounts = System.Collections.Generic.IReadOnlyDictionary<string, float>;
using ReadOnlyLiquidUnits = System.Collections.Generic.IReadOnlyDictionary<string, string>;

namespace TheCloset.TextAdventure {

	public abstract class Item : IVerbable {

		#region Fields

		protected readonly List<Verb> InnerVerbs = new List<Verb>();

		#endregion Fields


		#region Properties

		public FormattedString Name { get; private set; }

		public IEnumerable<Verb> Verbs => InnerVerbs;

		#endregion Properties


		#region Constructors

		protected Item(string name) {
			Name = name.Cyan();
		}

		protected Item(FormattedString name) {
			Name = name;
		}

		#endregion Constructors
	}

	public abstract class LiquidContainerItem : Item {

		#region Properties

		public ReadOnlyLiquidAmounts LiquidMax => InternalLiquidStorage;

		public ReadOnlyLiquidAmounts LiquidStorage => InternalLiquidStorage;

		public IReadOnlyList<string> LiquidTypes => InternalLiquidTypes;

		public ReadOnlyLiquidUnits LiquidUnits => InternalLiquidUnits;

		public float TotalAmount => InternalLiquidStorage.Sum(o => o.Value);

		public float TotalMax => InternalLiquidMax.Sum(o => o.Value);

		protected LiquidAmounts InternalLiquidMax { get; } = new LiquidAmounts();

		protected LiquidAmounts InternalLiquidStorage { get; } = new LiquidAmounts();

		protected List<string> InternalLiquidTypes { get; } = new List<string>();

		protected LiquidUnits InternalLiquidUnits { get; } = new LiquidUnits();

		#endregion Properties


		#region Constructors

		protected LiquidContainerItem(string name, params LiquidDef[] liquidTypes) : base(name) {
			Init(liquidTypes);
		}

		protected LiquidContainerItem(FormattedString name, params LiquidDef[] liquidTypes) : base(name) {
			Init(liquidTypes);
		}

		#endregion Constructors


		#region Methods

		private void Init(IEnumerable<LiquidDef> liquidTypes) {
			foreach (var liquidType in liquidTypes) {
				InternalLiquidTypes.Add(liquidType.LiquidType);
				InternalLiquidStorage[liquidType.LiquidType] = 0;
				InternalLiquidMax[liquidType.LiquidType] = liquidType.Max;
				InternalLiquidUnits[liquidType.LiquidType] = liquidType.Unit;
			}
		}

		#endregion Methods


		#region Structs

		public struct LiquidDef {

			#region Fields

			public string LiquidType;

			public float Max;

			public string Unit;

			#endregion Fields


			#region Constructors

			public LiquidDef(string liquidType, string unit, float max) {
				LiquidType = liquidType;
				Unit = unit;
				Max = max;
			}

			#endregion Constructors
		}

		#endregion Structs
	}
}