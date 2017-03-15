using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropDown : MonoBehaviour {

    public RectTransform container;
    public bool isOpen;

	// Use this for initialization
	void Start () {
        container.transform.FindChild("Container").GetComponent<RectTransform>();
        isOpen = false;
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isOpen)
        {
            Vector3 scale = container.localScale;
            scale.y = Mathf.Lerp(scale.y, 1, Time.deltaTime * 12);
            container.localScale = scale;

        }
        else
        {
            Vector3 scale = container.localScale;
            scale.y = Mathf.Lerp(scale.y, 0, Time.deltaTime * 12);
            container.localScale = scale;
        }
		
	}
}
