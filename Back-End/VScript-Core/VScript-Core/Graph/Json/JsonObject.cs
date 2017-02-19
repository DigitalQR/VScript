using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VScript_Core.Graph.Json
{

	public class JsonObject : JsonValue
	{
		public Dictionary<string, JsonValue> raw_values { get; private set; }

		public JsonObject(string raw_json = "")
		{
			raw_values = new Dictionary<string, JsonValue>();
			

			if (raw_json.Length == 0)
				return;


			//Parse raw json
			int bracket_count = -1;
			bool reading_key = true;

			bool reading_sub_object = false;
			bool reading_sub_array = false;
			bool just_read_sub_struct = false;

			string current_key = "";
			string current_value = "";

			foreach (char c in raw_json)
			{
				//Ignore next ',' or '}' char
				if (just_read_sub_struct)
				{
					just_read_sub_struct = false;
                    continue;
                }
				
				//Check brackets
				if (c == '{' || c == '[')
				{
					bracket_count++;

					//Ignore first bracket
					if (bracket_count == 0)
						continue;
                }
				else if (c == '}' || c == ']')
					bracket_count--;


				//Check to see if reading array or object
				if (!reading_sub_array && c == '{')
					reading_sub_object = true;

				if (!reading_sub_object && c == '[')
					reading_sub_array = true;


				//Store object until ready
				if (reading_sub_object)
				{
					current_value += c;

					if (bracket_count != 0)
						continue;

					//At end of sub object
					PutRaw(current_key, new JsonObject(current_value));

					reading_key = true;
					current_key = "";
					current_value = "";
					reading_sub_object = false;
					just_read_sub_struct = true;
                    continue;
				}


				//Store array until ready
				if (reading_sub_array)
				{
					current_value += c;

					if (bracket_count != 0)
						continue;

					//At end of sub array
					PutRaw(current_key, new JsonArray(current_value));

					reading_key = true;
					current_key = "";
					current_value = "";
					reading_sub_array = false;
					just_read_sub_struct = true;
					continue;
				}


				//Switching between key and value
				if (reading_key && c == ':')
				{
					current_key = current_key.Substring(1).Substring(0, current_key.Length - 2);
					reading_key = false;
					continue;
				}

				//Found key value pair
				if (!reading_key && (c == ',' || bracket_count == -1))
				{
					string key = current_key;
					string value = current_value;

					reading_key = true;
					current_key = "";
					current_value = "";

					//Attempt to find correct type
					//Int
					try
					{
						int raw_value = Int32.Parse(value);
						Put(key, raw_value);
						continue;
					}
					catch (FormatException) { }

					//Double
					try
					{
						double raw_value = double.Parse(value);
						Put(key, raw_value);
						continue;
					}
					catch (FormatException) { }

					//Bool
					try
					{
						bool raw_value = bool.Parse(value);
						Put(key, raw_value);
						continue;
					}
					catch (FormatException) { }

					//String
					if (value.StartsWith("\""))
                    {
						string corrected_string = value.Substring(1).Substring(0, value.Length-2);
						Put(key, corrected_string);
						continue;
					}

					//Unknown type, if here
				}


				if (reading_key)
					current_key += c;
				else
					current_value += c;
			}
        }

		/**
		Encode current object in json format
		*/
		public override string ToString()
		{
			string formatted_json = "{";
			bool first = true;

			foreach (KeyValuePair<string, JsonValue> pair in raw_values)
			{
				if (first)
					first = false;
				else
					formatted_json += ",";
				
				if(pair.Value == null)
				{
					formatted_json += '"' + pair.Key + "\":null";
					continue;
                }

				try
				{
					JsonValue<string> str_value = (JsonValue<string>)pair.Value;
					formatted_json += '"' + pair.Key + "\":\"" + str_value.raw_value + '"';
				}
				catch (InvalidCastException e)
				{
					//Use default format
					formatted_json += '"' + pair.Key + "\":" + pair.Value.ToString();
				}
			}

			formatted_json += "}";
            return formatted_json;
		}

		/**
		Get raw JsonValue object for key
		*/
		public JsonValue GetRaw(string key)
		{
			JsonValue pair;
			raw_values.TryGetValue(key, out pair);
			return pair;
		}

		/**
		Set raw JsonValue object for a key
		*/
		public void PutRaw(string key, JsonValue value)
		{
			raw_values[key] = value;
		}


		/**
		Returns true, if json contains key
		*/
		public bool HasKey(string key)
		{
			return raw_values.ContainsKey(key);
		}


		/**
		Generic getter for key/value
		*/
		public Type Get<Type>(string key, Type default_value = default(Type))
		{
			try
			{
				JsonValue<Type> pair = (JsonValue<Type>)GetRaw(key);
				if (pair != null)
					return pair.raw_value;
				return default_value;
			}
			catch (InvalidCastException)
			{
				return default_value;
			}
		}


		/**
		Generic getter for key/value
		*/
		public JsonObject GetObject(string key)
		{
			try
			{
				JsonObject pair = (JsonObject)GetRaw(key);
				if (pair != null)
					return pair;
				return null;
			}
			catch (InvalidCastException) { }

			return null;
		}

		/**
		Generic setter for key/value
		*/
		public void Put<Type>(string key, Type value)
		{
			PutRaw(key, new JsonValue<Type>(value));
		}

		/**
		Generic getter for array of key/values
		*/
		public List<Type> GetList<Type>(string key)
		{
			JsonValue raw_value = GetRaw(key);

			try
			{
				JsonArray array = (JsonArray)raw_value;

				if (array != null)
					return array.Get<Type>();
				else
					return new List<Type>();
			}
			catch (InvalidCastException)
			{
				return new List<Type>();
			}
		}

		/**
		Generic setter for array of key/values
		*/
		public void PutList<Type>(string key, List<Type> values)
		{
			JsonArray array = new JsonArray();

			foreach (Type value in values)
				array.values.Add(new JsonValue<Type>(value));

			PutRaw(key, array);
		}
	}
}
