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
			PythonProgram test_python = new PythonProgram("/Intermediate/print_test.py");
			test_python.Run("");
			Logger.Log("Done");
            Console.ReadLine();
		}
	}
}
