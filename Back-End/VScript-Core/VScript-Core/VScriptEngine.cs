using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using VScript_Core.Graphing;
using VScript_Core.System;
using System.Diagnostics;

namespace VScript_Core
{
    public class VScriptEngine
    {
        public static bool initialised { get; private set; }
        public static string engine_directory = "";
        public static string python_exe_directory = "C:/Python34/python.exe";

        public static string core_modules_directory = "Modules";
        public static string project_directory = "Projects";
        public static string executable_directory = "Execute";

        private static Dictionary<string, Graph> loaded_graphs;

        public static void Init()
        {
            Directory.CreateDirectory(engine_directory + core_modules_directory);
            Library.main.LoadModules(engine_directory + core_modules_directory);

            PythonConsole.exe_path = python_exe_directory;
            Directory.CreateDirectory(engine_directory + project_directory);

            loaded_graphs = new Dictionary<string, Graph>();

            initialised = true;
        }

        public static Graph GetGraph(string name)
        {
            if (loaded_graphs.ContainsKey(name))
                return loaded_graphs[name];

            Graph graph = new Graph(name);

            if (graph.Import(engine_directory + project_directory + "/"))
                Logger.Log("Loaded graph '" + name + "'");
            else
                Logger.Log("New graph '" + name + "'");

            loaded_graphs.Add(name, graph);
            return graph;
        }

        public static void SaveAll()
        {
            foreach (KeyValuePair<string, Graph> g in loaded_graphs)
            {
                g.Value.Export(engine_directory + project_directory + "/");
                Logger.Log("Saved graph '" + g.Key + "'");
            }
        }

        public static void CompileAndRun(Graph graph)
        {
            Logger.Log("===Executing '" + graph.display_name + "'===");
            Process process = PythonConsole.CompileAndRun(graph);
            process.WaitForExit();
            Logger.Log("===Finished execution===");
        }
    }
}
