using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VScript_Core.Graphing
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

		private Library()
		{
			LoadDefaults();
        }

		private void LoadDefaults()
		{
			Dictionary<int, Node> default_nodes = new Dictionary<int, Node>();

			//Null node
			{
				Node null_node = new Node("Null");
				null_node.source = "pass";
				default_nodes.Add(0, null_node);
			}

			//Start node
			{
				Node start_node = new Node("Start");
				start_node.id = 1;
				start_node.colour_r = 0.1f;
				start_node.colour_g = 0.1f;
				start_node.colour_b = 1.0f;

				start_node.source = "{eo:end}";

				NodeIO output = new NodeIO();
				output.name = "end";
				output.display_name = "";
				output.is_execution = true;

				start_node.outputs.Add(output);
				default_nodes.Add(1, start_node);
			}

			//True
			{
				Node const_node = new Node("True");
				const_node.id = 2;
				const_node.colour_r = 1.0f;
				const_node.colour_g = 0.1f;
				const_node.colour_b = 0.1f;

				const_node.source = "True{vo:end}";

				default_nodes.Add(2, const_node);
			}
			//False
			{
				Node const_node = new Node("False");
				const_node.id = 3;
				const_node.colour_r = 1.0f;
				const_node.colour_g = 0.1f;
				const_node.colour_b = 0.1f;

				const_node.source = "False{vo:end}";
				
				default_nodes.Add(3, const_node);
            }
            //Input
            {
                Node const_node = new Node("Input");
                const_node.id = 4;
                const_node.colour_r = 0.1f;
                const_node.colour_g = 1.0f;
                const_node.colour_b = 0.1f;

                const_node.source = "input(){vo:end}";

                default_nodes.Add(4, const_node);
            }

            module_nodes.Add(0, default_nodes);
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

		public Node GetNode(GraphNode node_meta)
		{
			//Do special stuff here using node_meta

			return GetNode(node_meta.module_id, node_meta.node_id);
        }

        private Node GetNode(int module_id, int node_id)
		{
			return module_nodes[module_id][node_id];
		}

	}
}
