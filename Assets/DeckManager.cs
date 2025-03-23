using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;
    private List<Card> deck = new List<Card>();
    private List<Card> discardPile = new List<Card>();
    private List<Card> hand = new List<Card>();

    void Start()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    private void InitializeDeck()
    {
        // 52장의 카드 생성
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
            {
                GameObject cardObj = Instantiate(cardPrefab);
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(suit, rank);
                deck.Add(card);
            }
        }
    }

    public void ShuffleDeck()
    {
        // Fisher-Yates 셔플 알고리즘
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Card temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            ReshuffleDiscardPile();
        }

        if (deck.Count > 0)
        {
            Card drawnCard = deck[0];
            deck.RemoveAt(0);
            hand.Add(drawnCard);
            return drawnCard;
        }

        return null;
    }

    public void DiscardCard(Card card)
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
            discardPile.Add(card);
        }
    }

    private void ReshuffleDiscardPile()
    {
        deck.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    public int GetDeckCount()
    {
        return deck.Count;
    }

    public int GetDiscardPileCount()
    {
        return discardPile.Count;
    }

    public void ClearHand()
    {
        hand.Clear();
    }

    public void AddJoker()
    {
        GameObject cardObj = Instantiate(cardPrefab);
        Card joker = cardObj.GetComponent<Card>();
        joker.isJoker = true;
        deck.Add(joker);
    }
} 