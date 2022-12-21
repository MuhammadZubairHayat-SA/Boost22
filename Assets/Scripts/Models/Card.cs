using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card 
{
	public Enums.CardType cardType;
	public bool isSelected = false;


	public Card (Enums.CardType newCardType)
	{
		cardType = newCardType;
	}
}
