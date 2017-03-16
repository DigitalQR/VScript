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
            
            graph.Clear();
            graph.AddNode(0, 1);


            GraphNode const_msg = graph.AddNode(0, 2);
            const_msg.meta_data.Put("value", "\"Hard coded msg\"");

            GraphNode if_then = graph.AddNode(1, 2);
            GraphNode true_node = graph.AddNode(1, 5);
            GraphNode false_node = graph.AddNode(1, 6);
            GraphNode print_true = graph.AddNode(1, 3);
            GraphNode print_false = graph.AddNode(1, 3);
            GraphNode input_node = graph.AddNode(1, 4);
            GraphNode print_input = graph.AddNode(1, 3);
            GraphNode print_msg = graph.AddNode(1, 3);

            graph.AddConnection(true_node, "end", print_true, "message");
            graph.AddConnection(false_node, "end", print_false, "message");
            graph.AddConnection(input_node, "end", print_input, "message");
            graph.AddConnection(const_msg, "end", print_msg, "message");


            graph.AddConnection(graph.start_node, "end", if_then, "begin");
            graph.AddConnection(true_node, "end", if_then, "condition");

            graph.AddConnection(if_then, "true", print_true, "begin");
            graph.AddConnection(if_then, "false", print_false, "begin");
            graph.AddConnection(if_then, "end", print_input, "begin");
            graph.AddConnection(print_input, "end", print_msg, "begin");

            //graph.AddConnection(if_then, "true", print_true, "begin");
            //graph.AddConnection(if_then, "false", print_false, "begin");



            VScriptEngine.CompileAndRun(graph);
            VScriptEngine.SaveAll();

            Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
