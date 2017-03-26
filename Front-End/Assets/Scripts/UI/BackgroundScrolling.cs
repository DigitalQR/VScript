using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour {

	[SerializeField]
	private GameObject Tile;
	private Vector2 Clamping;

	void Start ()
	{
		Clamping = Tile.transform.localScale;
		const int half_size = 2;

		//Create a box
		for (int x = -half_size; x <= half_size; x++)
			for (int y = -half_size; y <= half_size; y++)
			{
				GameObject part = Instantiate(Tile);
				part.transform.parent = transform;
				part.transform.localPosition = new Vector3(Clamping.x * x, Clamping.y * y, 0);

			}
	}

	private Vector3 Clamp(Vector3 location)
	{
		location.x = Mathf.Floor(location.x / Clamping.x) * Clamping.x;
		location.y = Mathf.Floor(location.y / Clamping.y) * Clamping.y;
		location.z = 0.0f;
		return location;
	}

	void Update ()
	{
		Vector3 camera_location = Camera.main.transform.position;
		transform.position = Clamp(camera_location);
	}
}
