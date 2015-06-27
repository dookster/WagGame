using UnityEngine;
using System.Collections;

public class GlyphClick : MonoBehaviour {

	public TwineThing twineThing;
	public Menu menu;

	void OnMouseDown()
	{
		if(menu.canvasGroup.alpha < 1)
			twineThing.SwitchGlyph();
	}
}
