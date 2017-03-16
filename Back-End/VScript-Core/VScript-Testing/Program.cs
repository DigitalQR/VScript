using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VScript_Core;
using VScript_Core.Graphing;
using VScript_Core.Graphing.Json;

using VScript_Core.System;

namespace VScript_Testing
{
	class Program
	{
		static void Main(string[] args)
		{
			Logger.Log("Hello World");

            VScriptEngine.engine_directory = "../../";
            VScriptEngine.python_exe_directory = "C:/Python35-32/python.exe";
            VScriptEngine.Init();

            Graph graph = VScriptEngine.GetGraph("Example");
            /*
            graph.Clear();
            graph.AddNode(0, 1);

            GraphNode if_node = graph.AddNode(1, 1);
            GraphNode print_node = graph.AddNode(1, 2);
            GraphNode true_node = graph.AddNode(0, 2);

            GraphNode print2_node = graph.AddNode(1, 2);
            GraphNode input_node = graph.AddNode(0, 4);

            graph.AddConnection(graph.start_node, "end", if_node, "begin");

            graph.AddConnection(true_node, "end", if_node, "condition");
            graph.AddConnection(input_node, "end", print_node, "message");

            graph.AddConnection(input_node, "end", print2_node, "message");
            graph.AddConnection(print_node, "end", print2_node, "begin");

            graph.AddConnection(if_node, "true", print_node, "begin");
            graph.AddConnection(if_node, "false", print_node, "begin");
            */

            VScriptEngine.CompileAndRun(graph);
            VScriptEngine.SaveAll();

            Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
