using System;
using System.Collections.Generic;
using System.Linq;
using TheCloset.ConsoleHelpers;

namespace TheCloset.TextAdventure {

	public class CommandPart {
		#region Fields

		public readonly CommandType CType;

		private readonly Func<IEnumerable<CommandPart>> _enumParts;

		private readonly List<CommandPart> _staticList;

		#endregion Fields

		#region Properties

		public IEnumerable<CommandPart> NextParts
		{
			get
			{
				switch (CType) {
					case CommandType.StaticList:
						foreach (var part in _staticList)
							yield return part;
						break;

					case CommandType.Function:
						foreach (var part in _enumParts())
							yield return part;
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		public FormattedString ThisPart { get; }

		#endregion Properties

		#region Constructors

		public CommandPart(FormattedString thispart, params CommandPart[] next) {
			CType = CommandType.StaticList;
			if (next.Length > 0 && next[0] != null) {
				_staticList = new List<CommandPart>(next.Length);
				_staticList.AddRange(next);
			} else
				_staticList = new List<CommandPart>(0);
			ThisPart = thispart;
		}

		public CommandPart(FormattedString thispart, Func<IEnumerable<CommandPart>> enumerate) {
			CType = CommandType.Function;
			_enumParts = enumerate;
			ThisPart = thispart;
		}

		#endregion Constructors

		#region Methods

		public static CommandPart FromArray(params FormattedString[] parts) {
			var next = parts.Length == 1 ? null : FromArray(parts.Skip(1).ToArray());
			return new CommandPart(parts[0], next);
		}

		public static CommandPart FromArray(Func<IEnumerable<CommandPart>> func, params FormattedString[] parts) {
			return parts.Length == 1
				? new CommandPart(parts[0], func)
				: new CommandPart(parts[0], FromArray(func, parts.Skip(1).ToArray()));
		}

		public void AddPart(CommandPart p) {
			_staticList.Add(p);
		}

		public int Depth() {
			return Depth(0);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((CommandPart)obj);
		}

		public void Extend(params FormattedString[] p) {
			if (NextParts.Any(o => o.ThisPart == p[0]))
				NextParts.First(o => o.ThisPart.Equals(p[0])).Extend(p.Skip(1).ToArray());
			else if (CType == CommandType.StaticList)
				_staticList.Add(FromArray(p));
		}

		public void Extend(FormattedString[] pre, CommandPart p) {
			if (pre.Length > 0 && NextParts.Any(o => o.ThisPart == pre[0]))
				NextParts.First(o => o.ThisPart.Equals(pre[0])).Extend(pre.Skip(1).ToArray(), p);
			else if (CType == CommandType.StaticList)
				_staticList.Add(p);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = (int)CType;
				hashCode = (hashCode * 397) ^ (_staticList?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (_enumParts?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (ThisPart?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		public IEnumerable<FormattedString> GetLevel(int level) {
			if (level == 0)
				yield return ThisPart;
			else
				foreach (var commandPart in NextParts.SelectMany(o => o.GetLevel(level - 1)))
					yield return commandPart;
		}

		public IEnumerable<FormattedString> LevelOpts() => NextParts.Select(commandPart => commandPart.LevelString(false));

		public FormattedString LevelString(bool showlevel = true, bool complete = true) {
			var b = new FormattedString();
			var count = NextParts.Count();
			b.Append(ThisPart);
			if (count == 1) {
				b.Append(" ");
				b.Append(NextParts.First().LevelString(showlevel));
				return b;
			}
			if (count != 0 && showlevel)
				b.Append($"[ {FormattedString.Join("| ", NextParts.Select(o => o.LevelString(complete)))}] ");
			else if (count != 0)
				b.Append("...");
			return b;
		}

		public IEnumerable<FormattedString> Permute() {
			if (!NextParts.Any())
				yield return ThisPart;
			else
				foreach (var commandPart in NextParts.SelectMany(o => o.Permute()))
					yield return $"{ThisPart} {commandPart}";
		}

		public IEnumerable<CommandPart> PermuteCommands() {
			if (!NextParts.Any())
				yield return this;
			else
				foreach (var commandPart in NextParts.SelectMany(o => o.PermuteCommands()))
					yield return new CommandPart(ThisPart, commandPart);
		}

		public override string ToString() {
			return LevelString().ToString();
		}

		protected bool Equals(CommandPart other) {
			return CType == other.CType && Equals(_staticList, other._staticList) && Equals(_enumParts, other._enumParts) &&
				   string.Equals(ThisPart.ToString(), other.ThisPart.ToString());
		}

		private int Depth(int cdepth) {
			if (!NextParts.Any())
				return cdepth;
			return cdepth + NextParts.Max(o => o.Depth(cdepth + 1));
		}

		#endregion Methods

		#region Enums

		public enum CommandType {
			StaticList,
			Function
		}

		#endregion Enums
	}
}