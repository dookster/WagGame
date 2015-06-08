using UnityEngine;
using System.Collections;
using Candlelight.UI;
using UnityEngine.UI;

public class HyperColorAnimate : MonoBehaviour {

	public HyperText hyperText;

	private float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		//hyperText.Styles.DefaultLinkStyle.Colors.normalColor = Color.Lerp(Color.blue, Color.yellow, (Time.time - startTime) / 10);
		ColorBlock colorBlock = hyperText.Styles.DefaultLinkStyle.Colors;
		colorBlock.normalColor = Color.Lerp(Color.blue, Color.yellow, (Time.time - startTime) / 10);
//		hyperText.Styles.DefaultLinkStyle.Colors = colorBlock;



		//colorBlock.normalColor = Color.Lerp(Color.blue, Color.yellow, (Time.time - startTime) / 10);
		//hyperText.Styles.DefaultLinkStyle.Colors.;

		//HyperTextStyles.LinkSubclass
		//hyperText.Styles.SetLinkStyles
	}


}
