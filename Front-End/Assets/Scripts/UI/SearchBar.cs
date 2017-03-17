using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VScript_Core.Graphing;
using UnityEngine.UI;

public class SearchBar : MonoBehaviour {

	[SerializeField]
	private GameObject SearchPanel;
	[SerializeField]
	private InputField SearchField;

	[SerializeField]
	private SearchResult Result;
	private List<SearchResult> Results;
	private bool Initialised = false;

	void Update()
	{
		if (!Initialised && VScript_Core.VScriptEngine.initialised)
		{
			UpdateSearch("");
			Initialised = true;
        }
    }

	public void OnKeywordChange()
	{
		UpdateSearch(SearchField.text);
    }

	void UpdateSearch(string keyword)
	{
		if (Results != null)
		{
			foreach (SearchResult result in Results)
				Destroy(result.gameObject);
			Results.Clear();
		}
		else
			Results = new List<SearchResult>();

		List<Node> nodes = Library.main.Search(keyword);

		foreach (Node node in nodes)
		{
			//Ignore null node
			if (node.id == 0 && node.module_id == 0)
				continue;
			//Ignore start node
			if (node.id == 1 && node.module_id == 0)
				continue;

			SearchResult result = Instantiate(Result, SearchPanel.transform);
			result.transform.localScale = new Vector3(1, 1, 1);
            result.Build(node);
			Results.Add(result);
        }
    }
}
