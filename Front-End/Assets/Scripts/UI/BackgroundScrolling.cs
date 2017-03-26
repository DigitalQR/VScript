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

		//Create new (inactive) parts where 0 is the main controller
		for (int i = 0; i < 9; i++) 
		{
			GameObject part = Instantiate(Tile);
			part.transform.parent = transform;

			switch (i)
			{
				case 0:
					part.transform.localPosition = new Vector3(0, 0, 0);
					break;
				case 1:
					part.transform.localPosition = new Vector3(0, Clamping.y, 0);
					break;
				case 2:
					part.transform.localPosition = new Vector3(Clamping.x, Clamping.y, 0);
					break;
				case 3:
					part.transform.localPosition = new Vector3(Clamping.x, 0, 0);
					break;

				case 4:
					part.transform.localPosition = new Vector3(Clamping.x * 2, 0, 0);
					break;
				case 5:
					part.transform.localPosition = new Vector3(Clamping.x * 2, Clamping.y, 0);
					break;
				case 6:
					part.transform.localPosition = new Vector3(Clamping.x * 2, Clamping.y * 2, 0);
					break;
				case 7:
					part.transform.localPosition = new Vector3(Clamping.x, Clamping.y * 2, 0);
					break;
				case 8:
					part.transform.localPosition = new Vector3(0, Clamping.y * 2, 0);
					break;
			};
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
