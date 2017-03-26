using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.UI;
using VScript_Core;


public delegate void GraphOperation(string GraphName);


public class GraphSelectDialog : MonoBehaviour {

	public static GraphSelectDialog main { get; private set; }

	[SerializeField]
	private InputField GraphInput;
	[SerializeField]
	private GameObject ScrollPane;
	[SerializeField]
	private GraphEntryDialog BaseEntry;
	private GraphOperation CurrentOperation;

	void Start ()
	{
		gameObject.SetActive(false);
		main = this;
	}

	public void Select(string name)
	{
		GraphInput.text = name;
	}

	public void OnAccept()
	{
		string name = GraphInput.text;
		if (name == null || name.Trim() == "")
			name = "Untitled";

		gameObject.SetActive(false);
		CurrentOperation(name);
		OnClose();
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
		CurrentOperation = null;
		GraphInput.text = "";
	}

	public void Open(GraphOperation operation)
	{
		if (operation == null)
			return;

		RebuildEntries();
		CurrentOperation = operation;
		gameObject.SetActive(true);
	}

	void ClearEntries()
	{
		foreach (GraphEntryDialog entry in ScrollPane.GetComponentsInChildren<GraphEntryDialog>())
			Destroy(entry.gameObject);
	}

	void RebuildEntries()
	{
		ClearEntries();
		string project_directory = VScriptEngine.engine_directory + VScriptEngine.project_directory + "/";

		foreach (string file in Directory.GetFiles(project_directory))
		{
			if (!file.EndsWith(VScript_Core.Graphing.Graph.file_extention))
				continue;

			string name = file.Substring(project_directory.Length, file.Length - project_directory.Length - VScript_Core.Graphing.Graph.file_extention.Length);

			GraphEntryDialog entry = Instantiate(BaseEntry, ScrollPane.transform);
			entry.transform.localScale = new Vector3(1, 1, 1);
			entry.SetName(name);
		}
	}
}
