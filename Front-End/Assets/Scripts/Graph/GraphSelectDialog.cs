using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public delegate void GraphOperation(string GraphName);


public class GraphSelectDialog : MonoBehaviour {

	public static GraphSelectDialog main { get; private set; }

	[SerializeField]
	private InputField GraphInput;
	private GraphOperation CurrentOperation;

	void Start ()
	{
		gameObject.SetActive(false);
		main = this;
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

		CurrentOperation = operation;
		gameObject.SetActive(true);
	}
}
