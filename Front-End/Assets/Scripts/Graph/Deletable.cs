using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NodeDescription))]
public class Deletable : MonoBehaviour {
	
	private NodeDescription Node;
	
	void Start ()
	{
		Node = GetComponent<NodeDescription>();

		if (Node.ReferenceNode.module_id == 0 && Node.ReferenceNode.node_id == 1) 
			enabled = false;
    }
	
	void OnMouseOver()
	{
		if (Input.GetKeyDown(KeyCode.Delete))
			VScriptManager.main.RemoveNode(Node);
	}
}
