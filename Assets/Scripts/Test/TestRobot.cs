using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TestRobot 
{
	public static List<int> GetRandomCardIndexes (Hand hand, int numberOfPlayedCards)
	{
		List<int> indexes = new List<int>();
		int numberOfCardsToPlay = 0;

		if (numberOfPlayedCards == 0)
		{
			if (hand.cards.Count > 3)
			{
				numberOfCardsToPlay = Mathf.FloorToInt(Random.Range(1, 2.99f));
			}
			else
			{
				numberOfCardsToPlay = 1;
			}
		}
		else
		{
			numberOfCardsToPlay = numberOfPlayedCards;
		}

		//int index = Random.Range(0, hand.cards.Count - 1);
		int index = Random.Range(0, hand.cards.Count - numberOfCardsToPlay);
		
		/*
		foreach (Card card in hand.cards)
		{
			if (Enums.CardTypeToInt(hand.cards[index].cardType) == Enums.CardTypeToInt(card.cardType))
			{
				indexes.Add(hand.cards.IndexOf(card));
			}
		}
		*/
		
		for (int i = 0; i < numberOfCardsToPlay; i++)
		{
			indexes.Add(index + i);
		}

		return indexes;
	}
}
