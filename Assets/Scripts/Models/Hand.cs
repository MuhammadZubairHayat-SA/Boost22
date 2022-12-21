using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand
{
    public List<Card> cards;


    public Hand()
    {
        cards = new List<Card>();
    }


    void Start()
    {

    }


    void Update()
    {

    }


    public bool CanSelectCard(Card card)
    {
        return GameManager.shared.CanSelectCard(card);
    }


    public List<Card> GetSelectedCards()
    {
        List<Card> selectedCards = new List<Card>();

        foreach (Card card in cards)
        {
            if (card.isSelected)
            {
                selectedCards.Add(card);
            }
        }

        return selectedCards;
    }


    public void ClearCards()
    {
        cards = new List<Card>();
    }


    public void RemoveCards(List<Card> cardsToRemove)
    {
        cards = cards.Except(cardsToRemove).ToList();
        SortCards();
    }


    public void AddCards(List<Card> cardsToAdd)
    {
        cards.AddRange(cardsToAdd);
        SortCards();
    }


    public void AddCard(Card cardToAdd)
    {
        cards.Add(cardToAdd);
        SortCards();
    }


    public List<List<Card>> GetCardsByFrequency()
    {
        // assuming that cards are sorted
        var result = new List<List<Card>>();
        if (cards.Count > 0) {
            var value = 0;
            foreach (Card card in cards) {
                if ((int)card.cardType / 4 != value)
                {
                    result.Add(new List<Card>());
                    value = (int)card.cardType / 4;
                }
                result[result.Count - 1].Add(card);
            }
        }
        return result;
    }


    private void SortCards()
    { 
        cards = cards.OrderBy(w => (int)w.cardType).ToList();
    }
}
