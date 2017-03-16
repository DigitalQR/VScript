using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using VScript_Core.Graph.Json;

namespace VScript_Core.Graph
{
	public struct GraphNode
	{
		public int module_id;
		public int node_id;
		public Dictionary<string, GraphNode> inputs;
		public Dictionary<string, GraphNode> outputs;
		public JsonObject meta_data;

		public GraphNode(int module_id, int node_id)
		{
			this.module_id = module_id;
			this.node_id = node_id;
			inputs = new Dictionary<string, GraphNode>();
			outputs = new Dictionary<string, GraphNode>();
			meta_data = new JsonObject();
        }

		public GraphNode GetInput(string name)
		{
			if (inputs.ContainsKey(name))
				return inputs[name];
			else
				return new GraphNode(0, 0);
		}

		public GraphNode GetOutput(string name)
		{
			if (outputs.ContainsKey(name))
				return outputs[name];
			else
				return new GraphNode(0, 0);
		}

		public void ParseFromJson(JsonObject json)
		{
			module_id = json.Get<int>("module_id");
			node_id = json.Get<int>("node_id");
			meta_data = json.GetObject("meta_data");

			foreach (JsonObject sub_json in json.GetObjectList("inputs"))
			{
				string name = sub_json.Get<string>("name");
				JsonObject obj = sub_json.GetObject("object");

				GraphNode sub_meta = new GraphNode();
				sub_meta.ParseFromJson(obj);
				inputs.Add(name, sub_meta);
			}

			foreach (JsonObject sub_json in json.GetObjectList("outputs"))
			{
				string name = sub_json.Get<string>("name");
				JsonObject obj = sub_json.GetObject("object");

				GraphNode sub_meta = new GraphNode();
				sub_meta.ParseFromJson(obj);
				outputs.Add(name, sub_meta);
			}
        }

		public JsonObject GetAsJson()
		{

			JsonObject json = new JsonObject();
			json.Put("module_id", module_id);
			json.Put("node_id", node_id);
			json.PutRaw("meta_data", meta_data);

			JsonArray input_json = new JsonArray();
			JsonArray output_json = new JsonArray();

			foreach (KeyValuePair<string, GraphNode> connection in inputs)
			{
				JsonObject con_json = new JsonObject();
				con_json.Put("name", connection.Key);
				con_json.Put("object", connection.Value.GetAsJson());
				input_json.Add(con_json);
			}

			foreach (KeyValuePair<string, GraphNode> connection in outputs)
			{
				JsonObject con_json = new JsonObject();
				con_json.Put("name", connection.Key);
				con_json.Put("object", connection.Value.GetAsJson());
				output_json.Add(con_json);
			}
			
			json.PutRaw("inputs", input_json);
			json.PutRaw("outputs", output_json);
			
			return json;
        }

		public override string ToString()
		{
			return GetAsJson().ToString();
		}
	}

	public class Graph
	{
		private static string extention = ".graph.json";
		public static string file_extention { get { return extention; } }

		public string name { get; private set; }
		public GraphNode root_node { get; private set; }

		public Graph(string name)
		{
			this.name = name;
			root_node = new GraphNode(0, 1);//Null/Start node
        }

		public void Export()
		{
			File.WriteAllText(name + extention, root_node.ToString());
		}

		public void Import()
		{
			try
			{
				string raw_json = File.ReadAllText(name + extention);
				GraphNode node = new GraphNode();
				node.ParseFromJson(new JsonObject(raw_json));
				root_node = node;
            }
			catch (FileNotFoundException)
			{
				Logger.LogError("Unable to import '" + name + extention + "'");
			}
			
		}

		public override string ToString()
		{
			return root_node.ToString();
		}
	}
}
