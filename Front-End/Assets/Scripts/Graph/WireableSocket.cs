using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireableSocket : SocketDescription {

	public NodeDescription ParentNode;
	private LineRenderer LineRenderer;

	private bool DraggingWire;
	public WireableSocket ConnectedSocket;

	private static Vector2 GetMouseLocation()
	{
		Vector3 world_location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return world_location;
	}

	void Start ()
	{
		LineRenderer = GetComponent<LineRenderer>();
		LineRenderer.numPositions = 2;
		LineRenderer.sortingOrder = 10;
    }
	
	void Update ()
	{
		DrawWire();
    }

	void DrawWire()
	{
		if (ConnectedSocket != null)
		{
			LineRenderer.enabled = true;
			LineRenderer.SetPosition(0, transform.position);
			LineRenderer.SetPosition(1, ConnectedSocket.transform.position);

			LineRenderer.startColor = SocketColour;
			LineRenderer.endColor = ConnectedSocket.SocketColour;
		}
		else if (DraggingWire)
		{
			LineRenderer.enabled = true;
			LineRenderer.SetPosition(0, transform.position);
			LineRenderer.SetPosition(1, GetMouseLocation());

			LineRenderer.startColor = SocketColour;
			LineRenderer.endColor = SocketColour;
		}
		else
			LineRenderer.enabled = false;
	}

	void OnMouseDown()
	{
		if (Input.GetMouseButtonDown(0))
		{
			DraggingWire = true;
			Detach();
		}
	}

	void OnMouseUp()
	{
		if (Input.GetMouseButtonUp(0))
		{
			DraggingWire = false;

			Collider2D collider = Physics2D.OverlapPoint(GetMouseLocation());
			if (collider != null)
			{
				WireableSocket socket = collider.GetComponent<WireableSocket>();

				//Check if socket is valid
				if (socket != null && socket.transform.parent != transform.parent && SocketIOType != socket.SocketIOType && TypeSocket == socket.TypeSocket)
					Attach(socket);

				else
					LineRenderer.enabled = false;
			}
			else
				LineRenderer.enabled = false;
		}
	}

	private void Attach(WireableSocket NewSocket)
	{
		NewSocket.Detach();
		ConnectedSocket = NewSocket;
		NewSocket.ConnectedSocket = this;

		if (SocketIOType == IOType.Input)
			VScriptManager.main.CurrentGraph.AddConnection(NewSocket.ParentNode.ReferenceNode, NewSocket.name, ParentNode.ReferenceNode, name);
		else
			VScriptManager.main.CurrentGraph.AddConnection(ParentNode.ReferenceNode, name, NewSocket.ParentNode.ReferenceNode, NewSocket.name);
	}

	private void Detach()
	{
		if (ConnectedSocket != null)
		{
			if (SocketIOType == IOType.Input)
				VScriptManager.main.CurrentGraph.RemoveConnection(ConnectedSocket.ParentNode.ReferenceNode, ConnectedSocket.name, ParentNode.ReferenceNode, name);
			else
				VScriptManager.main.CurrentGraph.RemoveConnection(ParentNode.ReferenceNode, name, ConnectedSocket.ParentNode.ReferenceNode, ConnectedSocket.name);

			ConnectedSocket.ConnectedSocket = null;
			ConnectedSocket = null;
        }
    }
}
