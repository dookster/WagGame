using UnityEngine;
using System.Collections;
using Candlelight.UI;

public class TextSwitcher : MonoBehaviour {

	public TwineThing twineThing;

	public Transform HingeA;
	public Transform HingeB;

	public HyperText HyperTextA;
	public HyperText HyperTextB;

	public AudioClip woosh;

	HyperText activeText;

	void Start()
	{
		activeText = HyperTextA;
		HyperTextB.CrossFadeAlpha(0, 0, true);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			if(twineThing.blockInput) return;

			AudioSource.PlayClipAtPoint(woosh, transform.position);
			//iTween.RotateTo(HingeA.gameObject, iTween.Hash("x", 180, "time", 0.75f, "islocal", false, "oncomplete", "Rotated", "oncompletetarget", gameObject));
			//iTween.RotateTo(HingeB.gameObject, iTween.Hash("x", 180, "time", 0.75f, "islocal", false));
			//iTween.ValueTo(HyperTextA.gameObject, iTween.Hash("from", 1, "to", 0, "time", 1, "onupdate", "SetAlpha", "onupdatetarget", HyperTextA.canvasRenderer.gameObject));
			//iTween.ValueTo(HyperTextB.gameObject, iTween.Hash("from", 1, "to", 0, "time", 1, "onupdate", "OnFade", "onupdatetarget", gameObject));

			if (activeText == HyperTextA)
			{
				HyperTextA.CrossFadeAlpha(0, 0.5f, false);
				HyperTextB.CrossFadeAlpha(1, 1, false);
				activeText = HyperTextB;
			}
			else 
			{
				HyperTextA.CrossFadeAlpha(1, 1, false);
				HyperTextB.CrossFadeAlpha(0, 0.5f, false);
				activeText = HyperTextA;
			}

		}
	}

	void Fade(float alpha)
	{

		//text.canvasRenderer.SetAlpha(alpha);
		//HyperTextB.canvasRenderer.SetAlpha(alpha);
		//HyperTextB.color = new Color(HyperTextB.color.r, HyperTextB.color.g, HyperTextB.color.b, alpha);
	}

	void Rotated()
	{
		if(activeText == HyperTextA)
		{
			activeText = HyperTextB;
		}
		else
		{
			activeText = HyperTextA;
		}
	}
}
