using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VScript_Core;
using VScript_Core.Graph;
using VScript_Core.Graph.Json;

using VScript_Core.System;

namespace VScript_Testing
{
	class Program
	{
		static void Main(string[] args)
		{
			Logger.Log("Hello World");

			Library.main.LoadModules("../../Modules");
			//Node node = Library.main.GetNode(1, 2);
			//Logger.Log(node.ToString());

			Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
