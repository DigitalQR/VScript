using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.IO;
using System.ComponentModel;

using VScript_Core.Graphing;

namespace VScript_Core.System
{
    public class PythonConsole
    {        
        public static String exe_path = "C:/Python34/python.exe";
        public delegate byte[] ReadInput();
		public static Process CurrentProcess { get; private set; }
		public static bool Running { get { return CurrentProcess != null && !CurrentProcess.HasExited; } }
		private static bool Aborting = false;

		public static void RunGraph(string graph_path, ReadInput input_function)
        {
			if (CurrentProcess != null && !CurrentProcess.HasExited)
				CurrentProcess.Kill();

			CurrentProcess = FetchNewProcess(graph_path);
			Aborting = false;

			if (input_function != null)
				CurrentProcess.StartInfo.RedirectStandardInput = true;

            try
            {
				//Start process and asynchronous reading
				CurrentProcess.Start();
				CurrentProcess.BeginOutputReadLine();
				CurrentProcess.BeginErrorReadLine();
				
				if (input_function != null)
				{
					using (StreamWriter writer = new StreamWriter(CurrentProcess.StandardInput.BaseStream, Encoding.ASCII))
					{
						while (!CurrentProcess.HasExited && !Aborting)
						{
							byte[] input = input_function();

							if (input != null && input.Length != 0)
								writer.WriteLine(Encoding.ASCII.GetString(input));
							else
								writer.Flush();
						}
					}
				}

				if (Aborting && !CurrentProcess.HasExited)
				{
					CurrentProcess.Kill();
					CurrentProcess.WaitForExit();
					VSLogger.Log("Aborted current process.");

				}
            }
            catch (Win32Exception exception)
            {
                VSLogger.LogError("Output:\n" + exception.ToString());
            }

			CurrentProcess.WaitForExit();
			CurrentProcess = null;
			Aborting = false;
        }

        private static Process FetchNewProcess(string graph_path, string args = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = exe_path;
            process.StartInfo.WorkingDirectory = VScriptEngine.executable_directory;
            Directory.CreateDirectory(VScriptEngine.executable_directory);

            process.StartInfo.Arguments = "\"" + Directory.GetCurrentDirectory() + "/" + graph_path + "\" " + args;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            //Register std output
            process.OutputDataReceived += (sender, arg) =>
            {
                if (arg != null && arg.Data != null)
                    VSLogger.Log(arg.Data.ToString());
            };

            //Register error output
            process.ErrorDataReceived += (sender, arg) =>
            {
                if (arg == null || arg.Data == null)
                    return;
                String error = arg.Data.ToString();

                if (error != null && error.Length != 0)
                {
                    VSLogger.LogError(error);
                }
            };

            return process;
        }

		public static bool AbortCurrentProcess()
		{
			if (Aborting)
				return true;

			if (!Running)
			{
				VSLogger.LogError("Cannot abort process, as none is active");
				return false;
			}
			
			Aborting = true;
            return true;
		}
	}
}
