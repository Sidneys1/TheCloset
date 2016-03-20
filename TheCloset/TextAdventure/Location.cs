using System.Collections.Generic;
using System.Linq;

namespace TheCloset.TextAdventure {

	public abstract class Location : IVerbable {
		#region Properties

		public List<Prop> Props { get; } = new List<Prop>();

		public IEnumerable<Verb> Verbs
		{
			get
			{
				foreach (var internalVerb in InternalVerbs)
					yield return internalVerb;
				foreach (var verb in Props.SelectMany(o => o.Verbs))
					yield return verb;
			}
		}

		protected List<Verb> InternalVerbs { get; } = new List<Verb>();

		#endregion Properties
	}
}