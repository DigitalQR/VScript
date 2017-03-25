using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VScriptIO : MonoBehaviour {


	void Start ()
	{	
	}
	
	void Update ()
	{	
	}

	public void New()
	{
		GraphSelectDialog.main.Open(
			delegate (string name) 
			{
				//Todo check if graph exists and then send warning if does
				VScriptManager.main.NewGraph(name);
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
