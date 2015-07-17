using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public Button startButton;
	public Button quitButton;
	public Button helpButton;
	public Button backButton;
	public Button creditsButton;
	public GameObject inGameGroup;

	public Text startText;

	public CanvasGroup canvasGroup;

	public CreditsView creditsViewGroup;
	
	// Use this for initialization
	void Start () {
		startButton.onClick.AddListener(StartClick);
		quitButton.onClick.AddListener(QuitClick);
		backButton.onClick.AddListener(BackClick);
		helpButton.onClick.AddListener(HelpClick);
		creditsButton.onClick.AddListener(CreditsClick);
		FadeIn();
	}

	void Update()
	{
		if(Input.GetButtonDown("Cancel"))
		{
			if(canvasGroup.alpha == 0f)
			{
				FadeIn();
			}
			if(canvasGroup.alpha == 1f)
			{
				if(TwineThing.Instance.gameStarted)
					FadeOut();
				else
					Application.Quit();
			}
		}
	}

	public void StartClick()
	{
		TwineThing.Instance.StartGame();
		FadeOut();
	}

	public void QuitClick()
	{
		startText.text = "Start";
		TwineThing.Instance.gameStarted = false;
		Application.Quit();
	}

	public void BackClick()
	{
		FadeOut();
	}

	public void CreditsClick()
	{
		creditsViewGroup.FadeIn();
	}

	public void HelpClick()
	{

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

	public void FadeOut()
	{
		StartCoroutine(FadeAlphaDown(2f));
		startButton.interactable = false;
	}

	public void FadeIn()
	{
		if(TwineThing.Instance.gameStarted){
			startText.text = "Restart";
			inGameGroup.SetActive(true);
		}
		StartCoroutine(FadeAlphaUp(2f));
		startButton.interactable = true;
	}

}
