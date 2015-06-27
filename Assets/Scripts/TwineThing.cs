using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Candlelight.UI;
using System.Collections;

public class TwineThing : MonoBehaviour {

	/*
	 *  Multi-body twine passages. Some words have two different variations. They are ordered on pairs between two (( and )) signs divided by |
	 *  ex:
	 * 
	 *  This is normal text followed by either ((this sentence|that sentence))
	 * 
	 * 	The marked text is colored differently and is changeable while playing.
	 */ 

	public enum PassageType {A, B};

	private static TwineThing instance = null;
	public static TwineThing Instance
	{
		get
		{
			if(instance == null)
			{
				instance = Object.FindObjectOfType<TwineThing>();
			}
			return instance;
		}
	}

	public TextAsset TweeFile;
	public HyperText MainHyperText;
	public HyperText HyperTextA;
	public HyperText HyperTextB;

	public Color ColorA;
	public Color ColorB;

	static Color SColorA;
	static Color SColorB;

	public bool blockInput = false;

	public AudioClip woosh;
	public AudioClip click;

	public GameObject inputGlyph;

	private TweePassage currentPassage;

	public Menu menu;

	public bool gameStarted;

	// Keeping track of which version was selected of each passage
	// <passageName, passagetype> 
	public Dictionary<string, PassageType> passageResults = new Dictionary<string, PassageType>();

	HyperText activeText;
	string tweeText;

	public Dictionary<string, TweePassage> passages = new Dictionary<string, TweePassage>();
		
	[System.Serializable]
	public class TweePassage 
	{
		public string title;
		public string[] tags;
		private string body;
		private string bodyA; // alternative body with some parts different
		private string bodyB; // alternative body with some parts different

		public string Body { set { body = value; } get { return SplitBodyInSubVersion(body); } }
		public string BodyA { set { bodyA = value; } get { return SplitBodyInSubVersion(bodyA); } }
		public string BodyB { set { bodyB = value; } get { return SplitBodyInSubVersion(bodyB); } }

		/**
		 * Which text to show depends on earlier choices, split given body string up to match
		 * 
		 */
		string SplitBodyInSubVersion(string rawBody)
		{
			string result = rawBody;

			while(result.Contains("(%"))
			{
				int startI = result.IndexOf ("(%");
				int endI = result.IndexOf("%)");
				int nextSpace = result.IndexOf(" ", startI);

				string valKey = result.Substring(startI+2, nextSpace-startI-2);
				string val = valKey.Substring(0,1);
				string key = valKey.Substring(1);

				Debug.Log("Key: " + key + " Val: " + TwineThing.Instance.passageResults[key]);

				result = result.Replace("(%", "");
			}
			return result;
		}

		public void SetBodies(string rawText)
		{
			//rawText = rawText.Substring(0, rawText.Length-5);
//			while(rawText[rawText.Length-1] == '\n')
//			{
//				Debug.Log ("Removing break, " + rawText.Length);
//				rawText = rawText.Remove(rawText.Length-1);
//			}
			string textWithLinks = TwineToHyper(rawText);
			FindAndSetColoredPassages(textWithLinks, this);
		}

	}
	
	// Use this for initialization
	void Start () 
	{
		SColorA = ColorA;
		SColorB = ColorB;

		tweeText = TweeFile.text;
		Parse();
	}

	void Update()
	{


		if(Input.GetKeyDown(KeyCode.Tab))
		{
			if(menu.canvasGroup.alpha < 1)
				SwitchGlyph();
		}
	}

	public void StartGame()
	{
		TweePassage startPassage = passages["Start"];
		currentPassage = startPassage;
		passageResults.Clear();

		if(activeText == HyperTextB)
		{
			HyperTextA.CrossFadeAlpha(1, 1, false);
			HyperTextB.CrossFadeAlpha(0, 0.5f, false);
			RotateGlyphDown();
		}

		//SetUiText(startPassage);
		activeText = HyperTextA;
		StartCoroutine(SwitchToPassage(startPassage, 0.5f));

		HyperTextB.CrossFadeAlpha(0, 0, true);
		gameStarted = true;
	}

	public void SwitchGlyph()
	{
		if(blockInput) return;
		
		AudioSource.PlayClipAtPoint(woosh, transform.position);
		
		if (activeText == HyperTextA)
		{
			HyperTextA.CrossFadeAlpha(0, 0.5f, false);
			HyperTextB.CrossFadeAlpha(1, 1, false);
			activeText = HyperTextB;
			RotateGlyphUp();
		}
		else 
		{
			HyperTextA.CrossFadeAlpha(1, 1, false);
			HyperTextB.CrossFadeAlpha(0, 0.5f, false);
			activeText = HyperTextA;
			RotateGlyphDown();
		}
	}

	void RotateGlyphUp()
	{
		iTween.RotateTo(inputGlyph, iTween.Hash("x", 90, "time", 0.75f));
	}

	void RotateGlyphDown()
	{
		iTween.RotateTo(inputGlyph, iTween.Hash("x", 0, "time", 0.75f));
	}

	private void Parse () {
		TweePassage currentPassage = null;
		
		// Buffer to hold the content of the current passage while we build it
		StringBuilder buffer = new StringBuilder();
		
		// Array that will hold all of the individual lines in the twee source
		string[] lines; 
		
		// Utility array used in various instances where a string needs to be split up
		string[] chunks;

		// Split the twee source into lines so we can make sense of it while parsing
		lines = tweeText.Split(new string[] {"\n"}, System.StringSplitOptions.None);
		
		for (long i = 0; i < lines.LongLength; i++) {
			
			// If a line begins with "::" that means a new passage has started
			if (lines[i].StartsWith("::")) {
				
				// If we were already building a passage, that one is done. Add it and get ready for a new
				if (currentPassage != null) {
					//currentPassage.body = TwineToHyper(buffer.ToString());
					currentPassage.SetBodies(buffer.ToString());
					passages.Add(currentPassage.title, currentPassage);
					buffer = new StringBuilder();
				}
				
				/* A new passage in a twee starts with a line like this:
	             *
	             * :: The Passage Begins Here [someTag anotherTag heyThere]
	             *               
	             * What's happening here is when a new passage starts, we ignore the
	             * :: prefix, strip off the ] at the end of the tags, and split the
	             * line on [ into two strings, one of which will be the passage title
	             * while the other has all of the passage's tags, if any are found.
	             */
				chunks = lines[i].Substring(2).Replace ("]", "").Split ('[');
				
				// We should always have at least a passage title, so we can
				// start a new passage here with that title.
				currentPassage = new TweePassage();
				currentPassage.title = chunks[0].Trim();
				
				// If there was anything after the [, the passage has tags, so just
				// split them up and attach them to the passage.
				if (chunks.Length > 1) {
					currentPassage.tags = chunks[1].Trim().Split(' ');  
				}
				
			} else if (currentPassage != null) {
				
				// If we didn't start a new passage, we're still in the previous one,
				// so just append this line to the current passage's buffer.
				buffer.AppendLine(lines[i]);    
			}
		}

		// When we hit the end of the file, we should still have the last passage in
		// the file in the buffer. Wrap it up and end it as well.
		if (currentPassage != null) {
			//currentPassage.body = TwineToHyper(buffer.ToString());
			currentPassage.SetBodies(buffer.ToString());
			passages.Add(currentPassage.title, currentPassage);
		}
		
	}
	
	/**
	 * Convert all links in given passage to hyperlinks
	 */
	public static string TwineToHyper(string bodyText)
	{
		while(bodyText.Contains("[["))
		{
			int startB = bodyText.IndexOf("[[");
			int endB = bodyText.IndexOf("]]") + 2;
			string link = bodyText.Substring(startB, endB - startB);
			bodyText = bodyText.Replace(link, TwineLinkToHyper(link));
		}
		return bodyText;
	}

	/**
	 * Convert a given [[...|...]] link to a hyperlink
	 */
	private static string TwineLinkToHyper(string bodyText)
	{
		bodyText = bodyText.Replace("[[", "");
		bodyText = bodyText.Replace("]]", "");
		string[] split = bodyText.Split('|');

		return "<a name=\"" + split[1] + "\">" + split[0] + "</a>";
	}

	/**
	 * Find the colored passages, replace with spaces in body and with
	 * colored versions in bodyA and bodyB
	 */
	public static void FindAndSetColoredPassages(string rawText, TweePassage passage)
	{
		string mainText = rawText;
		string aText = rawText;
		string bText = rawText;

		while(rawText.Contains("(("))
		{
			int startB = rawText.IndexOf("((");
			int endB = rawText.IndexOf("))") + 2;

			string colorPassage = rawText.Substring(startB, endB - startB);

			string[] variants = colorPassage.Split('|');
			variants[0] = variants[0].Replace("((", "").Replace("))", "");
			variants[1] = variants[1].Replace("((", "").Replace("))", "");

			rawText = rawText.Replace(colorPassage, "<color=#0000>" + variants[0] + "</Color>"); // Colored passages are invisible in the main text

			mainText = rawText;
			aText = aText.Replace(colorPassage, "<color=" + ColorToHex(SColorA) + ">" + variants[0] + "</Color>");
			bText = bText.Replace(colorPassage, "<color=" + ColorToHex(SColorB) + ">" + variants[1] + "</Color>");
		}

		passage.Body = mainText;
		passage.BodyA = aText;
		passage.BodyB = bText;
	}
	
	public void OnLinkClick(HyperText hyperText, HyperText.LinkInfo linkInfo)
	{
		//hyperText.text = passages[linkInfo.Id].body;
		//SetUiText(passages[linkInfo.Id]);
		StartCoroutine(SwitchToPassage(passages[linkInfo.Id], 0.25f));
	}

	/**
	 * Show the text from the given passage in the UI, alt-body set on HyperTextB
	 */
	void SetUiText(TweePassage passage)
	{
		MainHyperText.text = passage.Body;
		HyperTextA.text = passage.BodyA;
		HyperTextB.text = passage.BodyB;
	}

	IEnumerator SwitchToPassage(TweePassage passage, float time)
	{
		blockInput = true;
	
		AudioSource.PlayClipAtPoint(click, transform.position);

		MainHyperText.CrossFadeAlpha(0, time, false);
		if(activeText == HyperTextA)
		{
			if(!passageResults.ContainsKey(currentPassage.title)) passageResults.Add(currentPassage.title, PassageType.A);
			HyperTextA.CrossFadeAlpha(0, time, false);
		}
		else
		{
			if(!passageResults.ContainsKey(currentPassage.title)) passageResults.Add(currentPassage.title, PassageType.B);
			HyperTextB.CrossFadeAlpha(0, time, false);
		}

		yield return new WaitForSeconds(time);

		SetUiText(passage);

		MainHyperText.CrossFadeAlpha(1, time, false);
		if(activeText == HyperTextA)
		{
			HyperTextA.CrossFadeAlpha(1, time, false);
		}
		else 
		{
			HyperTextB.CrossFadeAlpha(1, time, false);
		}

		currentPassage = passage;
		blockInput = false;
	}


	static string ColorToHex(Color color)
	{
		string rgbString = string.Format("#{0:X2}{1:X2}{2:X2}", 
		                          (int)(color.r * 255), 
		                          (int)(color.g * 255), 
		                          (int)(color.b * 255));

		//Debug.Log("Color: " + rgbString);
		return rgbString;
	}

}
