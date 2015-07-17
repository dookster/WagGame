using UnityEngine;
using System.Collections;
using Candlelight.UI;
using UnityEngine.UI;

public class CreditsView : MonoBehaviour {

	public CanvasGroup canvasGroup;
	public Button closeButton;

	// Use this for initialization
	void Start () 
	{
		closeButton.onClick.AddListener(FadeOut);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FadeIn()
	{
		StartCoroutine(FadeAlphaUp(2f));
	}

	public void FadeOut()
	{
		StartCoroutine(FadeAlphaDown(2f));
	}

	public void OnLinkClick(HyperText hyperText, HyperText.LinkInfo linkInfo)
	{
		Application.OpenURL(linkInfo.Id);
	}

	IEnumerator FadeAlphaDown(float speed)
	{
		while (canvasGroup.alpha > 0f)
		{
			canvasGroup.alpha -= speed * Time.deltaTime;
			
			yield return null;
		}
		canvasGroup.alpha = 0;
		canvasGroup.blocksRaycasts = false;
	}
	
	IEnumerator FadeAlphaUp(float speed)
	{
		while (canvasGroup.alpha < 1f)
		{
			canvasGroup.alpha += speed * Time.deltaTime;
			
			yield return null;
		}
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
	}

}
