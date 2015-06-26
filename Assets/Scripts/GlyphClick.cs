using UnityEngine;
using System.Collections;

public class GlyphClick : MonoBehaviour {

	public TwineThing twineThing;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0))
		{

		}
	}

	void OnMouseDown()
	{
		twineThing.SwitchGlyph();
	}
}
