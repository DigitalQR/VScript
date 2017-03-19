using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using VScript_Core.Graphing.Json;

namespace VScript_Core.Graphing
{
	public class GraphNode
	{
        public Graph parent;

        public Guid guid;
		public int module_id;
		public int node_id;
		public Dictionary<string, Guid> inputs;
		public Dictionary<string, Guid> outputs;
		public JsonObject meta_data;

		internal GraphNode(int module_id = 0, int node_id = 0)
		{
            guid = Guid.NewGuid();
			this.module_id = module_id;
			this.node_id = node_id;
			inputs = new Dictionary<string, Guid>();
			outputs = new Dictionary<string, Guid>();
			meta_data = new JsonObject();
        }

		public GraphNode GetInput(string name)
		{
			if (inputs.ContainsKey(name))
				return parent.GetNode(inputs[name]);
			else
				return new GraphNode(0, 0);
		}

		public GraphNode GetOutput(string name)
		{
			if (outputs.ContainsKey(name))
				return parent.GetNode(outputs[name]);
			else
				return new GraphNode(0, 0);
		}

		public void ParseFromJson(JsonObject json)
		{
            guid = new Guid(json.Get<string>("guid"));
            module_id = json.Get<int>("module_id");
			node_id = json.Get<int>("node_id");
			meta_data = json.GetObject("meta_data");

            foreach (JsonObject sub_json in json.GetObjectList("inputs"))
            {
                string name = sub_json.Get<string>("name");
                Guid guid = new Guid(sub_json.Get<string>("guid"));
                inputs.Add(name, guid);
            }

            foreach (JsonObject sub_json in json.GetObjectList("outputs"))
            {
                string name = sub_json.Get<string>("name");
                Guid guid = new Guid(sub_json.Get<string>("guid"));
                outputs.Add(name, guid);
            }
        }

		public JsonObject GetAsJson()
		{
			JsonObject json = new JsonObject();
            json.Put("guid", guid.ToString());
            json.Put("module_id", module_id);
			json.Put("node_id", node_id);
			json.PutRaw("meta_data", meta_data);

			JsonArray input_json = new JsonArray();
			JsonArray output_json = new JsonArray();

            foreach (KeyValuePair<string, Guid> connection in inputs)
            {
                JsonObject con_json = new JsonObject();
                con_json.Put("name", connection.Key);
                con_json.Put("guid", connection.Value.ToString());
                input_json.Add(con_json);
            }

            foreach (KeyValuePair<string, Guid> connection in outputs)
            {
                JsonObject con_json = new JsonObject();
                con_json.Put("name", connection.Key);
                con_json.Put("guid", connection.Value.ToString());
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

        public string display_name { get; private set; }
        public GraphNode start_node { get; private set; }
        public Dictionary<Guid, GraphNode> nodes;

		public Graph(string name)
		{
			this.display_name = name;
            nodes = new Dictionary<Guid, GraphNode>();

            start_node = AddNode(0, 1);//Start node
            start_node.guid = Guid.Empty;
        }

        public GraphNode AddNode(int module_id, int node_id)
        {
            GraphNode node = new GraphNode(module_id, node_id);
            if (module_id == 0 && node_id == 1)
                node.guid = Guid.Empty;

            return AddNode(node);
        }

        private GraphNode AddNode(GraphNode node)
        {
            node.parent = this;
            nodes.Add(node.guid, node);
            
            if (start_node == null && node.guid == Guid.Empty)
                start_node = node;

            return node;
        }

        public void RemoveNode(GraphNode node)
        {
			foreach (KeyValuePair<Guid, GraphNode> other_node in nodes)
			{
				foreach (KeyValuePair<string, Guid> connection in other_node.Value.inputs.Where(con => con.Value == node.guid).ToList())
					other_node.Value.inputs.Remove(connection.Key);

				foreach (KeyValuePair<string, Guid> connection in other_node.Value.outputs.Where(con => con.Value == node.guid).ToList())
					other_node.Value.outputs.Remove(connection.Key);
			}

            nodes.Remove(node.guid);
        }

        public void Clear()
        {
            nodes.Clear();
            start_node = null;
        }

        public void AddConnection(GraphNode out_node, string out_key, GraphNode in_node, string in_key)
        {
            out_node.outputs[out_key] = in_node.guid;
            in_node.inputs[in_key] = out_node.guid;
        }

        public void RemoveConnection(GraphNode out_node, string out_key, GraphNode in_node, string in_key)
        {
            if (out_node.outputs.ContainsKey(out_key) && in_node.inputs.ContainsKey(in_key)
                && out_node.outputs[out_key] == in_node.guid && in_node.inputs[in_key] == out_node.guid
                )
            {
                out_node.outputs.Remove(out_key);
                in_node.inputs.Remove(in_key);
            }
            else
                VSLogger.LogError("Attempting to remove invalid connection between '" + out_node.module_id + ":" + out_node.node_id + "' and '" + in_node.module_id + ":" + in_node.node_id + "'");
        }

        public GraphNode GetNode(Guid guid)
        {
            return nodes[guid];
        }

		public void Export(string project_directory)
		{
			File.WriteAllText(project_directory + display_name + extention, ToString());
		}

		public bool Import(string project_directory)
		{
			try
			{
                Clear();

                JsonObject json = new JsonObject(File.ReadAllText(project_directory + display_name + extention));

                foreach (JsonObject node_json in json.GetObjectList("nodes"))
                {
                    GraphNode node = new GraphNode();
                    node.ParseFromJson(node_json);
                    AddNode(node);
                }

                return true;
			}
			catch (DirectoryNotFoundException)
			{
				return false;
			}
			catch (FileNotFoundException)
			{
				return false;
			}

		}

		public override string ToString()
		{
            JsonObject json = new JsonObject();
            
            JsonArray nodes_json = new JsonArray();

            foreach (KeyValuePair<Guid, GraphNode> graph_node in nodes)
                nodes_json.AddRaw(new JsonObject(graph_node.Value.ToString()));

            json.PutRaw("nodes", nodes_json);
            return json.ToString();
		}
	}
}
