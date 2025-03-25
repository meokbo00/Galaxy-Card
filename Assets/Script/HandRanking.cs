using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRanking : MonoBehaviour
{
    private DeckManager deckManager;
    private List<Card> selectedCards = new List<Card>();

    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
    }

    void Update()
    {
        // 선택된 카드 목록 업데이트
        selectedCards.Clear();
        foreach (Card card in deckManager.GetHand())
        {
            if (card != null && card.isSelected)
            {
                selectedCards.Add(card);
            }
        }

        // 선택된 카드 개수 로그 출력
        Debug.Log($"선택된 카드 개수: {selectedCards.Count}장");

        // 선택된 카드들의 랭크와 수트 정보 출력
        foreach (Card card in selectedCards)
        {
            string rank = card.rank.ToString();
            string suit = card.suit.ToString();
            //Debug.Log($"선택된 카드: {rank} of {suit}");
        }

        // 포커 조합 판단 및 출력
        if (selectedCards.Count > 0)
        {
            string handRank = DetermineHandRank();
            Debug.Log($"현재 포커 조합: {handRank}");
        }
    }

    private string DetermineHandRank()
    {
        if (selectedCards.Count == 1)
        {
            return "하이카드";
        }
        else if (selectedCards.Count == 2)
        {
            // 같은 랭크인지 확인
            if (selectedCards[0].rank == selectedCards[1].rank)
            {
                return "원 페어";
            }
            // 같은 수트인지 확인
            else if (selectedCards[0].suit == selectedCards[1].suit)
            {
                return "플러시";
            }
            // 연속된 숫자인지 확인
            else if (Mathf.Abs((int)selectedCards[0].rank - (int)selectedCards[1].rank) == 1)
            {
                return "스트레이트";
            }
            else
            {
                return "하이카드";
            }
        }
        else if (selectedCards.Count == 3)
        {
            // 트리플 확인
            if (selectedCards[0].rank == selectedCards[1].rank &&
                selectedCards[1].rank == selectedCards[2].rank)
            {
                return "트리플";
            }
            // 투 페어 확인
            else if (selectedCards[0].rank == selectedCards[1].rank ||
                     selectedCards[1].rank == selectedCards[2].rank ||
                     selectedCards[0].rank == selectedCards[2].rank)
            {
                return "투 페어";
            }
            // 플러시 확인
            else if (selectedCards[0].suit == selectedCards[1].suit &&
                     selectedCards[1].suit == selectedCards[2].suit)
            {
                return "플러시";
            }
            // 스트레이트 확인
            else if (IsStraight(selectedCards))
            {
                return "스트레이트";
            }
            else
            {
                return "하이카드";
            }
        }
        else if (selectedCards.Count == 4)
        {
            // 포카드 확인
            if (IsFourOfAKind(selectedCards))
            {
                return "포카드";
            }
            // 풀하우스 확인
            else if (IsFullHouse(selectedCards))
            {
                return "풀하우스";
            }
            // 플러시 확인
            else if (IsFlush(selectedCards))
            {
                return "플러시";
            }
            // 스트레이트 확인
            else if (IsStraight(selectedCards))
            {
                return "스트레이트";
            }
            // 트리플 확인
            else if (IsThreeOfAKind(selectedCards))
            {
                return "트리플";
            }
            // 투 페어 확인
            else if (IsTwoPair(selectedCards))
            {
                return "투 페어";
            }
            else
            {
                return "하이카드";
            }
        }
        else if (selectedCards.Count == 5)
        {
            // 로열 플러시 확인
            if (IsRoyalFlush(selectedCards))
            {
                return "로열 플러시";
            }
            // 스트레이트 플러시 확인
            else if (IsStraightFlush(selectedCards))
            {
                return "스트레이트 플러시";
            }
            // 포카드 확인
            else if (IsFourOfAKind(selectedCards))
            {
                return "포카드";
            }
            // 풀하우스 확인
            else if (IsFullHouse(selectedCards))
            {
                return "풀하우스";
            }
            // 플러시 확인
            else if (IsFlush(selectedCards))
            {
                return "플러시";
            }
            // 스트레이트 확인
            else if (IsStraight(selectedCards))
            {
                return "스트레이트";
            }
            else
            {
                return "하이카드";
            }
        }

        return "하이카드";
    }

    private bool IsStraight(List<Card> cards)
    {
        List<int> ranks = new List<int>();
        foreach (Card card in cards)
        {
            ranks.Add((int)card.rank);
        }
        ranks.Sort();

        for (int i = 1; i < ranks.Count; i++)
        {
            if (ranks[i] - ranks[i - 1] != 1)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsFlush(List<Card> cards)
    {
        Card.Suit firstSuit = cards[0].suit;
        foreach (Card card in cards)
        {
            if (card.suit != firstSuit)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsFourOfAKind(List<Card> cards)
    {
        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }
        return rankCount.ContainsValue(4);
    }

    private bool IsFullHouse(List<Card> cards)
    {
        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }
        return rankCount.ContainsValue(3) && rankCount.ContainsValue(2);
    }

    private bool IsThreeOfAKind(List<Card> cards)
    {
        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }
        return rankCount.ContainsValue(3);
    }

    private bool IsTwoPair(List<Card> cards)
    {
        Dictionary<Card.Rank, int> rankCount = new Dictionary<Card.Rank, int>();
        foreach (Card card in cards)
        {
            if (!rankCount.ContainsKey(card.rank))
            {
                rankCount[card.rank] = 0;
            }
            rankCount[card.rank]++;
        }
        int pairCount = 0;
        foreach (var count in rankCount.Values)
        {
            if (count >= 2)
            {
                pairCount++;
            }
        }
        return pairCount >= 2;
    }

    private bool IsStraightFlush(List<Card> cards)
    {
        return IsFlush(cards) && IsStraight(cards);
    }

    private bool IsRoyalFlush(List<Card> cards)
    {
        if (!IsStraightFlush(cards))
        {
            return false;
        }
        List<int> ranks = new List<int>();
        foreach (Card card in cards)
        {
            ranks.Add((int)card.rank);
        }
        ranks.Sort();
        return ranks[ranks.Count - 1] == (int)Card.Rank.Ace;
    }
}