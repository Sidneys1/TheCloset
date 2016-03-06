using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheCloset.ConsoleHelpers;
using TheCloset.Locations.InTheCloset;
using TheCloset.TextAdventure;

namespace TheCloset {

	internal class Game {

		#region Fields

		public readonly ScrollingPane OutputPane;
		private const int INV_WIDTH = 35;
		private readonly SwitchPane _optionsPane;
		private int _cusorLine;

		#endregion Fields


		#region Constructors

		public Game() {
			Console.SetWindowSize(200, 50);
			Console.SetBufferSize(200, 50);

			Instace = this;
			OutputPane = new ScrollingPane(0, 0, Console.BufferWidth - INV_WIDTH, Console.BufferHeight - 3);
			_optionsPane = new SwitchPane(0, 48, Console.BufferWidth);
		}

		#endregion Constructors


		#region Properties

		public static Game Instace { get; private set; }

		public Player Player { get; } = new Player {
			GlobalVars = {
				{"electricity", "false"},
				{"server", "false"}
			}
		};

		#endregion Properties


		#region Methods

		public static string ReadMyLine(Action<string> printStr, Func<string, string> tabcallback) {
			var buf = "";
			while (true) {
				var k = Console.ReadKey(true);
				switch (k.Key) {
					case ConsoleKey.Tab:
						if (buf.Length == Console.BufferWidth - 1) break;
						var ret = tabcallback(buf);
						if (ret == null) break;
						if (!ret.Equals(buf)) {
							ExtendedConsole.ClearCurrentConsoleLine();
							buf = ret;
							printStr(buf);
						}
						break;

					case ConsoleKey.Enter:
						return buf;

					case ConsoleKey.Escape:
						buf = string.Empty;
						ExtendedConsole.ClearCurrentConsoleLine();
						Console.SetCursorPosition(0, Console.CursorTop);
						break;

					case ConsoleKey.Backspace:
						if (buf.Length == 0) break;
						buf = buf.Substring(0, buf.Length - 1);
						Console.CursorLeft--;
						Console.Write(' ');
						Console.CursorLeft--;
						printStr(buf);
						break;

					default:
						if (buf.Length == Console.BufferWidth - 1) break;
						if (char.IsLetterOrDigit(k.KeyChar) || char.IsPunctuation(k.KeyChar) || char.IsWhiteSpace(k.KeyChar) ||
							char.IsSymbol(k.KeyChar)) {
							buf += k.KeyChar;
							printStr(buf);
						}
						break;
				}
			}
		}

		public string PrintLevel(string entry) {
			var b = new StringBuilder();
			var options = Player.Verbs.Where(o => o.Enabled && o.FirstPart.Permute().Any(x => x.StartsWith(entry, StringComparison.CurrentCultureIgnoreCase))).Select(o => o.FirstPart).ToArray();

			if (!options.Any())
				return null;

			var groups = options.SelectMany(o => o.Permute().Select(s => s.ToString())).GroupBy(g => g.Split(' ')[0]).ToArray();

			while (groups.Length == 1) {
				b.Append(groups[0].Key + " ");
				var addedLen = groups[0].Key.Length + 1;
				if (entry.Length > 0)
					entry = entry.Substring(entry.Length < addedLen ? entry.Length : addedLen);
				if (groups[0].Count() == 1) {
					b.Append(groups[0].First().Substring(groups[0].Key.Length + 1));
					ExtendedConsole.ClearConsoleLine(48);
					return b.ToString();
				}
				groups = groups[0].Select(o => o.Substring(groups[0].Key.Length + 1)).Where(o => o.StartsWith(entry, StringComparison.InvariantCultureIgnoreCase)).GroupBy(o => o.Split(' ')[0]).ToArray();
			}

			var suggest = new List<FormattedString>();

			foreach (var t in groups) {
				var group = t.Key;
				if (t.Any(o => o.Length == group.Length)) {
					group = group.Trim();
					if (t.Count() > 1)
						group += "...";
					suggest.Add(group);
					continue;
				}
				var nextChar = t.First()[group.Length];
				while (t.All(o => o.StartsWith(group + nextChar))) {
					group += nextChar;
					if (t.Any(o => o.Length == group.Length)) {
						nextChar = '\0';
						break;
					}
					nextChar = t.First()[group.Length];
				}
				group = group.Trim();
				if (nextChar != '\0')
					group += "...";
				suggest.Add(group);
			}

			if (suggest.Count <= 0) return b.ToString();

			_optionsPane.Draw("Options: ".DarkGray(), "...".DarkGray(), suggest);

			b.Append(entry);
			return b.ToString();
		}

		public void PrintStr(string buf) {
			Console.SetCursorPosition(0, Console.CursorTop);
			var pre = Console.ForegroundColor;
			var match = Player.Verbs.SelectMany(o => o.FirstPart.PermuteCommands()).FirstOrDefault(o => o.ToString().Equals(buf.Trim(), StringComparison.InvariantCultureIgnoreCase));
			
			if (match != null)
				match.LevelString().Write();
			else
				Console.Write(buf);
			Console.ForegroundColor = pre;
		}

		public void Run() {
			Player.ChangeLocation(new DarkPlace());

			PrintHeader();

			Console.ForegroundColor = ConsoleColor.DarkGray;

			PrintUserInterface();

			do {
				Console.SetCursorPosition(Console.BufferWidth - (INV_WIDTH - 2), 1);
				Console.ForegroundColor = ConsoleColor.Cyan;
				foreach (var item in Player.Items) {
					Console.Write(item.Name);
					Console.SetCursorPosition(Console.BufferWidth - (INV_WIDTH - 2), Console.CursorTop + 1);
				}

				OutputPane.Draw();
				ExtendedConsole.ClearConsoleLine(49);
				Console.SetCursorPosition(0, 49);
				var str = ReadMyLine(PrintStr, Tabcallback).Trim();

				var match = Player.CurrentLocation.Verbs.FirstOrDefault(
					o => o.FirstPart.Permute().Any(x => x.ToString().Equals(str, StringComparison.InvariantCultureIgnoreCase)))
							??
							Player.Verbs.FirstOrDefault(
								o => o.FirstPart.Permute().Any(x => x.ToString().Equals(str, StringComparison.InvariantCultureIgnoreCase)));

				if (match?.Action != null) {
					if (_cusorLine != Console.BufferHeight - 4)
						_cusorLine++;
					OutputPane.WriteLine();
					OutputPane.Write($"{str}> ".DarkGray());
					match.Action(str);
				} else {
					ExtendedConsole.ClearConsoleLine(48);
					"\rCould not find that command...".Red(true).Write();
				}
			} while (true);
		}

		private static void PrintUserInterface() {
			Console.SetCursorPosition(0, 47);
			Console.Write(new string('═', Console.BufferWidth - INV_WIDTH));
			Console.Write('╩');
			Console.Write(new string('═', INV_WIDTH - 1));
			for (var i = 0; i < Console.BufferHeight - 3; i++) {
				Console.SetCursorPosition(Console.BufferWidth - INV_WIDTH, i);
				Console.Write('║');
			}
			Console.SetCursorPosition(Console.BufferWidth - ((INV_WIDTH / 2) + 5), 0);
			"Inventory".White(true).Write();
		}

		private void PrintHeader() {
			var margin = new string(' ', (OutputPane.Width / 2) - 20);
			OutputPane.WriteLine((margin + @"   ____       _              ").Yellow());
			OutputPane.WriteLine((margin + @"    /  / _   /  /   _ _ _/   ").Yellow());
			OutputPane.WriteLine((margin + @"   /  /)(-  (_ /()_) (- /    ").Yellow());
			OutputPane.WriteLine((margin + "You wake up. Your head hurts..").White(true));
			OutputPane.WriteLine(new FormattedString((margin + "  You decide to press ").White(true), "[TAB]".DarkGray(true).DarkGrayBack()));
			OutputPane.WriteLine();
		}

		private string Tabcallback(string s) {
			var x = Console.CursorLeft;
			var y = Console.CursorTop;

			ExtendedConsole.ClearConsoleLine(48);
			Console.SetCursorPosition(0, 48);
			var ret = PrintLevel(s);

			Console.SetCursorPosition(x, y);
			return ret;
		}

		#endregion Methods
	}
}