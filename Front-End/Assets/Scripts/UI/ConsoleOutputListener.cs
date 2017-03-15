using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleOutputListener : MonoBehaviour {

	[SerializeField]
	private Text text;
	[SerializeField]
	private Scrollbar scrollbar;

	private string[] buffer = new string[256];
	int line_index = 0;


	void Start ()
	{
		Print("~Program Output~");
    }

	public void Print(string line)
	{
		if (line == "")
			line = " ";

		buffer[line_index++] = line;

		if (line_index >= buffer.Length)
			line_index = 0;
		
		RefreshVisuals();
		Debug.Log(line);
	}

	public void RefreshVisuals()
	{
		string display_string = "";

		for (int i = 0; i < buffer.Length; i++)
		{
			int index = line_index - i - 1;

			if (index < 0)
				index += buffer.Length;
			
			string current_line = buffer[index];

			if (current_line == null || current_line == "")
				break;

			else if (i == 0)
				display_string = current_line;
			else
				display_string = current_line + "\n" + display_string;
		}

		text.text = display_string + "\n";
		scrollbar.value = 0;
    }
}
