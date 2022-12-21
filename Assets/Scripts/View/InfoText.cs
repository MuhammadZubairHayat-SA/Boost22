using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoText : MonoBehaviour 
{
	public TextMeshProUGUI infoText1;
	public TextMeshProUGUI infoText2;
	public int languageId = 1;
	public InfoTextMessages[] infoTextMessages;
	public bool showDebugMessages = false;

	private Animator animator;
	private Coroutine textRoutine;
	private int currentlyShown = 0; // 0: none, 1: infoText1, 2: infoText2
	private int currentMessageId = -1;
	


	void Start () 
	{
		animator = GetComponent<Animator>();
	}


	public void ClearAnnouncements (float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announcements cleared" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = -1;
		textRoutine = StartCoroutine(SetTextDelayed(string.Empty, delay));
	}


	public void AnnounceHighAce (float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce High Ace" + ". Current message ID is " + currentMessageId);
		}

		currentMessageId = 0;
		textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].highAceAnnouncement, delay));
	}


	public void AnnounceHandStartingPlayer (string playerName, float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce Hand starting player" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 1;
		textRoutine = StartCoroutine(SetTextDelayed(playerName + " " + infoTextMessages[languageId].playerStartsMessage, delay));
	}


	public void AnnounceSwitchOneCard (float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce switched one card" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 2;
		textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].switchOneCardMessage, delay));
	}


	public void AnnounceSwitchNumberOfCards (int numberOfCards, float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce switched several cards" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 2;
		textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].switchNumberOfCardsMessage + " " + numberOfCards + " " + infoTextMessages[languageId].cardsPlural, delay));
	}


	public void AnnounceReceivedOneCard (float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce received one card" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 3;
		textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].receivedOneCardMessage, delay, 3.0f));
	}


	public void AnnounceReceivedNumberOfCards (int numberOfCards, float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce received several cards" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 3;
		string[] splitString = infoTextMessages[languageId].receivedNumberOfCardsMessage.Split(',');
		textRoutine = StartCoroutine(SetTextDelayed(splitString[0] + " " + numberOfCards + " " + splitString[1], delay, 3.0f));
	}


	public void AnnounceYourTurn (float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce your turn" + ". Current message ID is " + currentMessageId);
		}
		
		if (currentMessageId != 6)
		{
			currentMessageId = 4;
			textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].yourTurnMessage, delay));
		}
	}


	public void AnnouncePlayerWinsRound (string playerName, float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce player wins round" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 5;
		textRoutine = StartCoroutine(SetTextDelayed(playerName + " " + infoTextMessages[languageId].playerWinsRoundMessage, delay, 3.0f));
	}


	public void AnnounceCannotMatchCard (float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce cannot match one card" + ". Current message ID is " + currentMessageId);
		}

		currentMessageId = 6;
		textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].cannotMatchCardMessage, delay));
	}


	public void AnnounceCannotMatchCards (int numberOfCards, float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce cannot match several cards" + ". Current message ID is " + currentMessageId);
		}

		currentMessageId = 6;
		string[] splitString = infoTextMessages[languageId].cannotMatchCardsMessage.Split(',');
		textRoutine = StartCoroutine(SetTextDelayed(splitString[0] + " " + numberOfCards + " " + splitString[1], delay));
	}


	public void AnnounceLastCardsReveal (float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce last cards reveal" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 7;
		textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].lastCardsRevealAnnouncement, delay, 5.0f));
	}


	public void AnnouncePlayerIsExcluded (string playerName, float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce one player is excluded" + ". Current message ID is " + currentMessageId);
		}
		
		currentMessageId = 8;
		textRoutine = StartCoroutine(SetTextDelayed(playerName + " " + infoTextMessages[languageId].playerIsExcludedMessage, delay, 3.0f));
	}


	public void AnnouncePlayersAreExcluded (List<string> playerNames, float delay)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce several players are excluded" + ". Current message ID is " + currentMessageId);
		}

		currentMessageId = 8;
		string[] splitString = infoTextMessages[languageId].playersAreExcludedMessage.Split(',');
		string message = string.Empty;

		for (int i = 0; i < playerNames.Count; i++)
		{
			message += playerNames[i] + " ";
			
			if (i < playerNames.Count - 2)
			{
				message += ", ";
			}
			else if (i < playerNames.Count - 1)
			{
				message += splitString[0] + " ";
			}
		}

		message += splitString[1];

		textRoutine = StartCoroutine(SetTextDelayed(message, delay, 3.0f));
	}


	public void AnnounceCannotSelect (int reasonId)
	{
		if (showDebugMessages)
		{
			Debug.Log("Announce cannot select" + ". Current message ID is " + currentMessageId);
		}

		currentMessageId = 9;
		switch (reasonId)
		{
			case 0:
				textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].maxSelectedReason, 0.0f, 3.0f));
				break;
			case 1:
				textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].lastCardReason, 0.0f, 3.0f));
				break;
			case 2:
				textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].wrongValueReason, 0.0f, 3.0f));
				break;
			case 3:
				textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].tooFewOfValueReason, 0.0f, 3.0f));
				break;
			case 4:
				textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].tooLowValueReason, 0.0f, 3.0f));
				break;
			case 5:
				textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].tooManyReason, 0.0f, 3.0f));
				break;
			default:
				textRoutine = StartCoroutine(SetTextDelayed(infoTextMessages[languageId].noReason, 0.0f, 3.0f));
				break;
		}
	}


	private IEnumerator SetTextDelayed (string newText, float delay, float duration = 0.0f)
	{
		/*/if (showDebugMessages)
		{
			Debug.Log("Currently shown: " + currentlyShown + " showing '" + newText + "'" + ". Current message ID is " + currentMessageId);
		}*/

		yield return new WaitForSeconds(delay);

		/*if (textRoutine != null)
		{
			StopCoroutine(textRoutine);
		}*/

		if (currentlyShown == 1 || currentlyShown == 0)
		{
			animator.SetTrigger("crossfade1");
			infoText2.text = newText;
			ClearText(infoText1);
			currentlyShown = 2;
		}
		else
		{
			animator.SetTrigger("crossfade2");
			infoText1.text = newText;
			ClearText(infoText2);
			currentlyShown = 1;
		}

		if (duration > 0)
		{
			yield return new WaitForSeconds(duration);

			ClearAnnouncements(0.0f);
		}
	}


	private void ClearText (TextMeshProUGUI textToClear)
	{
		//yield return new WaitForSeconds(0.5f);
		textToClear.text = string.Empty;
	}
}
