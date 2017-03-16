using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NodeDescription))]
public class TestNode : MonoBehaviour {
	
	public string Name;
	public float Size = 1.0f;
	public Color Color = Color.black;
	public Color FontColor = Color.white;

	public NodeDescription.NodeIO[] Inputs;
	public NodeDescription.NodeIO[] Outputs;

	void Start ()
	{
		NodeDescription node = GetComponent<NodeDescription>();
		node.SetHeaderText(Name);
		node.SetHeaderColour(Color);

		node.SetHeaderFontSize(Size);
		node.SetHeaderFontColour(FontColor);

		node.SetInputs(Inputs);
		node.SetOutputs(Outputs);
	}
	
}
