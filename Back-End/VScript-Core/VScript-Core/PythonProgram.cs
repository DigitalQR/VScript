using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VScript_Core.System;
using System.IO;
using System.Diagnostics;

namespace VScript_Core
{
	public class PythonProgram
	{
		public static String python_path = "C:/Python34/python.exe";
		private String file_name;
		private Process process;

		public PythonProgram(String file_name)
		{
			this.file_name = file_name;
		}

		/**
		Starts python task with given settings
		@param	args			Desired command line arguments
		@param	wait_for_exit	Whether to wait for process to finish before regaining control
		*/
		public void Run(String args = "", bool wait_for_exit = true)
		{
			Logger.DebugLog("Launching '" + file_name + "'");

			//Get full file path (Correct relative file paths)
			String full_path = file_name;
			if (file_name.Length >= 1 && file_name[1] != ':')
				full_path = SystemConsole.GetWorkingDirectory() + file_name;


			//Setup input lambda
			SystemConsole.InputFunction input_function = 
				delegate (StreamWriter writer) 
				{
					//TODO - Replace with actual input method
					writer.WriteLine(Console.ReadLine());
				};

			process = SystemConsole.StartProcess(python_path, full_path + " " + args, input_function);

			if (wait_for_exit)
			{
				process.WaitForExit();
				Logger.DebugLog("Finished '" + file_name + "'");
			}
        }
		
		public bool IsFinished()
		{
			return process != null ? process.HasExited : false;
		}
	}
}
