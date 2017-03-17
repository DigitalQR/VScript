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

        public static Process CompileAndRun(Graph graph, ReadInput input_function)
        {
            string graph_path = Compiler.main.Compile(graph);
            Process process = FetchNewProcess(graph_path);

            if (input_function != null)
                process.StartInfo.RedirectStandardInput = true;

            try
            {
                //Start process and asynchronous reading
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
				
				if (input_function != null)
				{
					using (StreamWriter writer = new StreamWriter(process.StandardInput.BaseStream, Encoding.ASCII))
					{
						while (!process.HasExited)
						{
							byte[] input = input_function();

							if (input != null && input.Length != 0)
								writer.WriteLine(Encoding.ASCII.GetString(input));
							else
								writer.Flush();
						}
					}
				}
            }
            catch (Win32Exception exception)
            {
                VSLogger.LogError("Output:\n" + exception.ToString());
            }

            return process;
        }

        private static Process FetchNewProcess(string graph_path, string args = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = exe_path;
            process.StartInfo.WorkingDirectory = VScriptEngine.executable_directory;
            Directory.CreateDirectory(VScriptEngine.executable_directory);

            process.StartInfo.Arguments = Directory.GetCurrentDirectory() + "/" + graph_path + " " + args;
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
    }
}
