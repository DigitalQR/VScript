using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AreYouSureDialog : MonoBehaviour {

	public static AreYouSureDialog main { get; private set; }

	[SerializeField]
	private Text Description;
	private ConfirmOperation CurrentOperation;

	void Start ()
	{
		gameObject.SetActive(false);
		main = this;
	}

	public void OnInput(bool accepts)
	{
		if (!accepts)
		{
			Close();
			return; 
		}

		CurrentOperation();
		CurrentOperation = null;
		Close();
	}

	public void Open(string message, ConfirmOperation operation)
	{
		if (operation == null)
			return;

		Description.text = message;
		CurrentOperation = operation;
		gameObject.SetActive(true);
	}

	public void Close()
	{
		gameObject.SetActive(false);
		CurrentOperation = null;
	}
}
