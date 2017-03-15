using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VScript_Core.Graph
{
	public class Library
	{
		//Singlton accessing
		private static Library library_singlton;
		public static Library main
		{
			get
			{
				if (library_singlton == null)
					library_singlton = new Library();
				return library_singlton;
            }
		}


		private Dictionary<int, Dictionary<int, Node>> module_nodes = new Dictionary<int, Dictionary<int, Node>>();
		private Node start_node;

		private Library()
		{
			start_node = new Node("Start");
			start_node.colour_r = 0.1f;
			start_node.colour_g = 0.1f;
			start_node.colour_b = 1.0f;

			start_node.source = "{eo:end}";

			IOType output = new IOType();
			output.name = "end";
            output.display_name = "";
			output.is_execution = true;

			start_node.outputs.Add(output);
		}

		public void LoadModules(string folder)
		{
			int start_index = folder.Length + 1;

			foreach (string file in Directory.GetFiles(folder))
			{
				if (file.EndsWith(Module.file_extention))
				{
					string module_name = file.Substring(start_index, file.Length - start_index - Module.file_extention.Length);
					LoadModule(folder, module_name);
                }
			}
		}


        public void LoadModule(string folder, string name)
		{
			Module module = new Module(folder + "/" + name, "Python3");
			module.Import();

			if (module_nodes.ContainsKey(module.id))
			{
				Logger.LogError("Cannot load module '" + name + "':" + module.id + " as module with same id already exists");
				return;
			}


			Dictionary<int, Node> nodes = new Dictionary<int, Node>();

			foreach (string node_name in module.nodes)
			{
				Node node = new Node(folder + "/" + node_name);
				node.Import();

				if (nodes.ContainsKey(node.id))
				{
					Logger.LogError("Cannot load node '" + node_name + "':" + node.id + " in module '" + name + "' as node with same id already exists");
					continue;
				}
				nodes.Add(node.id, node);
			}

			module_nodes.Add(module.id, nodes);
			Logger.Log("Loaded module '" + name + "':" + module.id + " with " + nodes.Count + " nodes");
        }

		public Node GetNode(int module_id, int node_id)
		{
			if (module_id == 0 && node_id == 0)
				return start_node;
			
			return module_nodes[module_id][node_id];
		}

	}
}
