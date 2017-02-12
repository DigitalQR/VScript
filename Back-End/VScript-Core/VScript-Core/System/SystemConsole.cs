using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace VScript_Core.System
{
	public class SystemConsole
	{
		/*
		@return	The desired working directory
		*/
		public static String GetWorkingDirectory()
		{
#if DEBUG
			return Directory.GetCurrentDirectory() + "\\..\\..";
#else
			return Directory.GetCurrentDirectory();
#endif
		}

		/**
			Starts the given process and links it's output to logger
			-TODO: Setup input handle
		*/
		public static Process StartProcess(String exe_name, String args)
		{
			Process process = new Process();
			process.StartInfo.FileName = exe_name;
			process.StartInfo.WorkingDirectory = GetWorkingDirectory();
			process.StartInfo.Arguments = args;
			process.StartInfo.CreateNoWindow = true;

			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			
			try
			{
				process.Start();

				using (StreamReader reader = process.StandardOutput)
					Logger.Log(reader.ReadLine());


				using (StreamReader reader = process.StandardError)
				{
					String error = reader.ReadLine();

					if (error != null && error.Length != 0)
					{
						Logger.LogError("Process error for: '" + exe_name + "'");
						Logger.LogError("\t-args: " + args);
						Logger.LogError("\t-dir: " + GetWorkingDirectory());
						Logger.LogError("Output: ");
						Logger.LogError(error);
					}
				}
			}
			catch (Win32Exception exception)
			{
				Logger.LogError("Could not start process '" + exe_name + "'");
				Logger.LogError("Output:\n" + exception.ToString());
			}
			
            return process;
		}
	}
}
