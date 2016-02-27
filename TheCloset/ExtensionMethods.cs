using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCloset {
	public static class ExtensionMethods {
		public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source) {
			using (var e = source.GetEnumerator()) {
				if (!e.MoveNext()) yield break;
				for (var value = e.Current; e.MoveNext(); value = e.Current) {
					yield return value;
				}
			}
		}
	}
}
