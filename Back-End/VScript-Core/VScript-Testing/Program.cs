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

			Graph graph = new Graph("Test");
			graph.Import();
			//graph.root_node.forwards_conn.Add(new NodeMeta(3453, 23));
			//graph.root_node.forwards_conn.Add(new NodeMeta(3453, 33));
			//graph.root_node.backwards_conn.Add(new NodeMeta(3453, 13));
			//graph.Export();

			Logger.Log(graph.ToString());

			Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
