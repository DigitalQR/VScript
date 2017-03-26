using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GraphEntryDialog : MonoBehaviour {

	[SerializeField]
	private Text Entry;

	public void SetName(string name)
	{
		Entry.text = name;
	}

	public void Select()
	{
		GraphSelectDialog.main.Select(Entry.text);
	}

}
