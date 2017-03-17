using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Text;

public class ConsoleInputListener : MonoBehaviour {

	[SerializeField]
	private InputField InputField;
	private string next_input;

	public void OnEndEdit()
	{
		HandleInput(InputField.text);
		InputField.text = "";
    }

	void HandleInput(string input)
	{
		next_input = input;
	}

	public void ClearInput()
	{
		next_input = "";
    }
	public byte[] ReadInput()
	{
		string input = next_input;
		next_input = "";
		return Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(input));
    }
}
