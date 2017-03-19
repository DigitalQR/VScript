using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VScript_Core;
using VScript_Core.Graphing;
using VScript_Core.Graphing.Json;

using VScript_Core.System;

namespace VScript_Testing
{
	class Program
	{
		static void Print(string message)
		{
			Console.WriteLine(message);
		}

		static void Main(string[] args)
		{
			VSLogger.std_print = Print;
			VSLogger.debug_print = Print;
			VSLogger.error_print = Print;


			VScriptEngine.input_function = delegate () { return Encoding.ASCII.GetBytes("Test"); };
            VScriptEngine.engine_directory = "../../VScript/";
			VScriptEngine.Init();
            Graph graph = VScriptEngine.OpenGraph("Example");
			/*       
            graph.Clear();
            graph.AddNode(0, 1);

            GraphNode const_msg = graph.AddNode(0, 2);
            const_msg.meta_data.Put("value", "\"Hard coded msg\"");
			
			GraphNode true_node = graph.AddNode(1, 5);
			GraphNode false_node = graph.AddNode(1, 6);
            GraphNode print_true = graph.AddNode(1, 3);
            GraphNode print_false = graph.AddNode(1, 3);
            GraphNode print_msg = graph.AddNode(1, 3);

            graph.AddConnection(true_node, "end", print_true, "message");
            graph.AddConnection(false_node, "end", print_false, "message");
            graph.AddConnection(const_msg, "end", print_msg, "message");


			graph.AddConnection(graph.start_node, "end", print_true, "begin");
			graph.AddConnection(print_true, "end", print_false, "begin");
			graph.AddConnection(print_false, "end", print_msg, "begin");
			*/
			//graph.AddConnection(if_then, "true", print_true, "begin");
			//graph.AddConnection(if_then, "false", print_false, "begin");


			VScriptEngine.SaveOpenGraphs();
			//VScriptEngine.CompileAndRun(graph);

			VSLogger.Log("Done");
			Console.ReadLine();
		}
	}
}
