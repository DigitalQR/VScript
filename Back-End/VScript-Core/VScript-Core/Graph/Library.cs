using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public void LoadModule(string folder, string name)
		{
			Module module = new Module(folder + "/" + name, "Python3");
			module.Import();

			Dictionary<int, Node> nodes = new Dictionary<int, Node>();

			foreach (string node_name in module.nodes)
			{
				Node node = new Node(folder + "/" + node_name);
				node.Import();

				nodes.Add(node.id, node);
			}

			module_nodes.Add(module.id, nodes);
        }

		public Node GetNode(int module_id, int node_id)
		{
			if (module_id == 0 && node_id == 0)
				return start_node;
			
			return module_nodes[module_id][node_id];
		}

	}
}
