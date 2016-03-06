using System;
using System.Text;
using System.Threading;
using TheCloset.ConsoleHelpers;

namespace TheCloset {

	internal class Program {


		#region Methods

		private static void Main() {
			//ScrollingPaneDemo();
			new Game().Run();
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