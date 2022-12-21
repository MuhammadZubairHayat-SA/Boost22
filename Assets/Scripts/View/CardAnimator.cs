using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardAnimator : MonoBehaviour 
{
	public Image suitImage;
	public AudioClip selectSound;
	public AudioClip deselectSound;
	public AudioClip playSound;
	public AudioClip revealSound;
	public AudioClip revealRemoveSound;
	public TextMeshProUGUI valueText;
	public bool isSelected = false;
	public float highlightSpeed = 4.0f;
	public float highlightScale = 1.1f;
	public int playedByPlayerIndex;

	private Image fadeImage;
	private Button button;
	private Animator animator;
	private Image highlightImage;
	private AudioSource audioSource;
	private RectTransform rectParent;
	private CanvasGroup canvasGroup;
	private Vector3 tableCenterPoint;
	private Vector3 highAcePosition;
	private Vector2 targetPosition;
	private Vector2 startPosition;
	private float highlightTimer = 0.0f;
	private float fadeTimer = 0.0f;
	private bool highlight = false; 
	private bool fade = false;
	private float dealTimer = 0.0f;
	private bool dealHighAce = false;
	private float moveTimer = 0.0f;
	private bool move = false;
	private bool cardCanBeDeselected = true;
	private bool cardCanBeSelected = true;


	void Start () 
	{
		button = GetComponent<Button>();
		highAcePosition = transform.position;
		animator = GetComponent<Animator>();
		highlightImage = transform.Find("Highlight").GetComponent<Image>();
		fadeImage = transform.Find("Fade").GetComponent<Image>();
		audioSource = GetComponent<AudioSource>();
		StartCoroutine(SetCanvasGroupVisible());
	}
	

	void Update () 
	{
		if (highlight == true && highlightTimer < 1)
		{
			highlightTimer += Time.deltaTime * highlightSpeed;

			Color highlightColor = highlightImage.color;
			highlightColor.a = highlightTimer;
			highlightImage.color = highlightColor;

			transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * highlightScale, highlightTimer);
		}
		else if (highlight == false && highlightTimer > 0)
		{
			highlightTimer -= Time.deltaTime * highlightSpeed;

			Color highlightColor = highlightImage.color;
			highlightColor.a = highlightTimer;
			highlightImage.color = highlightColor;

			transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * highlightScale, highlightTimer);
		}
		else if (highlight == true && highlightTimer >= 1)
		{
			highlight = false;
		}


		if (fade == true && fadeTimer < 1)
		{
			fadeTimer += Time.deltaTime * highlightSpeed;

			Color fadeColor = fadeImage.color;
			fadeColor.a = Mathf.Lerp(0.0f, 0.5f, fadeTimer);
			fadeImage.color = fadeColor;
		}
		else if (fade == false && fadeTimer > 0)
		{
			fadeTimer -= Time.deltaTime * highlightSpeed;

			Color fadeColor = fadeImage.color;
			fadeColor.a = Mathf.Lerp(0.0f, 0.5f, fadeTimer);
			fadeImage.color = fadeColor;
		}

		if (dealHighAce == true && dealTimer < 1)
		{
			dealTimer += Time.deltaTime;

			transform.parent.position = Vector3.Lerp(tableCenterPoint, highAcePosition, dealTimer);
		}

		if (move == true && moveTimer < 1)
		{
			moveTimer += Time.deltaTime;

			rectParent.anchoredPosition = Vector3.Lerp(rectParent.anchoredPosition, targetPosition, moveTimer);
			rectParent.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.9f, moveTimer);
		}
	}


	public void SetSuitAndValue (Sprite suitSprite, Color suitColor, string value)
	{
		suitImage.sprite = suitSprite;
		suitImage.color = suitColor;
		valueText.text = value;
		valueText.color = suitColor;
	}


	public void UpdateSelectionColor ()
	{
		Card card = GameManager.shared.GetCardFromIndex(null, transform.GetSiblingIndex());
		ColorBlock colors = button.colors;
		cardCanBeSelected = GameManager.shared.CanSelectCard(card);

		if (cardCanBeSelected || isSelected && cardCanBeDeselected )
		{
			colors.pressedColor = Color.grey;
		}
		else
		{
			colors.pressedColor = GameManager.shared.gameViewController.redSuitColor;
		}

		button.colors = colors;
	}


	public void Clicked ()
	{
		Card card = GameManager.shared.GetCardFromIndex(null, transform.GetSiblingIndex());
		if (isSelected == false)
		{
			cardCanBeSelected = GameManager.shared.CanSelectCard(card, true);
		}
		else
		{
			GameManager.shared.DeselectOtherCards(card);
		}
		
		SelectDeselect();
	}


	public IEnumerator Select (bool canDeselect = true, float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);
		
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
		
		//Card card = GameManager.shared.GetCardFromIndex(null, transform.GetSiblingIndex());

		cardCanBeDeselected = canDeselect;
		isSelected = true;
		//card.isSelected = true;

		animator.ResetTrigger("reset");
		animator.ResetTrigger("select");
		animator.ResetTrigger("deselect");
		
		animator.SetTrigger("select");
		
		GameManager.shared.UpdateButtonState();

		audioSource.clip = selectSound;
		audioSource.Play();
	}


	public void Deselect ()
	{
		if (cardCanBeDeselected)
		{
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			
			Card card = GameManager.shared.GetCardFromIndex(null, transform.GetSiblingIndex());

			animator.ResetTrigger("reset");
			animator.ResetTrigger("select");
			animator.ResetTrigger("deselect");

			isSelected = false;
			card.isSelected = false;
			animator.SetTrigger("deselect");

			GameManager.shared.UpdateButtonState();

			audioSource.clip = deselectSound;
			audioSource.Play();
		}
	}
	
	
	public void Reset ()
	{
		if (cardCanBeDeselected)
		{
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			
			Card card = GameManager.shared.GetCardFromIndex(null, transform.GetSiblingIndex());

			animator.ResetTrigger("reset");
			animator.ResetTrigger("select");
			animator.ResetTrigger("deselect");

			isSelected = false;
			card.isSelected = false;
			animator.SetTrigger("reset");

			GameManager.shared.UpdateButtonState();

			audioSource.clip = deselectSound;
			audioSource.Play();
		}
	}


	public IEnumerator SwitchIn ()
	{
		yield return new WaitForSeconds(0.5f);

		Card card = GameManager.shared.GetCardFromIndex(null, transform.GetSiblingIndex());

		isSelected = false;
		card.isSelected = false;
		animator.SetTrigger("switchIn");
	}


	public void Highlight (float delay = 0.0f)
	{
		highlight = true;
		highlightTimer = 0.0f - delay;
	}
	

	public void HighlightRed (float delay = 0.0f)
	{
		if (highlightImage == null)
		{
			highlightImage = transform.Find("Highlight").GetComponent<Image>();
		}

		highlightImage.color = GameManager.shared.gameViewController.redSuitColor * new Color(1, 1, 1, 0);
		Highlight(delay);
	}


	public IEnumerator DelayedHighlight ()
	{
		yield return new WaitForSeconds(Delays.highAceDealDelay + Delays.highAceHighlightDelay);
		highlight = true;
		highlightTimer = 0.0f;
	}


	public void FadeOut ()
	{
		fade = true;
		fadeTimer = 0.0f;
	}


	public void FadeIn ()
	{
		fade = false;
		fadeTimer = 1.0f;
	}


	public void Appear ()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}

		animator.SetTrigger("appear");
	}


	public IEnumerator Reveal (bool isTurboGame, float delay)
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}

		FadeIn();
		animator.speed = Delays.GetGameTempo();
		animator.ResetTrigger("reset");
		animator.ResetTrigger("select");
		animator.ResetTrigger("deselect");
			
		yield return new WaitForSeconds(delay);

		animator.SetBool("turbo", isTurboGame);
		animator.SetTrigger("reveal");

		if (isTurboGame == false)
		{
			audioSource.clip = revealRemoveSound;
			audioSource.pitch = Delays.GetGameTempo();
			audioSource.Play();
			yield return new WaitForSeconds(8.0f * (1 / Delays.GetGameTempo()));
			DestroyImmediate(this.transform.parent.gameObject);
		}
		else
		{
			audioSource.clip = revealSound;
			audioSource.Play();
		}
	}


	public IEnumerator Remove (float delay)
	{	
		yield return new WaitForSeconds(delay);

		if (animator != null)
		{
			animator.speed = Delays.GetGameTempo();
			animator.SetTrigger("remove");
		}

		yield return new WaitForSeconds(0.5f * (1 / Delays.GetGameTempo()));

		if (this != null)
		{
			Destroy(this.transform.parent.gameObject);
			Destroy(this.gameObject);
		}
	}


	public IEnumerator DealHighAce (Transform tableCenter)
	{
		tableCenterPoint = tableCenter.position;
		yield return new WaitForSeconds(Delays.highAceDealDelay);	
		Appear();
	}


	public IEnumerator MoveToPositionOnTable (Vector2 positionOnTable, float yOffset, float delay = 0.0f)
	{
		cardCanBeSelected = false;
		cardCanBeDeselected = false;
		if (button == null)
		{
			button = GetComponent<Button>();
		}
		button.interactable = false;

		yield return new WaitForSeconds(delay);
		
		gameObject.SetActive(true);
		RectTransform rectTransform = GetComponent<RectTransform>();
		startPosition = rectTransform.anchoredPosition;
		rectParent = new GameObject("CardAnimatorParent").AddComponent<RectTransform>();
		rectParent.SetParent(transform.parent);
		rectParent.anchoredPosition = rectTransform.anchoredPosition;
		rectParent.localScale = Vector3.one;
		rectTransform.localScale = Vector3.one;
		transform.SetParent(rectParent);
		
		targetPosition = positionOnTable + Vector2.up * yOffset;
		moveTimer = 0;
		move = true;

		// Forcing update of text mesh fixes issue where text is not displayed after cloning
		//valueText.SetText(valueText.text);
		valueText.text = valueText.text;

		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
		audioSource.clip = playSound;
		audioSource.Play();
	}


	private void SelectDeselect ()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}

		Card card = GameManager.shared.GetCardFromIndex(null, transform.GetSiblingIndex());
		
		animator.ResetTrigger("reset");
		animator.ResetTrigger("select");
		animator.ResetTrigger("deselect");

		if (isSelected == false && cardCanBeSelected)
		{
			GameManager.shared.SelectOtherCardsOfType(card);
			GameManager.shared.SelectOtherBluffCards(card);
			isSelected = true;
			card.isSelected = true;
			animator.SetTrigger("select");
			audioSource.clip = selectSound;
			audioSource.Play();
		}
		else if (isSelected && cardCanBeDeselected)
		{
			isSelected = false;
			card.isSelected = false;
			animator.SetTrigger("deselect");
			audioSource.clip = deselectSound;
			audioSource.Play();
		}

		GameManager.shared.UpdateButtonState();
		GameManager.shared.gameViewController.UpdateCardsSelectionColor();
	}


	private IEnumerator SetCanvasGroupVisible ()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		yield return new WaitForSeconds(0.1f);

		canvasGroup.alpha = 1;
	}
}
