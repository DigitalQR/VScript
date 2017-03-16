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
            /*
            GraphNode print_node = graph.AddNode(1, 2);
            GraphNode true_node = graph.AddNode(0, 2);
            
            graph.AddConnection(graph.start_node, "end", print_node, "begin");
            graph.AddConnection(true_node, "end", print_node, "message");

            graph.RemoveConnection(true_node, "endE", print_node, "message");
            */
            //graph.Export();
            graph.Import();


            string path = Compiler.main.Compile(graph);
            PythonProgram program = new PythonProgram(path);
            PythonProgram.python_path = "C:/Python35-32/python.exe";
            program.Run("", true);


            Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
