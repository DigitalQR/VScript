using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using VScript_Core.Graph.Json;

namespace VScript_Core.Graph
{
	public struct IOType
	{
		public string name;
		public bool is_execution;

		public JsonObject ToJson()
		{
			JsonObject json = new JsonObject();
			json.Put("name", name);
			json.Put("is_execution", is_execution);
			return json;
		}
	}

	public class Node
	{
		private static string extention = ".node.json";
		public string name;
		public string source;
		public int id;

		public List<IOType> inputs;
		public List<IOType> outputs;


		public Node(string name)
		{
			this.name = name;

			inputs = new List<IOType>();
			outputs = new List<IOType>();
		}

		public void Export()
		{
			File.WriteAllText(name + extention, ToString());
		}

		public void Import()
		{
			try
			{
				string raw_json = File.ReadAllText(name + extention);
				JsonObject json = new JsonObject(raw_json);

				name = json.Get<string>("name");
				id = json.Get<int>("id");
				source = json.Get<string>("source");

				foreach (JsonObject input_json in json.GetObjectList("inputs"))
				{
					IOType input = new IOType();
					input.name = input_json.Get<string>("name");
					input.is_execution = input_json.Get<bool>("is_execution");
					inputs.Add(input);
				}

				foreach (JsonObject output_json in json.GetObjectList("outputs"))
				{
					IOType output = new IOType();
					output.name = output_json.Get<string>("name");
					output.is_execution = output_json.Get<bool>("is_execution");
					outputs.Add(output);
				}
			}
			catch (FileNotFoundException)
			{
				Logger.LogError("Unable to import '" + name + extention + "'");
			}
		}

		public override string ToString()
		{
			JsonObject json = new JsonObject();
			json.Put("name", name);
			json.Put("id", id);
			json.Put("source", source);

			List<JsonObject> input_objects = new List<JsonObject>();
			List<JsonObject> output_objects = new List<JsonObject>();

			foreach (IOType input in inputs)
				input_objects.Add(input.ToJson());

			foreach (IOType output in outputs)
				output_objects.Add(output.ToJson());

			json.PutObjectList("inputs", input_objects);
			json.PutObjectList("outputs", output_objects);
			return json.ToString();
		}
	}
}
