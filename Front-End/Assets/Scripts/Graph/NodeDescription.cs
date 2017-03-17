using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VScript_Core.Graphing;

public class NodeDescription : MonoBehaviour {
	
	public GraphNode ReferenceNode { get; private set; }

    [SerializeField]
    private TextMesh HeaderText;
    [SerializeField]
    private TextMesh DescriptionText;
    [SerializeField]
	private SpriteRenderer HeaderBar;
	[SerializeField]
	private GameObject ResizableBody;

	[SerializeField]
	private WireableSocket DefaultSocket;
	private Dictionary<string, WireableSocket> Inputs;
	private Dictionary<string, WireableSocket> Outputs;
	private const float ConnectionLocation = 1.564f;
	private const float ConnectionSpacing = 0.296f;


	public void SetReferenceNode(GraphNode node)
	{
		ReferenceNode = node;
		transform.position = new Vector3(
			ReferenceNode.meta_data.Get<float>("transform_x"),
            ReferenceNode.meta_data.Get<float>("transform_y"),
            ReferenceNode.meta_data.Get<float>("transform_z")
		);

		Rebuild();
		Rewire();
    }

	public void PreSave()
	{
		ReferenceNode.meta_data.Put("transform_x", transform.position.x);
		ReferenceNode.meta_data.Put("transform_y", transform.position.y);
		ReferenceNode.meta_data.Put("transform_z", transform.position.z);
	}

	public static void AddConnection(NodeDescription out_node, string out_key, NodeDescription in_node, string in_key)
	{
		WireableSocket out_socket = out_node.Outputs[out_key];
		WireableSocket in_socket = in_node.Inputs[in_key];

		out_socket.ConnectedSocket = in_socket;
		in_socket.ConnectedSocket = out_socket;
	}

	void Rebuild()
	{
		Node node = Library.main.GetNode(ReferenceNode);
		name = node.name;

        //Is const input node
        if (ReferenceNode.module_id == 0 && ReferenceNode.node_id == 2)
            DescriptionText.text = ReferenceNode.meta_data.Get<string>("value");
        else
            DescriptionText.text = "";

        HeaderText.text = node.name;
		HeaderText.color = Color.white;
		
		HeaderBar.color = new Color(node.colour_r, node.colour_g, node.colour_b);

		List<NodeIO> inputs_io = new List<NodeIO>();
		foreach (NodeIO node_io in node.inputs)
			inputs_io.Add(node_io);
		SetInputs(inputs_io);

		List<NodeIO> outputs_io = new List<NodeIO>();
		foreach (NodeIO node_io in node.outputs)
			outputs_io.Add(node_io);
		SetOutputs(outputs_io);
	}

	void Rewire()
	{
        foreach (KeyValuePair<string, System.Guid> node_desc in ReferenceNode.inputs)
		{
			try
			{
				NodeDescription other_node = VScriptManager.main.GetNode(node_desc.Value);
				string other_name = "";

				foreach (KeyValuePair<string, System.Guid> other_node_desc in other_node.ReferenceNode.outputs)
				{
					if (other_node_desc.Value == ReferenceNode.guid)
					{
						other_name = other_node_desc.Key;
						break;
					}
				}

				AddConnection(other_node, other_name, this, node_desc.Key);
			}
			catch (KeyNotFoundException) { };
		}

		foreach (KeyValuePair<string, System.Guid> node_desc in ReferenceNode.outputs)
		{
			try
			{
				NodeDescription other_node = VScriptManager.main.GetNode(node_desc.Value);
				string other_name = "";

				foreach (KeyValuePair<string, System.Guid> other_node_desc in other_node.ReferenceNode.inputs)
				{
					if (other_node_desc.Value == ReferenceNode.guid)
					{
						other_name = other_node_desc.Key;
						break;
					}
				}

				AddConnection(this, node_desc.Key, other_node, other_name);
			}
			catch (KeyNotFoundException) { };
		}
	}

	private void SetInputs(List<NodeIO> inputs)
	{
		if (Inputs != null)
		{
			//Remove existing sockets
			foreach (KeyValuePair<string, WireableSocket> socket in Inputs)
				Destroy(socket.Value.gameObject);
			Inputs.Clear();
		}
		else
			Inputs = new Dictionary<string, WireableSocket>();

		int i = 0;
		
		//Spawn new sockets
		foreach (NodeIO input in inputs)
		{
			GameObject socket = Instantiate(DefaultSocket.gameObject, transform);

			//Setup socket
			WireableSocket socket_desc = socket.GetComponent<WireableSocket>();
			socket_desc.ParentNode = this;
			socket_desc.name = input.name;
			socket_desc.SetName(input.display_name);
			socket_desc.SetNameColour(Color.black);
			socket_desc.SetSocketColour(new Color(input.colour_r, input.colour_g, input.colour_b));
			socket_desc.SetSocketType(input.is_execution ? SocketDescription.SocketType.Execution : SocketDescription.SocketType.Variable);
			socket_desc.SetIOType(SocketDescription.IOType.Input);

			socket.transform.localPosition = new Vector2(
				-ConnectionLocation,
				-0.414f - ConnectionSpacing * i
				);

			Inputs.Add(input.name, socket_desc);
			i++;
		}
		CheckNodeSize();
    }

	private void SetOutputs(List<NodeIO>outputs)
	{
		if (Outputs != null)
		{
			//Remove existing sockets
			foreach (KeyValuePair<string, WireableSocket> socket in Outputs)
				Destroy(socket.Value.gameObject);
			Outputs.Clear();
		}
		else
			Outputs = new Dictionary<string, WireableSocket>();

		int i = 0;

		//Spawn new sockets
		foreach (NodeIO output in outputs)
		{
			GameObject socket = Instantiate(DefaultSocket.gameObject, transform);

			//Setup socket
			WireableSocket socket_desc = socket.GetComponent<WireableSocket>();
			socket_desc.ParentNode = this;
			socket_desc.name = output.name;
            socket_desc.SetName(output.display_name);
			socket_desc.SetNameColour(Color.black);
			socket_desc.SetSocketColour(new Color(output.colour_r, output.colour_g, output.colour_b));
			socket_desc.SetSocketType(output.is_execution ? SocketDescription.SocketType.Execution : SocketDescription.SocketType.Variable);
			socket_desc.SetIOType(SocketDescription.IOType.Output);

			socket.transform.localPosition = new Vector2(
				ConnectionLocation,
				-0.414f - ConnectionSpacing * i
				);

			Outputs.Add(output.name, socket_desc);
			i++;
		}
		CheckNodeSize();
    }

	private void CheckNodeSize()
	{
		int size = 1;

		if (Inputs != null && Outputs != null)
		{
			if (Inputs.Count > Outputs.Count)
				size = Inputs.Count;
			else
				size = Outputs.Count;
		}
		else if (Inputs != null)
			size = Inputs.Count;

		else if (Outputs != null)
			size = Outputs.Count;
		
		ResizableBody.transform.localScale = new Vector2(
			ResizableBody.transform.localScale.x,
            (size + 2) * ConnectionSpacing * 0.5f
			);

		ResizableBody.transform.localPosition = new Vector2(
			ResizableBody.transform.localPosition.x,
			-(size + 1) * ConnectionSpacing * 0.5f
			);
	}
}
