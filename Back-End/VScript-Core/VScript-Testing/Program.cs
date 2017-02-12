using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VScript_Core;

using VScript_Core.System;

namespace VScript_Testing
{
	class Program
	{
		static void Main(string[] args)
		{
			Logger.Log("Hello World");
			SystemConsole.StartProcess("C:/Python34/python.exe", SystemConsole.GetWorkingDirectory() + "/Intermediate/print_test.py");
			Logger.Log("Done");
            Console.ReadLine();
		}
	}
}
