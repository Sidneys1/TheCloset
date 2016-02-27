using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheCloset.TextAdventure;

namespace TheCloset {
	class Program {
		static void Main() {
			Game g = new Game();
			g.Run();
		}
	}

	class Game {
		const int INV_WIDTH = 35;

		private static readonly char[] Whitespace = { ' ' };
		private List<string> _availcmds = new List<string>();
		public Player Player { get; } = new Player();

		public void Run() {
			Player.CurrentLocation = new Locations.DarkPlace(Player);
			Player.Start();

			Console.BufferWidth = Console.WindowWidth = 200;
			Console.BufferHeight = Console.WindowHeight = 50;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(@"               ___           _");
			Console.WriteLine(@"                | |_   _    /  |  _   _  _ _|_");
			Console.WriteLine(@"                | | | (/_   \_ | (_) _> (/_ |_");
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine("                You wake up. Your head hurts..");
			Console.WriteLine("                  You decide to press [TAB]");
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.SetCursorPosition(0, 47);
			Console.Write(new string('-', Console.BufferWidth- INV_WIDTH));
			for (int i = 0; i < Console.BufferHeight-3; i++) {
				Console.SetCursorPosition(Console.BufferWidth- INV_WIDTH, i);
				Console.Write('|');
			}
			Console.SetCursorPosition(Console.BufferWidth- ((INV_WIDTH/2)+5), 0);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("Inventory");
			
			var pos = 6;

			do {
				_availcmds.Clear();
				_availcmds.AddRange(Player.Verbs.Where(o=>o.Enabled).SelectMany(o => o.FirstPart.Permute()));
				_availcmds.AddRange(Player.CurrentLocation.Verbs.Where(o=>o.Enabled).SelectMany(o => o.FirstPart.Permute()));

				Console.SetCursorPosition(Console.BufferWidth - (INV_WIDTH-2), 1);
				var pre = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Cyan;
				foreach (var item in Player.Items) {
					Console.Write(item.Name);
					Console.SetCursorPosition(Console.BufferWidth - (INV_WIDTH - 2), Console.CursorTop+1);
				}
				Console.ForegroundColor = pre;
				Console.SetCursorPosition(0, 49);
				var str = ReadMyLine(PrintStr, Tabcallback).Trim();
				
				var match = Player.CurrentLocation.Verbs.FirstOrDefault(o => o.FirstPart.Permute().Any(x=>x.Equals(str, StringComparison.InvariantCultureIgnoreCase)))
					?? Player.Verbs.FirstOrDefault(o => o.FirstPart.Permute().Any(x => x.Equals(str, StringComparison.InvariantCultureIgnoreCase)));
				pre = Console.ForegroundColor;
				if (match?.Action != null) {
					if (pos != Console.BufferHeight - 4)
						pos++;
					Console.SetCursorPosition(0, pos);
					Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.Write(str);
					Console.Write(' ');
					Console.ForegroundColor = pre;
					
					match.Action(str);
					if(pos == Console.BufferHeight - 4)
						Console.MoveBufferArea(0, 1, Console.BufferWidth - INV_WIDTH, Console.BufferHeight - 4, 0, 0);
				}
				else {
					ClearConsoleLine(48);
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.Write("\rCould not find that command...");
				}
				Console.ForegroundColor = pre;
				ClearConsoleLine(49);
				Console.SetCursorPosition(0, 49);
			} while (true);
		}

		private string Tabcallback(string s) {
			var x = Console.CursorLeft;
			var y = Console.CursorTop;

			ClearConsoleLine(48);
			Console.SetCursorPosition(0, 48);
			var words = s.Split(Whitespace, StringSplitOptions.RemoveEmptyEntries);
			var ret = PrintLevel(words);
			Console.SetCursorPosition(x, y);
			return ret;
		}

		public string PrintLevel(string[] bits) {
			var b = new StringBuilder();
			var options = Player.CurrentLocation.Verbs.Where(o=>o.Enabled).Where(o => o.Depth >= bits.Length-1).Select(o=>o.FirstPart).ToList();
			options.AddRange(Player.Verbs.Where(o => o.Enabled).Where(o => o.Depth >= bits.Length - 1).Select(o => o.FirstPart));
			
			if (options.Count == 0)
				return string.Join(" ", bits);

			foreach (var bit in bits) {
				var matches = options.Where(o => o.ThisPart.StartsWith(bit, StringComparison.CurrentCultureIgnoreCase)).ToArray();
				if (matches.Length > 0 && matches.GroupBy(o=>o.ThisPart, StringComparer.InvariantCultureIgnoreCase).Count()==1) {
					b.Append(matches.First().ThisPart);
					b.Append(' ');
					options = matches.SelectMany(o=>o.NextParts).ToList();
				} else if (matches.Length > 0) {
					options = matches.ToList();
					break;
				}
				else {
					Console.Write($"No matches start with '{bit}'");
					return null;
				}
			}

			if (options.Count == 0)
				return b.ToString();

			while (options.Any() && options.All(o=>o.ThisPart.Equals(options[0].ThisPart, StringComparison.InvariantCultureIgnoreCase))) {
				b.Append(options[0].ThisPart);
				b.Append(' ');
				options = options.SelectMany(o => o.NextParts).ToList();
			}

			var suggest = new List<string>();
			var groups = options.GroupBy(o => o.ThisPart.ToLower(), StringComparer.InvariantCultureIgnoreCase);
			foreach (var group in groups) {
				var gb = new StringBuilder();
				gb.Append(group.First().ThisPart);
				var gar = group.SelectMany(o=>o.NextParts).GroupBy(o=>o.ThisPart, StringComparer.InvariantCultureIgnoreCase).ToArray();
				while(gar.Length == 1) {
					gb.Append(' ');
					gb.Append(gar.First().First().ThisPart);
					gar = gar.First().SelectMany(o=>o.NextParts).GroupBy(o => o.ThisPart, StringComparer.InvariantCultureIgnoreCase).ToArray();
				}
				if (gar.Length > 1)
					gb.Append("...");
				suggest.Add(gb.ToString());
			}
			
			if (suggest.Count > 0) {
				Console.Write("Options: ");
				PrintList(suggest);
			}
			return b.ToString();
		}

		public static void PrintList(IEnumerable<string> list) {
			foreach (var opt in list) {
				var pref = Console.ForegroundColor;
				var preg = Console.BackgroundColor;
				Console.BackgroundColor = ConsoleColor.DarkGray;
				Console.ForegroundColor = ConsoleColor.Black;
				Console.Write(opt);
				Console.BackgroundColor = preg;
				Console.ForegroundColor = pref;
				Console.Write(' ');
			}

		}

		public static void ClearCurrentConsoleLine() {
			ClearConsoleLine(Console.CursorTop);
		}

		public static void ClearConsoleLine(int line) {
			Console.SetCursorPosition(0, line);
			Console.Write(new string(' ', Console.WindowWidth - 1));
		}

		public void PrintStr(string buf) {
			Console.SetCursorPosition(0, Console.CursorTop);
			var pre = Console.ForegroundColor;
			if (_availcmds.Contains(buf.Trim(), StringComparer.InvariantCultureIgnoreCase))
				Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(buf);
			Console.ForegroundColor = pre;
		}

		public static string ReadMyLine(Action<string> print, Func<string, string> tabcallback) {
			var buf = "";
			while (true) {
				var k = Console.ReadKey(true);
				switch (k.Key) {
					case ConsoleKey.Tab:
						if (buf.Length == Console.BufferWidth - 1) break;
						var ret = tabcallback(buf);
						if (ret == null) break;
						if (!ret.Equals(buf)) {
							ClearCurrentConsoleLine();
							buf = ret;
							print(buf);
						}
						break;
					case ConsoleKey.Enter:
						return buf;
					case ConsoleKey.Escape:
						buf = string.Empty;
						ClearCurrentConsoleLine();
						Console.SetCursorPosition(0, Console.CursorTop);
						break;
					case ConsoleKey.Backspace:
						if (buf.Length == 0) break;
						buf = buf.Substring(0, buf.Length - 1);
						Console.CursorLeft--;
						Console.Write(' ');
						Console.CursorLeft--;
						print(buf);
						break;

					default:
						if (buf.Length == Console.BufferWidth - 1) break;
						if (char.IsLetterOrDigit(k.KeyChar) || char.IsPunctuation(k.KeyChar) || char.IsWhiteSpace(k.KeyChar) || char.IsSymbol(k.KeyChar)) {
							buf += k.KeyChar;
							print(buf);
						}
						break;
				}
			}

		}
	}
}
