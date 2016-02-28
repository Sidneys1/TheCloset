using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheCloset.TextAdventure {
	public class CommandPart {
		public enum CommandType {
			StaticList,
			Action
		}

		private readonly Func<IEnumerable<CommandPart>> _enumParts;
		private readonly List<CommandPart> _staticList;
		public readonly CommandType CType;

		public CommandPart(string thispart, params CommandPart[] next) {
			CType = CommandType.StaticList;
			if (next.Length > 0 && next[0] != null) {
				_staticList = new List<CommandPart>(next.Length);
				_staticList.AddRange(next);
			}
			else
				_staticList = new List<CommandPart>(0);
			ThisPart = thispart;
		}

		public CommandPart(string thispart, Func<IEnumerable<CommandPart>> enumerate) {
			CType = CommandType.Action;
			_enumParts = enumerate;
			ThisPart = thispart;
		}

		public IEnumerable<CommandPart> NextParts {
			get {
				switch (CType) {
					case CommandType.StaticList:
						foreach (var part in _staticList)
							yield return part;
						break;
					case CommandType.Action:
						foreach (var part in _enumParts())
							yield return part;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		public string ThisPart { get; }

		public void AddPart(CommandPart p) {
			_staticList.Add(p);
		}

		public void Extend(params string[] p) {
			if (NextParts.Any(o => o.ThisPart == p[0]))
				NextParts.First(o => o.ThisPart.Equals(p[0])).Extend(p.Skip(1).ToArray());
			else if (CType == CommandType.StaticList)
				_staticList.Add(FromArray(p));
		}

		public void Extend(string[] pre, CommandPart p) {
			if (pre.Length > 0 && NextParts.Any(o => o.ThisPart == pre[0]))
				NextParts.First(o => o.ThisPart.Equals(pre[0])).Extend(pre.Skip(1).ToArray(), p);
			else if (CType == CommandType.StaticList)
				_staticList.Add(p);
		}

		public override string ToString() {
			return LevelString();
		}

		public string LevelString(bool showlevel = true, bool complete = true) {
			var b = new StringBuilder();
			var count = NextParts.Count();
			b.Append(ThisPart);
			b.Append(' ');
			if (count == 1) {
				b.Append(NextParts.First().LevelString(showlevel));
				return b.ToString();
			}
			if (count != 0 && showlevel)
				b.AppendFormat("[ {0}] ", string.Join("| ", NextParts.Select(o => o.LevelString(complete))));
			return b.ToString();
		}

		public IEnumerable<string> LevelOpts() => NextParts.Select(commandPart => commandPart.LevelString(false));

		public IEnumerable<string> Permute() {
			if (!NextParts.Any())
				yield return ThisPart;
			else
				foreach (var commandPart in NextParts.SelectMany(o => o.Permute()))
					yield return $"{ThisPart} {commandPart}";
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((CommandPart) obj);
		}

		public static CommandPart FromArray(params string[] parts) {
			var next = parts.Length == 1 ? null : FromArray(parts.Skip(1).ToArray());
			return new CommandPart(parts[0], next);
		}

		public static CommandPart FromArray(Func<IEnumerable<CommandPart>> func, params string[] parts) {
			return parts.Length == 1
				? new CommandPart(parts[0], func)
				: new CommandPart(parts[0], FromArray(func, parts.Skip(1).ToArray()));
		}

		public int Depth() {
			return Depth(0);
		}

		private int Depth(int cdepth) {
			if (!NextParts.Any())
				return cdepth;
			return cdepth + NextParts.Max(o => o.Depth(cdepth + 1));
		}

		public IEnumerable<string> GetLevel(int level) {
			if (level == 0)
				yield return ThisPart;
			else
				foreach (var commandPart in NextParts.SelectMany(o => o.GetLevel(level - 1)))
					yield return commandPart;
		}

		#region Equality

		protected bool Equals(CommandPart other) {
			return CType == other.CType && Equals(_staticList, other._staticList) && Equals(_enumParts, other._enumParts) &&
			       string.Equals(ThisPart, other.ThisPart);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = (int) CType;
				hashCode = (hashCode*397) ^ (_staticList?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (_enumParts?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (ThisPart?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		#endregion
	}
}