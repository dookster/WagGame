using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Candlelight.UI;

public class TwineThing : MonoBehaviour {

	/*
	 *  Multi-body twine passages. Some words have two different variations. They are ordered on pairs between two (( and )) signs divided by |
	 *  ex:
	 * 
	 *  This is normal text followed by either ((this sentence|that sentence))
	 * 
	 * 	The marked text is colored differently and is changeable while playing.
	 */ 


	public TextAsset TweeFile;
	public HyperText MainHyperText;
	public HyperText HyperTextA;
	public HyperText HyperTextB;

	public Color ColorA;
	public Color ColorB;

	static Color SColorA;
	static Color SColorB;
	
	string tweeText;

	public Dictionary<string, TweePassage> passages = new Dictionary<string, TweePassage>();
		
	[System.Serializable]
	public class TweePassage {
		public string title;
		public string[] tags;
		public string body;
		public string bodyA; // alternative body with some parts different
		public string bodyB; // alternative body with some parts different

		public void SetBodies(string rawText)
		{
			string textWithLinks = TwineToHyper(rawText);

			FindAndSetColoredPassages(textWithLinks, this);
		}

	}
	
	// Use this for initialization
	void Start () {
		SColorA = ColorA;
		SColorB = ColorB;

		tweeText = TweeFile.text;
		Parse();

		TweePassage startPassage = passages["Start"];

		SetUiText(startPassage);
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

		passage.body = mainText;
		passage.bodyA = aText;
		passage.bodyB = bText;
	}
	
	public void OnLinkClick(HyperText hyperText, HyperText.LinkInfo linkInfo)
	{
		//hyperText.text = passages[linkInfo.Id].body;
		SetUiText(passages[linkInfo.Id]);
	}

	/**
	 * Show the text from the given passage in the UI, alt-body set on HyperTextB
	 */
	void SetUiText(TweePassage passage)
	{
		MainHyperText.text = passage.body;
		HyperTextA.text = passage.bodyA;
		HyperTextB.text = passage.bodyB;
	}

	static string ColorToHex(Color color)
	{
		string rgbString = string.Format("#{0:X2}{1:X2}{2:X2}", 
		                          (int)(color.r * 255), 
		                          (int)(color.g * 255), 
		                          (int)(color.b * 255));

		Debug.Log("Color: " + rgbString);
		return rgbString;
	}

}
