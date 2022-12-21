using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour 
{
	public List<CardAnimator> cardAnimators;
	public AudioClip unfoldSound;

	private CardsLayoutGroup layoutGroup;
	private Animator animator;
	private CanvasGroup canvasGroup;
	private AudioSource audioSource;
	private float fadeTimer = 0.0f;
	

	void Start () 
	{
		layoutGroup = GetComponent<CardsLayoutGroup>();
		animator = GetComponent<Animator>();
		canvasGroup = GetComponent<CanvasGroup>();
		audioSource = GetComponent<AudioSource>();
	}
	

	void Update () 
	{
		/*
		if (canvasGroup.interactable == true && fadeTimer < 1)
		{
			fadeTimer += Time.deltaTime * 3;
			Color fadeColor = Color.black;
			fadeColor.a = Mathf.Lerp(0.5f, 0.0f, fadeTimer);

			foreach (CardAnimator cardAnimator in cardAnimators)
			{
				cardAnimator.fadeImage.color = fadeColor;
			}
		}
		else if (canvasGroup.interactable == false && fadeTimer > 0)
		{
			fadeTimer -= Time.deltaTime * 3;

			Color fadeColor = Color.black;
			fadeColor.a = Mathf.Lerp(0.5f, 0.0f, fadeTimer);

			foreach (CardAnimator cardAnimator in cardAnimators)
			{
				cardAnimator.fadeImage.color = fadeColor;
			}
		}
		*/
	}


	public IEnumerator SetCanvasGroupInteractable (bool interactable, float delay)
	{
		yield return new WaitForSeconds(delay);
		
		canvasGroup.interactable = interactable;

		foreach (CardAnimator cardAnimator in cardAnimators)
		{
			if (interactable == false)
			{
				cardAnimator.FadeOut();
			}
			else
			{
				cardAnimator.FadeIn();
			}
		}
	}


	public void ShowHand ()
	{
		Color fadeColor = Color.black;
		fadeColor.a = 0.5f;

		foreach (CardAnimator cardAnimator in cardAnimators)
		{
			cardAnimator.FadeOut();
			cardAnimator.gameObject.SetActive(true);
			cardAnimator.Appear();
		}

		StartCoroutine(DelayedUnfold());
	}


	public void SwitchedCards (List<int> indexes)
	{
		for (int i = 0; i < cardAnimators.Count; i++)
		{
			if (indexes.Contains(i))
			{
				StartCoroutine(cardAnimators[i].SwitchIn());
			}
			else
			{
				cardAnimators[i].Reset();
			}
		}
	}


	public List<CardAnimator> GetSelectedCardAnimators ()
	{
		List<CardAnimator> selectedCardAnimators = new List<CardAnimator>();

		foreach (CardAnimator cardAnimator in cardAnimators)
		{
			if (cardAnimator.isSelected)
			{
				selectedCardAnimators.Add(cardAnimator);
			}
		}

		return selectedCardAnimators;
	}


	public void FadeOut ()
	{
		animator.ResetTrigger("unfold");
		animator.ResetTrigger("fold");
		animator.SetTrigger("fadeOut");
	}


	private IEnumerator DelayedUnfold ()
	{
		yield return new WaitForSeconds(0.15f);
		layoutGroup.ResizeSmooth();
		audioSource.clip = unfoldSound;
		audioSource.Play();
	}
}
