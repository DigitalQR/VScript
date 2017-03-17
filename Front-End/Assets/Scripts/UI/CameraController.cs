using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

    public static CameraController main { get; private set; }
    private Camera camera;
    private float zoom;
    private Vector2 anchor;
    private Vector2 offset;
    private bool dragging = false;
    private float start_distance;

    private Vector2 GetMouseLocation()
    {
        Vector3 world_location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return world_location - main.transform.position;
    }

    void Start ()
    {
        main = this;
        camera = GetComponent<Camera>();
        zoom = camera.orthographicSize;
        start_distance = transform.position.z;
    }
	
	void Update ()
    {
        UpdateMousePosition();
        UpdateCameraZoom();
    }

    void UpdateCameraZoom()
    {
        zoom -= Input.GetAxis("Mouse ScrollWheel") * 2.0f;

        if (zoom < 4.16f)
            zoom = 4.16f;
        if (zoom > 15.0f)
            zoom = 15.0f;

        camera.orthographicSize = zoom;
    }


    void UpdateMousePosition()
    {
        bool draggingNow = Input.GetMouseButton(1);

        if (dragging != draggingNow)
        {
            if (draggingNow)
            {
                anchor = GetMouseLocation();
                offset = new Vector2(transform.position.x, transform.position.y);
            }
            dragging = draggingNow;
        }


        if (draggingNow)
        {
            camera.transform.position = anchor - GetMouseLocation() + offset;
            camera.transform.position += new Vector3(0, 0, start_distance);
        }
    }
}
