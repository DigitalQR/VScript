using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VScript_Core.Graphing.Json
{
	public class JsonArray : JsonValue
	{
		public List<JsonValue> values;

		public JsonArray(string raw_array = "")
		{
			values = new List<JsonValue>();


			if (raw_array.Length == 0)
				return;


			//Parse raw array
			int bracket_count = -1;
			bool reading_string = false;

			bool reading_sub_object = false;
			bool reading_sub_array = false;
			bool just_read_sub_struct = false;
			
			string current_value = "";

			foreach (char c in raw_array)
			{
				if (!reading_string)
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
						values.Add(new JsonObject(current_value));

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
						values.Add(new JsonArray(current_value));

						current_value = "";
						reading_sub_array = false;
						just_read_sub_struct = true;
						continue;
					}
				}

				
				//Found key value pair
				if (c == ',' || bracket_count == -1)
				{
					string value = current_value;
					current_value = "";

                    //Attempt to find correct type
                    //Int
                    try
					{
						int raw_value = Int32.Parse(value);
						values.Add(new JsonValue<int>(raw_value));
						continue;
					}
					catch (FormatException) { }

					//Double
					try
					{
						double raw_value = double.Parse(value);
						values.Add(new JsonValue<double>(raw_value));
						continue;
					}
					catch (FormatException) { }

					//Bool
					try
					{
						bool raw_value = bool.Parse(value);
						values.Add(new JsonValue<bool>(raw_value));
						continue;
					}
					catch (FormatException) { }
                    
                    //String
                    if (value.StartsWith("\""))
                    {
                        string corrected_string = value.Substring(1).Substring(0, value.Length - 2);
                        values.Add(new JsonValue<string>(corrected_string));
                        continue;
                    }

					//Unknown type, if here
				}

                if (c == '\r')
                    continue;

                if (c == '"')
                    reading_string = !reading_string;

                if (!reading_string && (c == '\n' || c == '\t' || c == '\r'))
                    continue;

                current_value += c;
			}
		}

		/**
		Returns all elements of given type
		*/
		public List<Type> Get<Type>()
		{
			List<Type> value_list = new List<Type>();

			foreach (JsonValue pair in values)
			{
				try
				{
					JsonValue<Type> type_pair = (JsonValue<Type>)pair;

					if (type_pair != null)
						value_list.Add(type_pair.raw_value);
				}
				catch (InvalidCastException)
				{
				}
			}

			return value_list;
		}

		/**
		Returns all elements of given type
		*/
		public List<JsonObject> GetObjects()
		{
			List<JsonObject> value_list = new List<JsonObject>();

			foreach (JsonValue pair in values)
			{
				try
				{
					JsonObject type_pair = (JsonObject)pair;

					if (type_pair != null)
						value_list.Add(type_pair);
				}
				catch (InvalidCastException)
				{
				}
			}

			return value_list;
		}

		/**
		Add raw JsonValue object
		*/
		public void AddRaw(JsonValue value)
		{
			values.Add(value);
		}

		/**
		Generic added for value
		*/
		public void Add<Type>(Type value)
		{
			values.Add(new JsonValue<Type>(value));
		}

		/**
		Encode current object in json format
		*/
		public override string ToString()
		{
			string formatted_json = "[";
			bool first = true;

			foreach (JsonValue pair in values)
			{
				if (first)
					first = false;
				else
					formatted_json += ",";

				if (pair == null)
				{
					formatted_json += "null";
					continue;
				}
				try
				{
					JsonValue<string> str_value = (JsonValue<string>)pair;
					formatted_json += "\"" + str_value.raw_value + '"';
				}
				catch (InvalidCastException e)
				{
					//Use default format
					formatted_json += pair.ToString();
				}
			}

			formatted_json += "]";
			return formatted_json;
		}
	}


}
