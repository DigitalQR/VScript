using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour {

	private bool BeingDragged;
	private Vector3 Anchor;

	private Vector3 GetMouseLocation()
	{
		Vector3 world_location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		world_location.z = transform.position.z;
		return world_location;
	}

	void Update ()
	{
		if (!BeingDragged)
			return;
		
        transform.position = GetMouseLocation() - Anchor;
	}

	void OnMouseDown()
	{
		if (Input.GetMouseButtonDown(0))
		{
			BeingDragged = true;
			Anchor = GetMouseLocation() - transform.position;
        }
    }

	void OnMouseUp()
	{
		if (Input.GetMouseButtonUp(0))
			BeingDragged = false;
	}
}
