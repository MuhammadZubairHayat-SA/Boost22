using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonView : MonoBehaviour 
{
	public AudioClip yourTurnSound;
	public CanvasGroup lobbyButton;
	public TextMeshProUGUI playAgainText;

	private Animator animator;
	private AudioSource audioSource;


	void Start () 
	{
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
	}
	

	void Update () 
	{
		
	}


	public IEnumerator SetButtonsActive (bool enable, float delay)
	{
		yield return new WaitForSeconds(delay);
		
		animator.SetBool("isActive", enable);
	}


	public IEnumerator ChangeButtons (int newButtonIndex, float delay)
	{
		yield return new WaitForSeconds(delay);

		switch (newButtonIndex)
		{
			case 0: // No swap
				animator.ResetTrigger("switchCards");
				animator.ResetTrigger("playCards");
				animator.SetTrigger("noSwitch");
				break;
			case 1: // Swap cards
				animator.ResetTrigger("noSwitch");
				animator.ResetTrigger("playCards");
				animator.SetTrigger("switchCards");
				break;
			case 2: // Play cards (not flashing)
				animator.ResetTrigger("noSwitch");
				animator.ResetTrigger("switchCards");
				animator.SetBool("flash", false);
				animator.SetTrigger("playCards");
				break;
			case 3: // Play cards (flashing)
                animator.ResetTrigger("noSwitch");
				animator.ResetTrigger("switchCards");
				animator.SetBool("flash", true);
				animator.SetTrigger("playCards");
				break;
		}
	}


	public void NoSwitch ()
	{
		GameManager.shared.PlayerButtonTapped(0);
	}


	public void SwitchCards ()
	{
		GameManager.shared.PlayerButtonTapped(1);
	}


	public void PlayCards ()
	{
		GameManager.shared.PlayerButtonTapped(2);
	}


	public IEnumerator PlayNotificationSound (float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);
		audioSource.clip = yourTurnSound;
		audioSource.Play();
	}


	public IEnumerator GameOver (float delay, bool isRushGame)
	{	
		Debug.Log("Buttons: Game over (" + delay +")");
		if (isRushGame)
		{
			playAgainText.text = "A new game will start momentarily...";
		}

		lobbyButton.interactable = false;
		//lobbyButton.alpha = 0.3f;
		yield return new WaitForSeconds(delay);
		animator.SetTrigger("gameOver");
	}


	public void DoneSaving ()
	{
		lobbyButton.interactable = true;
		lobbyButton.alpha = 1.0f;
		animator.SetBool("playAgain", true);
	}


	public void FadeOut ()
	{
		animator.ResetTrigger("noSwitch");
		animator.ResetTrigger("switchCards");
		animator.ResetTrigger("playCards");
		animator.ResetTrigger("gameOver");
		animator.SetBool("fadeIn", false);
		animator.SetBool("flash", false);
		animator.SetBool("isActive", false);
		animator.SetBool("playAgain", false);
		
		animator.SetTrigger("fadeOut");
	}


	public void FadeIn ()
	{
		animator.SetBool("fadeIn", true);
	}
}
