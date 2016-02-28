using System;

namespace TheCloset.ConsoleHelpers {
	internal struct FormattedText {
		public ConsoleColor? BackgroundColor;
		public ConsoleColor? ForegroundColor;
		public bool Reset;
		public string Text;

		public FormattedText(string text, ConsoleColor? fcolor = null, ConsoleColor? bcolor = null, bool reset = false) {
			Text = text;
			ForegroundColor = fcolor;
			BackgroundColor = bcolor;
			Reset = reset;
		}

		public void SetColors() {
			if (Reset)
				Console.ResetColor();
			else {
				if (ForegroundColor.HasValue)
					Console.ForegroundColor = ForegroundColor.Value;
				if (BackgroundColor.HasValue)
					Console.BackgroundColor = BackgroundColor.Value;
			}
		}

		public static implicit operator FormattedText(string s) => new FormattedText(s, reset: true);
		public static explicit operator string(FormattedText s) => s.Text;

		public override string ToString() {
			return $"{(Reset ? "R" : ($"{ForegroundColor?.ToString() ?? "X"}/{BackgroundColor?.ToString() ?? "X"}"))}\"{Text}\"";
		}
	}
}