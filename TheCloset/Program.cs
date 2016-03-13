using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TheCloset.ConsoleHelpers;

namespace TheCloset {

	internal class Program {
		#region Methods

		private static void Main() {
			Console.Title = "The Closet";

			Console.SetWindowSize(100, 25);
			Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

			ExtendedConsole.Init();
			ExtendedConsole.FixConsoleSize();


			//ExtendedConsoleMouseDemo();
			//ExtendedConsoleDemo();
			//ScrollingPaneDemo();
			new Game().Run();
			Console.ReadLine();
		}

		private static void ExtendedConsoleMouseDemo() {
			Console.CursorVisible = false;
			var buf = new ExtendedConsole.INPUT_RECORD[1];
			int lx = 0, ly = 0;
			var myChar = ' ';
			while (true) {
				ExtendedConsole.GetConsoleInput(ref buf);
				if ((buf[0].EventType & 0x0002) == 0x0002) {
					if ((buf[0].MouseEvent.dwEventFlags & 0x0001) == 0x0001) {
						lx = buf[0].MouseEvent.dwMousePosition.X;
						ly = buf[0].MouseEvent.dwMousePosition.Y;
					}
					if ((buf[0].MouseEvent.dwButtonState & 0x0001) == 0x0001) {
						Console.SetCursorPosition(lx, ly);
						Console.WriteLine(myChar);
					}
					Console.SetCursorPosition(0, 0);
					Console.ForegroundColor = (buf[0].MouseEvent.dwButtonState & 0x0001) == 0x0001
						? ConsoleColor.White
						: ConsoleColor.DarkGray;
					Console.WriteLine("Left");
					Console.ForegroundColor = (buf[0].MouseEvent.dwButtonState & 0x0004) == 0x0004
						? ConsoleColor.White
						: ConsoleColor.DarkGray;
					Console.WriteLine("Middle");
					Console.ForegroundColor = (buf[0].MouseEvent.dwButtonState & 0x0002) == 0x0002
						? ConsoleColor.White
						: ConsoleColor.DarkGray;
					Console.WriteLine("Right");
					Console.ForegroundColor = (buf[0].MouseEvent.dwEventFlags & 0x0002) == 0x0002
						? ConsoleColor.White
						: ConsoleColor.DarkGray;
					Console.WriteLine("Double click!");
					Console.ForegroundColor = (buf[0].MouseEvent.dwEventFlags & 0x0001) == 0x0001
						? ConsoleColor.White
						: ConsoleColor.DarkGray;
					Console.WriteLine("Mouse moved");
					Console.ForegroundColor = ConsoleColor.DarkGray;
					if ((buf[0].MouseEvent.dwEventFlags & 0x0004) == 0x0004) {
						var s = (short) (buf[0].MouseEvent.dwButtonState >> 16);
						Console.WriteLine(s > 0 ? "Mousewheeled up  " : "Mousewheeled down");
						for (var i = 0; i < Math.Abs(s/120); i++) {
							var x = (s > 0) ? myChar++ : myChar--;
						}
						if (myChar < ' ')
							myChar = ' ';
						else if (myChar > '~')
							myChar = '~';
					}
					else
						Console.WriteLine();
					Console.WriteLine($"Char: '{myChar}'");
				}
				if ((buf[0].EventType & 0x0001) != 0x0001) continue;
				Console.Clear();
				Console.SetCursorPosition(0, 0);
				Console.Write(buf[0].KeyEvent.UnicodeChar);
			}
		}

		private static void ExtendedConsoleDemo() {
			Console.CursorVisible = false;
			short w = (short)Console.BufferWidth;
			short h = (short)Console.BufferHeight;

			var buf = new ExtendedConsole.CharInfo[w * h];

			var rect = new ExtendedConsole.SmallRect { Left = 0, Top = 0, Right = w, Bottom = h };

			var p = ExtendedConsole.COORD.Zero;
			var s = new ExtendedConsole.COORD(w, h);
			var chars = Encoding.GetEncoding(437).GetChars(Enumerable.Range(0, byte.MaxValue).Select(i => (byte)i).Except(new byte[] { 8, 9, 10, 7 }).ToArray());

			for (int x = 0; x < 10000; x++) {
				for (int i = 0; i < buf.Length; ++i) {
					buf[i].Attributes = (short)((x + i) % 0x80);
					buf[i].Char.AsciiChar = (byte)chars[(x + i) % chars.Length];
				}

				ExtendedConsole.UpdateRegion(buf, p, s, ref rect);
			}

		}

		private static void ScrollingPaneDemo() {
			var c = new ScrollingPane(10, 5, 40, 20);

			{
				Console.SetCursorPosition(9, 4);
				Console.Write('╔');
				Console.Write(new string('═', 40));
				Console.Write('╗');

				for (int i = 0; i < 20; i++) {
					Console.SetCursorPosition(9, 5 + i);
					Console.Write('║');
					Console.SetCursorPosition(50, 5 + i);
					Console.Write('║');
				}

				Console.SetCursorPosition(9, 25);
				Console.Write('╚');
				Console.Write(new string('═', 40));
				Console.Write('╝');
			}
			Console.CursorVisible = false;
			Console.ReadKey(true);

			Random r = new Random();
			var names = Enum.GetNames(typeof(ConsoleColor));
			for (int i = 0; i < 50; i++) {
				if (i != 0)
					c.WriteLine();
				for (int j = 0; j < i + 1; j++) {
					c.Write("Text! ", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), names[r.Next() % (names.Length - 1)]),
						(ConsoleColor)Enum.Parse(typeof(ConsoleColor), names[r.Next() % (names.Length - 1)]));
					//c.Write("Text! ");
					c.Draw();
					Thread.Sleep(25);
				}
			}

			//c.Draw();

			Console.ReadLine();
		}

		#endregion Methods
	}
}