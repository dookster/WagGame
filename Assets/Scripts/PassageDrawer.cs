using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PassageDrawer : MonoBehaviour {

	public GameObject textPrefab;

	public RectTransform panel;

	private List<Text> textViews = new List<Text>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for(int n = 1 ; n < textViews.Count ; n++)
		{
			RectTransform prevView = textViews[n-1].rectTransform;
			Vector2 newPos = new Vector2(prevView.rect.width + prevView.localPosition.x, prevView.localPosition.y);
			textViews[n].rectTransform.localPosition = newPos;
		}
	}

	public void SetPassage(TwineThing.TweePassage passage)
	{
		List<string> words = new List<string>();

		//words.AddRange(passage.body.Split(' '));
		string test = "Vildere klovne test. En hel masse tekst og også lidt\n tegn tegn og tegn 13 2 1 og tal øå' ?# osv ... .";
		test = test.Replace("\n", "");
		test = test.Replace("  ", " ");
		words.AddRange(test.Split(' '));

		foreach(string str in words)
		{
			GameObject go = Instantiate(textPrefab);
			Text textView = go.GetComponent<Text>();
			textView.text = str + "_";
			textView.rectTransform.SetParent(panel, false);
			textViews.Add(textView);
		}
	}

}
