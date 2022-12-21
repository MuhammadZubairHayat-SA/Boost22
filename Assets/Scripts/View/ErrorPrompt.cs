using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorPrompt : MonoBehaviour
{
	public TextMeshProUGUI message;
	private Animator anim;
	private bool isOpen = false;

	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Show()
	{
		StartCoroutine(ShowDelayed());
	}


	public void Hide ()
	{
		if (isOpen)
		{
			isOpen = false;
			anim.Play("LobbyPrompt_close");
		}
	}

	public void TappedCancel()
	{
		isOpen = false;
		anim.Play("LobbyPrompt_close");
	}

	public void TappedTryAgain()
	{
		GameManager.shared.SendPoints();
	}


	private IEnumerator ShowDelayed ()
	{
		yield return new WaitForSeconds(Delays.clearTableDelay + Delays.revealLastCardsDelay + Delays.excludeCardsDelay);
		isOpen = true;
		anim.Play("LobbyPrompt_open");
	}
}
