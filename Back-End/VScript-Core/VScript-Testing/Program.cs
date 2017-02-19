using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VScript_Core;
using VScript_Core.Graph;
using VScript_Core.Graph.Json;

using VScript_Core.System;

namespace VScript_Testing
{
	class Program
	{
		static void Main(string[] args)
		{
			Logger.Log("Hello World");
			
			/*
			JsonObject another_json = new JsonObject();
			another_json.Put("yes", true);

			JsonObject json = new JsonObject();
			json.Put("test", 2);
			json.Put("a", 3);
			json.Put("b", "awe");
			json.PutList("test", new List<string>{ "a", "we", "awewa"});

			another_json.Put("json", json);
            Logger.Log(another_json.ToString());
			*/

			JsonObject json = new JsonObject("{\"test\":30,\"a\":{\"force\":true},\"arr\":[0,1,2,\"apple\"],\"b\":\"apple\"}");
			Logger.Log(json.ToString());
			Logger.Log(json.Get<int>("test").ToString());
			Logger.Log("force: "+ json.GetObject("a").Get<bool>("force"));

			foreach(int i in json.GetList<int>("arr"))
				Logger.Log("" + i);


			Logger.Log("Done");
			Console.ReadLine();
		}
	}
}
