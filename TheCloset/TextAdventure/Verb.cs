using System;
using System.Collections.Generic;
using TheCloset.ConsoleHelpers;

namespace TheCloset.TextAdventure {

	public class Verb {
		#region Fields

		public Action<string> Action;
		public CommandPart FirstPart;

		#endregion Fields

		#region Properties

		public int Depth => FirstPart.Depth();

		public bool Enabled { get; set; } = true;

		#endregion Properties

		#region Constructors

		public Verb(FormattedString parts, Action<string> action = null) {
			//FirstPart = CommandPart.FromArray(parts.Sections.Select(o=>(FormattedString)o).ToArray());
			FirstPart = new CommandPart(parts);
			Action = action;
		}

		public Verb(FormattedString parts, Func<IEnumerable<CommandPart>> enumerate, Action<string> action = null) {
			//FirstPart = CommandPart.FromArray(enumerate, parts.Sections.Select(o => (FormattedString)o).ToArray());
			FirstPart = new CommandPart(parts, enumerate);
			Action = action;
		}

		#endregion Constructors

		//public void Extend(FormattedString ext) {
		//	var parts = ext.Sections;
		//	if (parts[0].ToString() == FirstPart.ThisPart.ToString())
		//		FirstPart.Extend(parts.Skip(1).Select(o=>(FormattedString)o).ToArray());
		//}

		//public void Extend(FormattedString pre, CommandPart p) {
		//	var parts = pre.Sections;
		//	if (parts[0].ToString() == FirstPart.ThisPart.ToString())
		//		FirstPart.Extend(parts.Skip(1).Select(o => (FormattedString)o).ToArray(), p);
		//}

		#region Methods

		public IEnumerable<FormattedString> GetLevel(int level) => FirstPart.GetLevel(level);

		public override string ToString() => FirstPart.LevelString().ToString();

		#endregion Methods
	}
}