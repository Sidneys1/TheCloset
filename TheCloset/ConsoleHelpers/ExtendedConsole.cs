using System;

namespace TheCloset.ConsoleHelpers {

	public static class ExtendedConsole {


		#region Methods

		public static void ClearConsoleLine(int line) {
			Console.SetCursorPosition(0, line);
			Console.Write(new string(' ', Console.WindowWidth - 1));
		}

		public static void ClearCurrentConsoleLine() {
			ClearConsoleLine(Console.CursorTop);
		}

		#endregion Methods
	}
}