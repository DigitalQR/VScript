using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using VScript_Core;
using VScript_Core.Graphing;

public class VScriptManager : MonoBehaviour {

	public static VScriptManager main { get; private set; }

	[SerializeField]
	private NodeDescription NodeObject;
	[SerializeField]
	private ConsoleOutputListener ConsoleOutput;
	[SerializeField]
	private ConsoleInputListener ConsoleInput;

	public Graph CurrentGraph { get; private set; }
	private Thread ExecutionThread;
	private Dictionary<System.Guid, NodeDescription> CurrentGraphNodes;

	void Start ()
	{
		if (main != null)
			return;
		else
			main = this;

		VSLogger.stamp_time = false;
        VSLogger.std_print = StdPrint;
		VSLogger.debug_print = DebugPrint;
		VSLogger.error_print = ErrorPrint;

		VScriptEngine.engine_directory = "VScript/";
		VScriptEngine.input_function = ConsoleInput.ReadInput;
        VScriptEngine.Init();
    }

	void OnDestroy()
	{
        ConsoleOutput.Print("Closing VScriptEngine");
	}

	public void OpenGraph(string name)
	{
		if (CurrentGraph != null)
			CloseGraph();

		if (CurrentGraphNodes == null)
			CurrentGraphNodes = new Dictionary<System.Guid, NodeDescription>();


		CurrentGraph = VScriptEngine.GetGraph(name);

		//Create all the nodes
		foreach (KeyValuePair<System.Guid, GraphNode> pair in CurrentGraph.nodes)
		{
			NodeDescription new_node = Instantiate(NodeObject);
			new_node.SetReferenceNode(pair.Value);
			CurrentGraphNodes.Add(pair.Key, new_node);
        }

        Camera.main.transform.position = new Vector3(0, 0, -8.26f);
    }

	public void SaveGraph()
	{
		if (CurrentGraph == null || CurrentGraphNodes == null)
			return;

		foreach (KeyValuePair<System.Guid, NodeDescription> node in CurrentGraphNodes)
			node.Value.PreSave();

		VScriptEngine.SaveAll();
	}

	public void CloseGraph()
	{
		if (CurrentGraph == null || CurrentGraphNodes == null)
			return;
		SaveGraph();

		foreach (KeyValuePair<System.Guid, NodeDescription> node in CurrentGraphNodes)
			Destroy(node.Value.gameObject);
		
		CurrentGraphNodes.Clear();

		if (ExecutionThread != null)
		{
			ExecutionThread.Abort();
			ExecutionThread = null;
        }
    }

	public void ExecuteGraph()
	{
		if (CurrentGraph == null)
			return;

		if (ExecutionThread != null)
		{
			ExecutionThread.Abort();
			ExecutionThread = null;
		}

		ConsoleInput.ClearInput();

		ExecutionThread = new Thread(new ThreadStart(
			delegate () 
			{
				VScriptEngine.CompileAndRun(CurrentGraph);
			}
		));
		ExecutionThread.Start();
    }

	public NodeDescription GetNode(System.Guid guid)
	{
		return CurrentGraphNodes[guid];
    }

	void StdPrint(string message)
	{
		ConsoleOutput.Print(message);
    }

	void DebugPrint(string message)
	{
		Debug.Log(message);
	}

	void ErrorPrint(string message)
	{
		ConsoleOutput.Print("[ERROR] " + message);
	}
}
