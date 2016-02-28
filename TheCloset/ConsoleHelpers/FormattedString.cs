using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheCloset.ConsoleHelpers {
	internal class FormattedString {
		public FormattedString(params FormattedText[] sections) {
			Sections.AddRange(sections);
		}

		public List<FormattedText> Sections { get; } = new List<FormattedText>();
		public int Length => Sections.Sum(o => o.Text.Length);

		public override string ToString() {
			return
				Sections.Aggregate(new StringBuilder(), (builder, text) => builder.AppendFormat("<{0}>", text.ToString()))
					.ToString();
		}

		public IEnumerable<string> GetToPrint() {
			foreach (var section in Sections) {
				section.SetColors();
				yield return section.Text;
			}
		}

		public void Append(FormattedString str) {
			Sections.AddRange(str.Sections);
		}
	}
}