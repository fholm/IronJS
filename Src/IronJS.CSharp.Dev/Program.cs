using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.CSharp.Dev {
	class Program {
		static void Main(string[] args) {
			var test1 = new object[2];
			test1[0] = typeof(int);
			test1[1] = typeof(string);

			var test2 = new object[2];
			test2[0] = typeof(int);
			test2[1] = typeof(string);

			Console.WriteLine(test1[0].GetHashCode() ^ test1[1].GetHashCode());
			Console.WriteLine(test2[0].GetHashCode() ^ test2[1].GetHashCode());

			Console.ReadLine();
		}
	}
}
