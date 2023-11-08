using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSuitData : Data
{
    public enum CARD { HEART, DIAMOUND, SPADE, CLUB };
    private static Dictionary<CARD, string> cardSuitName = new Dictionary<CARD, string>() { { CARD.HEART, "\x2665" }, { CARD.DIAMOUND, "\x2666" }, { CARD.SPADE, "\x2660" }, { CARD.CLUB, "\x2663" } };

    public CARD card;

    public CardSuitData(CARD card)
    {
        this.card = card;
    }

    public override string ToString()
    {
        return cardSuitName[card];
    }
}
