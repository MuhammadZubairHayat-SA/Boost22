using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPlayer : Player 
{
    public Enums.RobotType robotType;

    public RobotPlayer(User user, Enums.RobotType robotType)
    {
        this.user = user;
        playerType = Enums.PlayerType.Robot;
        this.robotType = robotType;
        hand = new Hand();
    }

/*    public override void PlayedCards(List<Card> playedCards)
    {
        if (hand.cards.Count == 7)
        {
            SwitchCards();
        }
        base.PlayedCards(playedCards);
    }*/


    public void SwitchCards()
    {
        // switching is only possible, if the player has 7 cards on his hand
        if (hand.cards.Count != 7 || robotType == Enums.RobotType.Easy)
        {
            return;
        }
        //Debug.Log("SWITCH CARDS");
        //string s = "Cards: ";
        //foreach (Card card in hand.cards)
        //{
        //    s += (int)card.cardType / 4 + " ";
        //}
        //Debug.Log(s);

        switch (robotType)
        {
            case Enums.RobotType.Medium:
                {
                    // switch 0 or 1 card
                    var cardsToSwitch = new List<Card>();
                    foreach (List<Card> cardList in hand.GetCardsByFrequency())
                    {
                        if ((int)cardList[0].cardType / 4 > 4 && (int)cardList[0].cardType / 4 < 11 && cardList.Count == 1)
                        {
                            cardsToSwitch.Add(cardList[0]);
                            break;
                        }
                    }
                    GameManager.shared.SwitchCards(player: this, cards: cardsToSwitch);
                    break;
                }
            case Enums.RobotType.Hard:
                {
                    // switch any single cards that seem switchable
                    var cardsToSwitch = new List<Card>();
                    var playerHand = hand.GetCardsByFrequency();
                    var lowestSwitchValue = (int)playerHand[0][0].cardType / 4;
                    var highestSwitchValue = (int)playerHand[playerHand.Count - 1][0].cardType / 4;
                    lowestSwitchValue = lowestSwitchValue <= 4 ? 5 : 6;
                    highestSwitchValue = highestSwitchValue >= 12 ? 11 : 12;
                    foreach (List<Card> cardList in playerHand)
                    {
                        if ((int)cardList[0].cardType / 4 >= lowestSwitchValue && (int)cardList[0].cardType / 4 <= highestSwitchValue && cardList.Count == 1)
                        {
                            cardsToSwitch.Add(cardList[0]);
                        }
                    }
                    GameManager.shared.SwitchCards(player: this, cards: cardsToSwitch);
                    break;
                }

            default: break;
        }

        //s = "After: ";
        //foreach (Card card in hand.cards)
        //{
        //    s += (int)card.cardType / 4 + " ";
        //}
        //Debug.Log(s);
    }

    public void StartRobot ()
	{

	}
}
