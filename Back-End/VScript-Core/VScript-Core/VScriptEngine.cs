using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using VScript_Core.Graphing;
using VScript_Core.Graphing.Json;
using VScript_Core.System;
using System.Diagnostics;
using System.ComponentModel;

namespace VScript_Core
{
    public class VScriptEngine
    {
        public static bool initialised { get; private set; }
		private static string engine_settings_path = "vscript-engine.json";
		public static string engine_directory = "";
        public static string python_exe_directory = "C:/Python34/python.exe";

        public static string core_modules_directory = "Modules";
        public static string project_directory = "Projects";
        public static string executable_directory = "Execute";

        private static Dictionary<string, Graph> loaded_graphs;

        public static void Init()
		{
			LoadSettings();
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
			SaveSettings();

			foreach (KeyValuePair<string, Graph> g in loaded_graphs)
            {
                g.Value.Export(engine_directory + project_directory + "/");
                Logger.Log("Saved graph '" + g.Key + "'");
            }
        }

        public static bool CompileAndRun(Graph graph)
        {
			try
			{
				Logger.Log("===Executing '" + graph.display_name + "'===");
				PythonConsole.ReadInput input_function =
					delegate ()
					{
						return "test input string";
					};

				Process process = PythonConsole.CompileAndRun(graph, input_function);
				process.WaitForExit();
				Logger.Log("===Finished execution===");
				return true;
			}
			catch (Exception e)
			{
				Logger.LogError(e.Message + "\n" + e.StackTrace);
				Logger.Log("===Aborting execution===");
				return false;
			}
        }

		public static void SaveSettings()
		{
			JsonObject json = new JsonObject();

			json.Put("python_exe_directory", python_exe_directory);
			json.Put("core_modules_directory", core_modules_directory);
			json.Put("project_directory", project_directory);
			json.Put("executable_directory", executable_directory);

			File.WriteAllText(engine_directory + engine_settings_path, json.ToString());
			Logger.Log("Saved '" + engine_settings_path + "'");
		}

		public static void LoadSettings()
		{
			try
			{
				string raw_json = File.ReadAllText(engine_directory + engine_settings_path);
				JsonObject json = new JsonObject(raw_json);

				python_exe_directory = json.Get("python_exe_directory", python_exe_directory);
				core_modules_directory = json.Get("core_modules_directory", core_modules_directory);
				project_directory = json.Get("project_directory", project_directory);
				executable_directory = json.Get("executable_directory", executable_directory);

				Logger.Log("Loaded '" + engine_settings_path + "'");
			}
			catch (FileNotFoundException) { }
			catch (DirectoryNotFoundException) { }
		}
	}
}
