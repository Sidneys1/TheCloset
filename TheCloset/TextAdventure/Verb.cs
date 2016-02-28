using System;
using System.Collections.Generic;
using System.Linq;

namespace TheCloset.TextAdventure {
	public class Verb {
		public Action<string> Action;
		public CommandPart FirstPart;

		public Verb(string parts, Action<string> action = null) {
			FirstPart = CommandPart.FromArray(parts.Split(' '));
			Action = action;
		}

		public Verb(string parts, Func<IEnumerable<CommandPart>> enumerate, Action<string> action = null) {
			FirstPart = CommandPart.FromArray(enumerate, parts.Split(' '));
			Action = action;
		}

		public int Depth => FirstPart.Depth();
		public bool Enabled { get; set; } = true;

		public void Extend(string ext) {
			var parts = ext.Split(' ');
			if (parts[0] == FirstPart.ThisPart)
				FirstPart.Extend(parts.Skip(1).ToArray());
		}

		public void Extend(string pre, CommandPart p) {
			var parts = pre.Split(' ');
			if (parts[0] == FirstPart.ThisPart)
				FirstPart.Extend(parts.Skip(1).ToArray(), p);
		}

		public override string ToString() => FirstPart.LevelString();
		public IEnumerable<string> GetLevel(int level) => FirstPart.GetLevel(level);
	}
}