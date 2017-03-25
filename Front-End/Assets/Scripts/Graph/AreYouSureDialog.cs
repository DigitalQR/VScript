using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AreYouSureDialog : MonoBehaviour {

	public static AreYouSureDialog main { get; private set; }

	[SerializeField]
	private Text description;

	void Start ()
	{
		gameObject.SetActive(false);
		main = this;
	}

	public void OnInput(bool accepts)
	{
	}
}
