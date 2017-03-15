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
			/*
			Graph graph = new Graph("Test");
			graph.Import();
			//graph.root_node.forwards_conn.Add(new NodeMeta(3453, 23));
			//graph.root_node.forwards_conn.Add(new NodeMeta(3453, 33));
			//graph.root_node.backwards_conn.Add(new NodeMeta(3453, 13));
			//graph.Export();

			Logger.Log(graph.ToString());

			Module module = new Module("Test");
			module.Export();
			Logger.Log(module.ToString());
			*/

			Node node = new Node("Example");
			//node.source = "Print(i)";
			//node.id = 69;

			IOType input_0 = new IOType();
			//input_0.name = "in 0";
			//input_0.is_execution = true;
			//node.inputs.Add(input_0);
			node.inputs.Add(input_0);
			node.Export();
			//node.Import();

			Logger.Log(node.ToString());

			Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
