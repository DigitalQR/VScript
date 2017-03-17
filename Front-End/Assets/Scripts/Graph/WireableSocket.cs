using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireableSocket : SocketDescription {

	public NodeDescription ParentNode;
	private LineRenderer LineRenderer;

	private bool DraggingWire;
	private int WireResolution = 3;
	public WireableSocket ConnectedSocket;

	private static Vector2 GetMouseLocation()
	{
		Vector3 world_location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return world_location;
	}

	void Start ()
	{
		LineRenderer = GetComponent<LineRenderer>();
		WireResolution = LineRenderer.numPositions;
		LineRenderer.sortingOrder = 10;
    }
	
	void Update ()
	{
		DrawWire();
    }

	static Vector3 GetBezierPosition(Vector3 start, Vector3 end, float t)
	{
		Vector3 p0 = start;
		Vector3 p1 = p0 + new Vector3(1, 0, 0);
		Vector3 p3 = end;
		Vector3 p2 = p3 + new Vector3(-1, 0, 0);
		return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
	}

	void DrawWire()
	{
		if ((ConnectedSocket != null && SocketIOType == IOType.Output) || DraggingWire)
		{
			LineRenderer.enabled = true;
			Vector3 start_position = transform.position;
			Vector3 end_position;
			
			//Set colour and desired end position
			if (ConnectedSocket != null)
			{
				end_position = ConnectedSocket.transform.position;
				LineRenderer.startColor = SocketColour;
				LineRenderer.endColor = ConnectedSocket.SocketColour;
			}
			else
			{
				LineRenderer.startColor = SocketColour;
				LineRenderer.endColor = SocketColour;

				//Switch positions if dragging wire for input
				if (SocketIOType == IOType.Input)
				{
					end_position = start_position;
					start_position = GetMouseLocation();
				}
				else
					end_position = GetMouseLocation();
			}

			LineRenderer.SetPosition(0, start_position);
			LineRenderer.SetPosition(WireResolution - 1, end_position);

			for (int i = 1; i < WireResolution-1; i++) 
			{
				float v = (float)(i) / (float)(WireResolution - 1);
				LineRenderer.SetPosition(i, GetBezierPosition(start_position + new Vector3(0.1f, 0, 0), end_position + new Vector3(-0.1f, 0, 0), v));
			}
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
