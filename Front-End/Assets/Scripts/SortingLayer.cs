using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshRenderer))]
public class SortingLayer : MonoBehaviour {

	[SerializeField]
	private int Layer;
	private MeshRenderer Mesh;

	void Start ()
	{
		Mesh = GetComponent<MeshRenderer>();
	}
	
	void Update ()
	{
#if UNITY_EDITOR
		Mesh.sortingOrder = Layer;
#endif
	}
}
