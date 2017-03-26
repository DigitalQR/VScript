using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using VScript_Core;

public class VScriptIO : MonoBehaviour {


	void Start ()
	{	
	}
	
	void Update ()
	{	
	}

	private bool DoesGraphExist(string name)
	{
		string[] files = Directory.GetFiles(VScriptEngine.engine_directory + VScriptEngine.project_directory + "/");

		foreach (string file in files)
			if (file.EndsWith(name + VScript_Core.Graphing.Graph.file_extention))
				return true;

		return false;
	}

	public void New()
	{
		GraphSelectDialog.main.Open(
			delegate (string name) 
			{
				if (DoesGraphExist(name))
				{
					AreYouSureDialog.main.Open(
						"A graph with the same name already exists.\nAre you sure you want to overwrite it?",
						delegate ()
						{
							VScriptManager.main.NewGraph(name);
						}
					);
				}
			}
		);
	}

	public void Open()
	{
		GraphSelectDialog.main.Open(
			delegate (string name) 
			{
				VScriptManager.main.OpenGraph(name);
			}
		);
	}

	public void Close()
	{
		VScriptManager.main.CloseGraph();
	}

	public void Save()
	{
		VScriptManager.main.SaveGraph();
	}
}
