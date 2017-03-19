using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public static PythonConsole.ReadInput input_function;

		public static void Init()
		{
			initialised = true;
			VSLogger.Log("Initialising VScriptEngine");

			if (engine_directory == "")
			{
				VSLogger.LogError("VScriptEngine 'engine_directory' not set");
				initialised = false;
				return;
			}

			LoadSettings();
			Directory.CreateDirectory(engine_directory + core_modules_directory);
            Library.main.LoadModules(engine_directory + core_modules_directory);

            PythonConsole.exe_path = python_exe_directory;


            Directory.CreateDirectory(engine_directory + project_directory);

            loaded_graphs = new Dictionary<string, Graph>();
			VSLogger.Log("VScriptEngine initialised");
		}

		public static Graph NewGraph(string name)
		{
			if (!initialised)
			{
				VSLogger.LogError("VScriptEngine not initialised");
				return null;
			}
			
			Graph graph = new Graph(name);
			VSLogger.Log("New graph '" + name + "'");

			loaded_graphs[name] = graph;
			return graph;
		}

		public static Graph OpenGraph(string name)
		{
			if (!initialised)
			{
				VSLogger.LogError("VScriptEngine not initialised");
				return null;
			}

			if (loaded_graphs.ContainsKey(name))
				return loaded_graphs[name];

			Graph graph = new Graph(name);

			if (graph.Import(engine_directory + project_directory + "/"))
			{
				VSLogger.Log("Loaded graph '" + name + "'");
				return graph;
			}
			else
			{
				VSLogger.LogError("Failed to open graph '" + name + "'");
				return null;
			}
		}

		public static void CloseGraph(string name, bool save = true)
		{
			if (!initialised)
			{
				VSLogger.LogError("VScriptEngine not initialised");
				return;
			}

			if (loaded_graphs.ContainsKey(name))
			{
                if (save)
				{
					loaded_graphs[name].Export(engine_directory + project_directory + "/");
					VSLogger.Log("Saved graph '" + name + "'");
				}

				loaded_graphs.Remove(name);
				VSLogger.Log("Closed graph '" + name + "'");
			}
			else
			{
				VSLogger.LogError("Cannot close graph '" + name + "' as it is not open");
			}
		}
		
		public static void SaveOpenGraphs()
		{
			if (!initialised)
			{
				VSLogger.LogError("VScriptEngine not initialised");
				return;
			}
			
			foreach (KeyValuePair<string, Graph> g in loaded_graphs)
            {
                g.Value.Export(engine_directory + project_directory + "/");
                VSLogger.Log("Saved graph '" + g.Key + "'");
            }
        }

        public static bool CompileAndRun(Graph graph)
		{
			if (!initialised)
			{
				VSLogger.LogError("VScriptEngine not initialised");
				return false;
			}

			try
			{
				VSLogger.Log("===Executing '" + graph.display_name + "'===");
				Process process = PythonConsole.CompileAndRun(graph, input_function);
				process.WaitForExit();
				VSLogger.Log("===Finished execution===");
				return true;
			}
			catch (Exception e)
			{
				VSLogger.LogError(e.Message + "\n" + e.StackTrace);
				VSLogger.Log("===Aborting execution===");
				return false;
			}
        }

		public static void SaveSettings()
		{
			if (!initialised)
			{
				VSLogger.LogError("VScriptEngine not initialised");
				return;
			}

			JsonObject json = new JsonObject();

			json.Put("python_exe_directory", python_exe_directory);
			json.Put("core_modules_directory", core_modules_directory);
			json.Put("project_directory", project_directory);
			json.Put("executable_directory", executable_directory);

			File.WriteAllText(engine_directory + engine_settings_path, json.ToString());
			VSLogger.Log("Saved '" + engine_settings_path + "'");
		}

		public static void LoadSettings()
		{
			if (!initialised)
			{
				VSLogger.LogError("VScriptEngine not initialised");
				return;
			}

			try
			{
				string raw_json = File.ReadAllText(engine_directory + engine_settings_path);
				JsonObject json = new JsonObject(raw_json);

				python_exe_directory = json.Get("python_exe_directory", python_exe_directory);
				core_modules_directory = json.Get("core_modules_directory", core_modules_directory);
				project_directory = json.Get("project_directory", project_directory);
				executable_directory = json.Get("executable_directory", executable_directory);

				VSLogger.Log("Loaded '" + engine_settings_path + "'");
			}
			catch (FileNotFoundException) { }
			catch (DirectoryNotFoundException) { }
		}
	}
}
