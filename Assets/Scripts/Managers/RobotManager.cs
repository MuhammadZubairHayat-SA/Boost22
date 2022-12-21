using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotManager
{

    public static List<Card> ChooseCardsToPlay(Game game, RobotPlayer player)
    {
        if (game.gameIsRigged == false)
        {
            player.SwitchCards();
        }
        
        var cardTypes = new List<Card>();
        var cardsByFrequency = player.hand.GetCardsByFrequency();
        if (player == game.roundStartingPlayer)
        {
            var maxCount = player.hand.cards.Count - 1; // the amount of cards that maximum can be played now
            if (maxCount > 4)
            {
                maxCount = 4;
            }

            var lowestCardValueToConsider = 8;
            if (player.hand.cards.Count == 3 && (int)player.hand.cards.Last().cardType / 4 >= 13)
            {
                lowestCardValueToConsider = (int)player.hand.cards[1].cardType / 4;
            }

            var possibleCards = new List<List<Card>>();  // [(count: Int, cardValue: Int)]()
            for (int i = maxCount; i >= 1; i--)
            {
                // find all card lists containing "i" cards with a value from 8 and up
                possibleCards = cardsByFrequency.ToList().Where(cardList => ((List<Card>)cardList).Count >= i && (int)((List<Card>)cardList)[0].cardType / 4 >= lowestCardValueToConsider).ToList();
                // iOS: possibleCards = cardsByFrequency.filter({ $0.count >= i && $0.cardValue >= 8 })

                if (possibleCards.Count > 0)
                {
                    break;
                }
            }

            int valueToPlay;
            if (possibleCards.Count == 0 || cardsByFrequency.Count == 2)
            {
                // if no possible cards were found (if values are low), play the card(s) with the largest value
                valueToPlay = (int)cardsByFrequency.ToList().Last()[0].cardType / 4;
            }
            else
            {
                if (possibleCards.Count == player.hand.cards.Count)
                {
                    // all cards are high - take one of the highest
                    if ((int)player.hand.cards.Last().cardType / 4 >= 13 && possibleCards.Count >= 3)
                    {
                        // pick next highest card
                        valueToPlay = (int)possibleCards[possibleCards.Count - 2][0].cardType / 4;
                    }
                    else
                    {
                        // pick highest card
                        valueToPlay = (int)possibleCards[possibleCards.Count - 1][0].cardType / 4;
                    }
                }
                else
                {
                    valueToPlay = (int)possibleCards[0][0].cardType / 4;
                }
            }

            List<Card> cardsToPlay = player.hand.cards.Where(card => (int)card.cardType / 4 == valueToPlay).ToList();
            if (cardsToPlay.Count == player.hand.cards.Count)
            {
                cardsToPlay.RemoveAt(0);
            }
            foreach (Card card in cardsToPlay)
            {
                cardTypes.Add(card);
            }

        }
        else
        {
            var highestCardsPlayed = game.highestCardsPlayed;
            if (highestCardsPlayed.Count > 0)
            {
                var count = highestCardsPlayed.Count;
                var value = (int)highestCardsPlayed[0].cardType / 4;
                var firstValue = value >= 8 ? value : 8;
                var possibleCards = cardsByFrequency.ToList().Where(cardList => ((List<Card>)cardList).Count == count && (int)((List<Card>)cardList)[0].cardType / 4 >= firstValue).ToList();
                // iOS: var possibleCards = cardsByFrequency.filter({ $0.count == count && $0.cardValue >= firstValue })

                if (possibleCards.Count == 0)
                {
                    possibleCards = cardsByFrequency.ToList().Where(cardList => ((List<Card>)cardList).Count >= count && (int)((List<Card>)cardList)[0].cardType / 4 >= value).ToList();
                    // iOS: possibleCards = cardsByFrequency.filter({ $0.count >= count && $0.cardValue >= value })
                }

                if (possibleCards.Count > 0)
                {
                    if (cardsByFrequency.Count == 2)
                    {
                        // make sure to only keep the card(s) with the largest value
                        while (possibleCards.Count > 1)
                        {
                            possibleCards.RemoveAt(0);
                        }
                    }
                    foreach (Card card in possibleCards[0])
                    {
                        cardTypes.Add(card);
                        if (cardTypes.Count >= count)
                        {
                            break;
                        }
                    }
                    game.highestCardsPlayed = cardTypes;
                }
                else
                {
                    // play the card(s) with the lowest value(s)
                    for (int i = 0; i < count; i++)
                    {
                        cardTypes.Add(player.hand.cards[i]);
                    }
                }
            }
        }

        return cardTypes;
    }
}
