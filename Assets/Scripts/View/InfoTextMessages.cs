using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTextMessages : MonoBehaviour
{
	public string languageName;

	// General
	public string cardSingular;
	public string cardsPlural;

	// High Ace mode
	public string highAceAnnouncement;
	public string playerStartsMessage;

	// Switch Cards mode
	public string switchOneCardMessage;
	public string switchNumberOfCardsMessage;
	public string receivedOneCardMessage;
	public string receivedNumberOfCardsMessage;
	
	// Play Cards mode
	public string yourTurnMessage;
	public string playerWinsRoundMessage;
	public string cannotMatchCardMessage;
	public string cannotMatchCardsMessage;

	// Last cards reveal
	public string lastCardsRevealAnnouncement;
	public string playerIsExcludedMessage;
	public string playersAreExcludedMessage;

	// Cannot select card reasons
	public string noReason;
	public string maxSelectedReason;
	public string lastCardReason;
	public string wrongValueReason;
	public string tooFewOfValueReason;
	public string tooLowValueReason;
	public string tooManyReason;
}

