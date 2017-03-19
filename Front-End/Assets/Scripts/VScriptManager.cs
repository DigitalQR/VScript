using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using VScript_Core;
using VScript_Core.Graphing;
using VScript_Core.System;

public class VScriptManager : MonoBehaviour {

	public static VScriptManager main { get; private set; }

	[SerializeField]
	private NodeDescription NodeObject;
	[SerializeField]
	private ConsoleOutputListener ConsoleOutput;
	[SerializeField]
	private ConsoleInputListener ConsoleInput;
	[SerializeField]
	private GameObject GraphSheild;

	public Graph CurrentGraph { get; private set; }
	private Thread ExecutionThread;
	private Dictionary<System.Guid, NodeDescription> CurrentGraphNodes;

	void Start ()
	{
		if (main != null)
			return;
		else
			main = this;
		GraphSheild.SetActive(true);

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
	}

	public void NewGraph(string name)
	{
		Graph graph = VScriptEngine.NewGraph(name);
		OpenGraph(name);
	}

	public void OpenGraph(string name)
	{
		if (CurrentGraph != null)
			CloseGraph();

		if (CurrentGraphNodes == null)
			CurrentGraphNodes = new Dictionary<System.Guid, NodeDescription>();


		CurrentGraph = VScriptEngine.OpenGraph(name);

		if (CurrentGraph == null)
			return;

		Vector2 desired_position = new Vector2();

		//Create all the nodes
		foreach (KeyValuePair<System.Guid, GraphNode> pair in CurrentGraph.nodes)
		{
			NodeDescription new_node = Instantiate(NodeObject);
			new_node.SetReferenceNode(pair.Value);
			CurrentGraphNodes.Add(pair.Key, new_node);

			//If start node
			if (pair.Value.module_id == 0 && pair.Value.node_id == 1)
				desired_position = new_node.transform.position;
        }
		GraphSheild.SetActive(false);
        Camera.main.transform.position = new Vector3(desired_position.x, desired_position.y - 2.0f, -8.26f);
    }

	public void SaveGraph()
	{
		if (CurrentGraph == null || CurrentGraphNodes == null)
			return;

		foreach (KeyValuePair<System.Guid, NodeDescription> node in CurrentGraphNodes)
			node.Value.PreSave();

		VScriptEngine.SaveOpenGraphs();
	}

	public void CloseGraph()
	{
		if (CurrentGraph == null || CurrentGraphNodes == null)
			return;

		VScriptEngine.CloseGraph(CurrentGraph.display_name, false);

		foreach (KeyValuePair<System.Guid, NodeDescription> node in CurrentGraphNodes)
			Destroy(node.Value.gameObject);
		
		CurrentGraphNodes.Clear();

		if (ExecutionThread != null)
		{
			ExecutionThread.Abort();
			ExecutionThread = null;
        }
		GraphSheild.SetActive(true);
		CurrentGraph = null;
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

	public void AbortExecution()
	{
		if (CurrentGraph == null || CurrentGraphNodes == null)
			return;

		PythonConsole.AbortCurrentProcess();
    }

	public NodeDescription AddNode(int module_id, int node_id)
	{
		if (CurrentGraph == null || CurrentGraphNodes == null)
			return null;

		GraphNode graph_node = CurrentGraph.AddNode(module_id, node_id);

		NodeDescription new_node = Instantiate(NodeObject);
		new_node.SetReferenceNode(graph_node);
		CurrentGraphNodes.Add(graph_node.guid, new_node);

		//Set location to middle of screen
		Vector3 position = Camera.main.transform.position + new Vector3(0, 2.0f, 0);
		position.z = 0;
		new_node.transform.position = position;

		return new_node;
	}

	public void RemoveNode(NodeDescription node)
	{
		if (CurrentGraph == null || CurrentGraphNodes == null || (node.ReferenceNode.module_id == 0 && node.ReferenceNode.node_id == 1))
			return;

		CurrentGraph.RemoveNode(node.ReferenceNode);
		CurrentGraphNodes.Remove(node.ReferenceNode.guid);
		Destroy(node.gameObject);
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
