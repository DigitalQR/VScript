using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VScript_Core.Graphing.Json
{
	public class JsonValue
	{
	}

	public class JsonValue<Type> : JsonValue
	{
		public Type raw_value;

		public JsonValue(Type value)
		{
			this.raw_value = value;
		}

		public override string ToString()
		{
            return raw_value.ToString();
		}
	}
}
