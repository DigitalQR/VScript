using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using VScript_Core.Graph.Json;

namespace VScript_Core.Graph
{
	public struct NodeMeta
	{
		public int module_id;
		public int node_id;
		public List<NodeMeta> forwards_conn;
		public List<NodeMeta> backwards_conn;
		

		public NodeMeta(int module_id, int node_id)
		{
			this.module_id = module_id;
			this.node_id = node_id;
			forwards_conn = new List<NodeMeta>();
			backwards_conn = new List<NodeMeta>();
		}

		public void ParseFromJson(JsonObject json)
		{
			module_id = json.Get<int>("module_id");
			node_id = json.Get<int>("node_id");

			foreach (JsonObject sub_json in json.GetObjectList("forwards_conn"))
            {
				NodeMeta sub_meta = new NodeMeta(0, 0);
				sub_meta.ParseFromJson(sub_json);
				forwards_conn.Add(sub_meta);
			}

			foreach (JsonObject sub_json in json.GetObjectList("backwards_conn"))
			{
				NodeMeta sub_meta = new NodeMeta(0, 0);
				sub_meta.ParseFromJson(sub_json);
				backwards_conn.Add(sub_meta);
			}
		}

		public JsonObject GetAsJson()
		{

			JsonObject json = new JsonObject();
			json.Put("module_id", module_id);
			json.Put("node_id", node_id);

			JsonArray forwards_json = new JsonArray();
			JsonArray backwards_json = new JsonArray();

			foreach (NodeMeta meta in forwards_conn)
				forwards_json.Add(meta.GetAsJson());

			foreach (NodeMeta meta in backwards_conn)
				backwards_json.Add(meta.GetAsJson());

			json.PutRaw("forwards_conn", forwards_json);
			json.PutRaw("backwards_conn", backwards_json);
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
		public string name { get; private set; }
		public NodeMeta root_node { get; private set; }

		public Graph(string name)
		{
			this.name = name;
			root_node = new NodeMeta(0, 0);//Null/Start node
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
				root_node.ParseFromJson(new JsonObject(raw_json));
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
