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


			Graph graph = new Graph("Test");

			GraphNode true_node = new GraphNode(0, 2);
			GraphNode false_node = new GraphNode(0, 3);

			GraphNode if_node = new GraphNode(1, 1);
			GraphNode if_node2 = new GraphNode(1, 1);

			GraphNode print_true = new GraphNode(1, 2);
			GraphNode print_false = new GraphNode(1, 2);

			print_true.inputs.Add("message", true_node);
			print_false.inputs.Add("message", false_node);

			if_node.inputs.Add("condition", true_node);
			if_node.outputs.Add("true", print_true);
			if_node.outputs.Add("false", print_false);
			
			graph.root_node.outputs.Add("end", if_node);

			graph.Export();

			Compiler.main.Compile(graph);

			Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
