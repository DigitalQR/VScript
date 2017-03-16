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
			return Directory.GetCurrentDirectory();
		}

		public delegate void InputFunction(StreamWriter writer);

		/**
			Starts the given process and links it's output to logger
			-TODO: Setup input handle

			@param	exe_name		Name of the desired executable to run
			@param	args			Desired command line arguments to launch using	
			@param	input_function	Function to used to pass input to process, if required
		*/
		public static Process StartProcess(String exe_name, String args = "", InputFunction input_function = null)
		{
			//Setup process with desired (minimized settings)
			Process process = new Process();
			process.StartInfo.FileName = exe_name;
			process.StartInfo.WorkingDirectory = GetWorkingDirectory();
			process.StartInfo.Arguments = args;
			process.StartInfo.CreateNoWindow = true;

			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			if(input_function != null)
				process.StartInfo.RedirectStandardInput = true;


			//Register output
			process.OutputDataReceived += (sender, arg) => 
			{
				if(arg != null && arg.Data != null)
					Logger.Log(arg.Data.ToString());
			};

			//Register error output
			process.ErrorDataReceived += (sender, arg) => 
			{
				if (arg == null || arg.Data == null)
					return;
                String error = arg.Data.ToString();

				if (error != null && error.Length != 0)
				{
					Logger.LogError("Process error for: '" + exe_name + "'");
					Logger.LogError("\t-args: " + args);
					Logger.LogError("\t-dir: " + GetWorkingDirectory());
					Logger.LogError("Output: ");
					Logger.LogError(error);
				}
			};


			try
			{
				//Start process and asynchronous reading
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				
				//Link input, if avaliable
				if (true)
				{
					using (StreamWriter writer = process.StandardInput)
					{
						while (!process.HasExited)
						{
							input_function(writer);
						}
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
