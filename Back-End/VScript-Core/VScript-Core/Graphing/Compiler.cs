using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VScript_Core.Graphing
{
	public class Compiler
	{
		//Singlton accessing
		private static Compiler compiler_singlton;
		public static Compiler main
		{
			get
			{
				if (compiler_singlton == null)
					compiler_singlton = new Compiler();
				return compiler_singlton;
			}
		}

		private enum IO_type { Exe, Var, Ref }
        public string build_path = "Temp/Build/";

        private string Expand(GraphNode current_node)
		{
			Node node = Library.main.GetNode(current_node);
			string raw_source = node.GetSource(0);
			string real_source = "";

            if (raw_source == "")
                return "";
			
			
			using (StringReader reader = new StringReader(raw_source))
			{
				string line = "";

				while ((line = reader.ReadLine()) != null)
				{
					int line_indentation = 0;

					string real_line = "";
					bool found_start = false;
					
					string current_io = "";
					string io_name = "";
					int io_read = 0;
					IO_type io_type;
					bool is_input = true;

					foreach (char c in line)
					{
						if (c == '{' || io_read != 0)
						{
							current_io += c;

							/*Read the tag in format such as {vi:n} where:
								v is variable type (execution, variable, reference)
								i is IO (input, output)
								n is the name (Uncapped in size)
							*/
							switch (io_read)
							{
								case 0:
									break;
								
								//Check which type of io
								case 1:
									if (c == 'e')
										io_type = IO_type.Exe;
									else if (c == 'v')
										io_type = IO_type.Var;
									else if (c == 'r')
										io_type = IO_type.Ref;
									else //Invalid
									{
										real_line += current_io;
                                        current_io = "";
										io_read = 0;
                                        continue;
									}
									break;
								
								//Check whether IO
								case 2:
									if (c == 'i')
										is_input = true;
									else if (c == 'o')
										is_input = false;
									else //Invalid
									{
										real_line += current_io;
										current_io = "";
										io_read = 0;
										continue;
									}
									break;

								//Final check to see if valid
								case 3:
									if (c != ':') //Invalid
									{
										real_line += current_io;
										current_io = "";
										io_read = 0;
										continue;
									}
									else
										io_name = "";
									break;

								default:
									if (c != '}')
										io_name += c;
									break;
							}
							io_read++;

							//At end of IO
							if (c == '}')
							{
								string new_source = "";

								if (is_input)
								{
									//Don't bother fetching what's connected to begin
									if (current_io != "{ei:begin}" && current_io != "{vi:begin}")
										new_source = Expand(current_node.GetInput(io_name));
								}
								else
								{
									//Variable ends are considered the end of the strip
									if (current_io != "{vo:end}")
										new_source = Expand(current_node.GetOutput(io_name));
								}

								//Tab in code by correct amount
								if (new_source != "")
									real_line += Node.GetCorrectedSource(new_source, line_indentation);

								current_io = "";
								io_read = 0;
							}
						}
						else
						{
							//Keep track of tabs
							if (c == '\t')
							{
								if (found_start)
									real_line += c;
								else
									line_indentation++;
                            }
							else
							{
								//Found end of initial tabs
								if (io_read == 0)
									found_start = true;
								real_line += c;
							}
						}
                    }

                    if(real_line.Trim() != "")
                        real_source += real_line + "\n";
                }
			}

            //Remove final \n
            return real_source.Length != 0 ? real_source.Substring(0, real_source.Length - 1) : "";
        }

		public string Compile(Graph graph)
        {
            string graph_path = build_path + graph.display_name + ".py";
            Logger.DebugLog("Starting Compile '" + graph_path + "'");

            Directory.CreateDirectory(build_path);
            string source = Expand(graph.start_node);
            File.WriteAllText(graph_path, source);
            Logger.DebugLog("\n==Source-start==\n" + source + "\n===Source-end===");
            Logger.DebugLog("Finished Compile!");
			return graph_path;
		}
	}
}
