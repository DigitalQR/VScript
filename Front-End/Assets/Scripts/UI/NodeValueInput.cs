using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class NodeValueInput : MonoBehaviour {

	public static NodeValueInput main { get; private set; }

	[SerializeField]
	private InputField InputField;
	private NodeDescription CurrentNode;

	void Start ()
	{
		main = this;
		gameObject.SetActive(false);
    }
	
	void Update ()
	{
		
	}

	public void Possess(NodeDescription node)
	{
		CurrentNode = node;
    }

	public void OnEndEdit()
	{
		string input = InputField.text;
		InputField.text = "";

		if (CurrentNode != null)
		{
			CurrentNode.ReferenceNode.meta_data.Put("value", input);
			CurrentNode.Rebuild();
			CurrentNode = null;
        }

        gameObject.SetActive(false);
	}
}
