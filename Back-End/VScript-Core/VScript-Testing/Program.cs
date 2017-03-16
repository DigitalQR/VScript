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
            GraphNode print_node = graph.AddNode(1, 2);
            GraphNode true_node = graph.AddNode(0, 2);

            graph.AddConnection(graph.start_node, "end", print_node, "begin");
            graph.AddConnection(true_node, "end", print_node, "message");

            //graph.RemoveConnection(true_node, "endE", print_node, "message");

            VScriptEngine.CompileAndRun(graph);
            VScriptEngine.SaveAll();

            Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
