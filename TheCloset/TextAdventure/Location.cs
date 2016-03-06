using System.Collections.Generic;
using System.Linq;

namespace TheCloset.TextAdventure {

	public abstract class Location : IVerbable {

		#region Properties

		public IEnumerable<Prop> AdjacentProps => Props.Where(o => Player.Instance.X == o.X && Player.Instance.Y == o.Y);
		public List<Prop> Props { get; } = new List<Prop>();
		public IEnumerable<Verb> Verbs
		{
			get
			{
				foreach (var internalVerb in InternalVerbs)
					yield return internalVerb;
				foreach (var verb in AdjacentProps.SelectMany(o => o.Verbs))
					yield return verb;
			}
		}

		protected List<Verb> InternalVerbs { get; } = new List<Verb>();

		#endregion Properties
	}
}