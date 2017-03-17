using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class NodeValueEditor : MonoBehaviour
{
	public static NodeValueEditor main { get; private set; }

	[SerializeField]
	private InputField Field;
	private NodeDescription CurrentNode;

	void Start ()
	{
		main = this;
		gameObject.SetActive(false);
    }
	
	void Update ()
	{
		if (CurrentNode != null)
		{
			Vector2 position = Camera.main.WorldToScreenPoint(CurrentNode.transform.position);
			transform.position = position;
        }

		if (Input.GetKeyDown(KeyCode.Escape))
			Minimise();
    }

	public void OnEndEdit()
	{
		string input = Field.text;
		Field.text = "";

		if (CurrentNode != null)
		{
			CurrentNode.ReferenceNode.meta_data.Put(CurrentNode.ActualNode.meta_value_key, input);
			CurrentNode.Rebuild();
			CurrentNode.Rewire();
		}
		Minimise();
    }

	public void Minimise()
	{
		CurrentNode = null;
		gameObject.SetActive(false);
	}

	public void Possess(NodeDescription node)
	{
		CurrentNode = node;
		gameObject.SetActive(true);
	}
}
