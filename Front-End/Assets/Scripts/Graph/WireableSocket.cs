using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SocketDescription)), RequireComponent(typeof(LineRenderer))]
public class WireableSocket : MonoBehaviour {

	private SocketDescription Socket;
	private LineRenderer LineRenderer;

	private bool DraggingWire;
	private WireableSocket ConnectedSocket;

	private static Vector2 GetMouseLocation()
	{
		Vector3 world_location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return world_location;
	}

	void Start ()
	{
		Socket = GetComponent<SocketDescription>();

		LineRenderer = GetComponent<LineRenderer>();
		LineRenderer.numPositions = 2;
		LineRenderer.sortingOrder = 10;
    }
	
	void Update ()
	{
		if (ConnectedSocket != null)
		{
			LineRenderer.enabled = true;
			LineRenderer.SetPosition(0, transform.position);
			LineRenderer.SetPosition(1, ConnectedSocket.transform.position);

			LineRenderer.startColor = Socket.SocketColour;
			LineRenderer.endColor = ConnectedSocket.Socket.SocketColour;
		} 
		else if (DraggingWire)
		{
			LineRenderer.enabled = true;
			LineRenderer.SetPosition(0, transform.position);
			LineRenderer.SetPosition(1, GetMouseLocation());

			LineRenderer.startColor = Socket.SocketColour;
			LineRenderer.endColor = Socket.SocketColour;
		}
		else
			LineRenderer.enabled = false;
	}

	void OnMouseDown()
	{
		if (Input.GetMouseButtonDown(0))
		{
			DraggingWire = true;

			if (ConnectedSocket != null)
			{
				ConnectedSocket.ConnectedSocket = null;
				ConnectedSocket = null;
			}
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
				if (socket != null && socket.transform.parent != transform.parent && Socket.TypeIO != socket.Socket.TypeIO && Socket.TypeSocket == socket.Socket.TypeSocket)
				{
					ConnectedSocket = socket;

					//Update connection on other ned
					if (socket.ConnectedSocket != null && socket.ConnectedSocket.ConnectedSocket != null)
						socket.ConnectedSocket.ConnectedSocket = null;
					socket.ConnectedSocket = this;
                }
				else
					LineRenderer.enabled = false;
			}
			else
				LineRenderer.enabled = false;
		}
	}
}
