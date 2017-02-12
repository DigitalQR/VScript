using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VScript_Core
{
	public class Logger
	{
		public static void Log(String message)
		{
			String date_stamp = DateTime.Now.ToString("[HH:mm:ss]");
			Console.WriteLine(date_stamp + message);
		}
	}
}
