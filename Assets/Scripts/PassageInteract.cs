using UnityEngine;
using System.Collections;
using Candlelight.UI;

public class PassageInteract : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnLinkClick(HyperText hyperText, HyperText.LinkInfo linkInfo)
	{
		Debug.Log ("Clicked on: " + linkInfo.Id);
	}
}
