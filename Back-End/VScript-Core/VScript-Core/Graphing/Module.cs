using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using VScript_Core.Graphing.Json;

namespace VScript_Core.Graphing
{
	public class Module
	{
		private static string extention = ".module.json";
		public static string file_extention { get { return extention; }	}

		public string name { get; private set; }
		public int id { get; private set; }
		public string lang { get; private set; }

		//Stores required file names
		public List<string> module_dep { get; private set; }
		public List<string> file_dep { get; private set; }
		public List<string> nodes { get; private set; }

		public Module(string name, string lang = "null")
		{
			this.name = name;
			this.lang = lang;
			module_dep = new List<string>();
			file_dep = new List<string>();
			nodes = new List<string>();
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
				lang = json.Get<string>("lang");

				module_dep = json.GetList<string>("module_dep");
				file_dep = json.GetList<string>("file_dep");
				nodes = json.GetList<string>("nodes");
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
			json.Put("lang", lang);

			json.PutList("module_dep", module_dep);
			json.PutList("file_dep", file_dep);
			json.PutList("nodes", nodes);
			return json.ToString();
		}

	}
}
