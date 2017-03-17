using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VScript_Core
{
	public class VSLogger
	{
        public static bool stamp_time = true;
		public delegate void Print(string message);
		public static Print std_print;
		public static Print debug_print;
		public static Print error_print;


		public static void Log(String message)
		{
			if (std_print != null)
			{
				if (stamp_time)
				{
					String date_stamp = DateTime.Now.ToString("[HH:mm:ss] ");
					std_print(date_stamp + message);
				}
				else
					std_print(message);
            }
        }

		public static void DebugLog(String message)
		{
			if (debug_print != null)
			{
				if (stamp_time)
				{
					String date_stamp = DateTime.Now.ToString("[HH:mm:ss]~ ");
					debug_print(date_stamp + message);
				}
				else
					debug_print(message);
            }
		}

		public static void LogError(String message)
		{
			if (error_print != null)
			{
				if (stamp_time)
				{
					String date_stamp = DateTime.Now.ToString("[HH:mm:ss][ERROR] ");
					error_print(date_stamp + message);
				}
				else
					error_print(message);
			}

		}
	}
}
