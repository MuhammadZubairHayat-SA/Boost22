using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Delays 
{
	public static float highAceDealDelay = 0.0f;
	public static float highAceHighlightDelay = 0.0f;
	public static float starterPieceDelay = 2.0f;
	public static float switchCardsModeDelay = 5.0f;
	public static float showHandInteractionDelay = 1.5f;
	public static float yourTurnInteractionDelay = 0.0f;
	public static float selectLowestCardsDelay = 0.0f;
	public static float opponentCardsDelay = 1.7f;
	public static float clearTableDelay = 4.0f;
	public static float newRoundDelay = 0.0f;
	public static float revealLastCardsDelay = 5.0f;
	public static float newHandDelay = 3.0f;
	public static float excludeCardsDelay = 3.5f;
	public static float excludePlayersDelay = 4.0f;
	public static float playerCrossOutDelay = 3.0f;
	public static float wonGameScoreboardDelay = 4.0f;
	public static float lostGameScoreboardDelay = 6.0f;

	private static float gameTempo = 1.0f;
	private static float defaultStarterPieceDelay;
	private static float defaultSwitchCardsModeDelay;
	private static float defaultShowHandInteractionDelay;
	private static float defaultYourTurnInteractionDelay;
	private static float defaultOpponentCardsDelay;
	private static float defaultClearTableDelay;
	private static float defaultRevealLastCardsDelay;
	private static float defaultExcludeCardsDelay;
	private static float defaultExcludePlayersDelay;
	private static float defaultNewHandDelay;
	private static float defaultWonGameScoreboardDelay;
	private static float defaultLostGameScoreboardDelay;


	public static void SetGameTempo (float newGameTempo)
	{
		gameTempo = newGameTempo;
	}


	public static void ResetGameTempo ()
	{
		if (PlayerPrefs.HasKey("GameTempo"))
		{
			gameTempo = PlayerPrefs.GetFloat("GameTempo");
		}
		else
		{
			PlayerPrefs.SetFloat("GameTempo", gameTempo);
			PlayerPrefs.Save();
		}
	}


	public static float GetGameTempo ()
	{
		return gameTempo;
	}
	

	public static void SetDelaysTempo ()
	{
		Debug.Log("Setting delays");
		defaultStarterPieceDelay = starterPieceDelay;
		defaultSwitchCardsModeDelay = switchCardsModeDelay;
		defaultShowHandInteractionDelay = showHandInteractionDelay;
		defaultYourTurnInteractionDelay = yourTurnInteractionDelay;
		defaultOpponentCardsDelay = opponentCardsDelay;
		defaultClearTableDelay = clearTableDelay;
		defaultRevealLastCardsDelay = revealLastCardsDelay;
		defaultExcludeCardsDelay = excludeCardsDelay;
		defaultExcludePlayersDelay = excludePlayersDelay;
		defaultNewHandDelay = newHandDelay;
		defaultWonGameScoreboardDelay = wonGameScoreboardDelay;
		defaultLostGameScoreboardDelay = lostGameScoreboardDelay;

		starterPieceDelay *= 1 / gameTempo;
		switchCardsModeDelay *= 1 / gameTempo;
		showHandInteractionDelay *= 1 / gameTempo;
		yourTurnInteractionDelay *= 1 / gameTempo;
		opponentCardsDelay *= 1 / gameTempo;
		clearTableDelay *= 1 / gameTempo;
		revealLastCardsDelay *= 1 / gameTempo;
		excludeCardsDelay *= 1 / gameTempo;
		excludePlayersDelay *= 1 / gameTempo;
		newHandDelay *= 1 / gameTempo;
		wonGameScoreboardDelay *= 1 / gameTempo;
		lostGameScoreboardDelay *= 1 / gameTempo;
	}


	public static void ResetDelays ()
	{
		if (defaultStarterPieceDelay != 0)
		{
			Debug.Log("Resetting delays");
			starterPieceDelay = defaultStarterPieceDelay;
			switchCardsModeDelay  = defaultSwitchCardsModeDelay;
			showHandInteractionDelay = defaultShowHandInteractionDelay;
			yourTurnInteractionDelay = defaultYourTurnInteractionDelay;
			opponentCardsDelay  = defaultOpponentCardsDelay;
			clearTableDelay = defaultClearTableDelay;
			revealLastCardsDelay  = defaultRevealLastCardsDelay;
			excludeCardsDelay = defaultExcludeCardsDelay;
			excludePlayersDelay = defaultExcludePlayersDelay;
			newHandDelay = defaultNewHandDelay;
			wonGameScoreboardDelay = defaultWonGameScoreboardDelay;
			lostGameScoreboardDelay = defaultLostGameScoreboardDelay;
		}
		else
		{
			Debug.Log("Delays not yet set");
		}
	}
}
