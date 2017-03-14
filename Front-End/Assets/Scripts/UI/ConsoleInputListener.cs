using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleInputListener : MonoBehaviour {

	[SerializeField]
	private InputField InputField;

	public void OnEndEdit()
	{
		HandleInput(InputField.text);
		InputField.text = "";
    }

	void HandleInput(string input)
	{
		Debug.Log("Do something with input '" + input + "'");

		//TESTING ONLY
		ConsoleOutputListener output = FindObjectOfType<ConsoleOutputListener>();
		output.Print(input);

	}
}
