using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VScript_Core.Graphing;
using UnityEngine.UI;

public class SearchResult : MonoBehaviour {

	[SerializeField]
	private Image HeaderBar;
	[SerializeField]
	private Text HeaderText;
	[SerializeField]
	private Text InputText;
	[SerializeField]
	private Text OutputText;
	private Node ThisNode;

	public void Build(Node node)
	{
		ThisNode = node;
        HeaderBar.color = new Color(node.colour_r, node.colour_g, node.colour_b);
		HeaderText.text = node.name;

		InputText.text = node.inputs.Count + " Inputs";
		OutputText.text = node.outputs.Count + " Outputs";
	}

	public void OnClicked()
	{
		VScriptManager.main.AddNode(ThisNode);
	}
}
