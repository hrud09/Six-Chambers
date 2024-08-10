using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PokerEvaluator : MonoBehaviour
{
    string winType(int handValue)
    {
        switch (handValue)
        {
            case 1: return "High Card";
            case 2: return "One Pair";
            case 3: return "Two Pair";
            case 4: return "Three of a Kind";
            case 5: return "Straight";
            case 6: return "Flush";
            case 7: return "Full House";
            case 8: return "Four of a Kind";
            case 9: return "Straight Flush";
            default: return "Unknown Hand";
        }
    }
    public TMP_Text winText;
    public CardManager cardManager;
    public PlayerHandManager playerHandManager;
    public int[] rankwisePoints;
    public void RevealLastTwoCards()
    {
        for (int i = 0; i < cardManager.boardManager.cardsOnBoard.Count; i++)
        {
            int index = i;
            Transform card = cardManager.boardManager.cardsOnBoard[i].transform;

            card.DOLocalRotate(Vector3.zero, 0.5f).SetDelay(0.2f).OnComplete(() =>
            {
                if (index == cardManager.boardManager.cardsOnBoard.Count - 1)
                {
                    RevealEveryOnesCards();
                }
            });
        }
    }

    public void RevealEveryOnesCards()
    {
        foreach (Transform t in cardManager.cardsOnHands)
        {
            int index = cardManager.cardsOnHands.IndexOf(t);
            t.DOLocalRotate(Vector3.zero, 0.5f).SetDelay(cardManager.cardsOnHands.IndexOf(t) * 0.03f).OnComplete(() =>
            {
                if (index == cardManager.cardsOnHands.Count - 1)
                {
                    CheckPokerLogics();
                }
            });
        }
    }

    void CheckPokerLogics()
    {
        List<Card> cardsOnBoard = cardManager.boardManager.cards;
        Chamber winningChamber = null;
        List<Chamber> winningChambers;
        int highestHandValue = -1;
        List<Chamber> tiedChambers = new List<Chamber>();

        foreach (Transform t in cardManager.chamberManager.chamberTransforms)
        {
            Chamber chamberSelected = t.GetComponent<Chamber>();

            List<Card> chamberCards = new List<Card>(cardsOnBoard);
            chamberCards.AddRange(chamberSelected.chamberCards);

            HandEvaluation handEvaluation = EvaluateHand(chamberCards);
            chamberSelected.chamberCards = handEvaluation.HighValueCards;
            int handValue = handEvaluation.HandValue;
            if (handValue > highestHandValue)
            {
                highestHandValue = handValue;
                winningChamber = chamberSelected;
                tiedChambers.Clear();
                tiedChambers.Add(winningChamber);

            }
            else if (handValue == highestHandValue)
            {
                tiedChambers.Add(chamberSelected);
            }
        }

        if (tiedChambers.Count == 1)
        {
            HandEvaluation winningHand = EvaluateHand(new List<Card>(cardsOnBoard.Concat(winningChamber.chamberCards)));

            winText.text = $"{winningChamber.index} Wins by {winType(highestHandValue)}";
        }
        else
        {
            string tiedChambersIndices = string.Join(", ", tiedChambers.Select(chamber => chamber.index.ToString()).ToArray());
            winningChambers = BreakTie(tiedChambers);
            if (winningChambers.Count == 1)
            {
                HandEvaluation winningHand = EvaluateHand(new List<Card>(cardsOnBoard.Concat(winningChambers[0].chamberCards)));
                tiedChambers[0].topRankUI.SetActive(true);
                playerHandManager.CheckSelectedChamber(tiedChambers[0], rankwisePoints[highestHandValue - 1]);
                winText.text = $"{winningChambers[0].index} Wins by Tie-Breaker among Chambers: {tiedChambersIndices} by {winType(highestHandValue)}";
            }
            else
            {
                string message = "";
                foreach (var item in winningChambers)
                {
                    message += item.index.ToString() + ",";
                }
                message += " Wins by " + winType(highestHandValue).ToString();
                winText.text = message;
            }
            playerHandManager.CheckSelectedChamber(tiedChambers, rankwisePoints[highestHandValue - 1]);
            foreach (Chamber ch in tiedChambers)
            {

                Debug.LogWarning(

                    $"{ch.chamberCards[0].cardInfo.cardNumber},{ch.chamberCards[1].cardInfo.cardNumber},{ch.chamberCards[2].cardInfo.cardNumber}," +
                    $"{ch.chamberCards[3].cardInfo.cardNumber}, {ch.chamberCards[4].cardInfo.cardNumber}"

                    );


            }
        }
    }


    HandEvaluation EvaluateHand(List<Card> cards)
    {
        List<Card> sortedCards = cards.OrderBy(card => card.cardInfo.cardNumber).ToList();
        List<Card> sortedCardWithoutDuplicates = cards.OrderBy(card => card.cardInfo.cardNumber).Distinct().ToList();

        HandEvaluation result = new HandEvaluation();
        result.HandValue = 0;

        bool isStraight = IsStraight(sortedCardWithoutDuplicates);
        bool isFlush = CheckFlush(sortedCards);


        int tempHandValue = GetMultiplesValue(sortedCards, out List<Card> highValueCards);
        result.HighValueCards = highValueCards;

        if (isStraight && isFlush)
        {
            List<Card> flushedCards = cards
      .GroupBy(card => card.cardInfo.CardType)
      .Where(group => group.Count() >= 5)
      .SelectMany(group => group.ToList())
      .ToList();

            flushedCards.Distinct();
            if (flushedCards.Count >= 5 && IsStraight(flushedCards))
            {
                result.HandValue = 9;
                return result;
            }

        }
        else if (isFlush && tempHandValue < 6)
        {
            result.HandValue = 6;


            return result;
        }
        else if (isStraight && tempHandValue < 5)
        {
            result.HandValue = 5;
            List<Card> allCardsInHand = new List<Card>();

            allCardsInHand.Add(sortedCardWithoutDuplicates[0]);

            for (int i = 1; i < allCardsInHand.Count; i++)
            {
                if (allCardsInHand[i].cardInfo.cardNumber == allCardsInHand[i - 1].cardInfo.cardNumber + 1)
                {
                    allCardsInHand.Add(allCardsInHand[i]);
                }
                else
                {
                    allCardsInHand = new List<Card>();
                    allCardsInHand.Add(allCardsInHand[i]);
                }
            }
            result.HighValueCards = allCardsInHand;
            return result;
        }

        result.HandValue = tempHandValue;

        return result;
    }

    public bool IsStraight(List<Card> cards)
    {

        List<Card> sortedCards = cards.OrderBy(card => card.cardInfo.cardNumber).ToList();


        List<Card> uniqueCards = sortedCards.GroupBy(card => card.cardInfo.cardNumber)
                                            .Select(group => group.First())
                                            .ToList();


        for (int i = 0; i <= uniqueCards.Count - 5; i++)
        {
            bool isStraight = true;
            for (int j = 1; j < 5; j++)
            {
                if (uniqueCards[i + j].cardInfo.cardNumber != uniqueCards[i].cardInfo.cardNumber + j)
                {
                    isStraight = false;
                    break;
                }
            }
            if (isStraight)
            {
                return true;
            }
        }

        // Special case for Ace-low straight (5, 4, 3, 2, Ace)
        if (uniqueCards.Count >= 5 &&
            uniqueCards[0].cardInfo.cardNumber == 2 &&
            uniqueCards[1].cardInfo.cardNumber == 3 &&
            uniqueCards[2].cardInfo.cardNumber == 4 &&
            uniqueCards[3].cardInfo.cardNumber == 5 &&
            uniqueCards.Last().cardInfo.cardNumber == 14)
        {
            return true;
        }

        return false;
    }

    bool CheckFlush(List<Card> cards)
    {
        return cards.GroupBy(card => card.cardInfo.CardType).Any(group => group.Count() >= 5);
    }

    int GetMultiplesValue(List<Card> cards, out List<Card> highValueCards)
    {
        highValueCards = new List<Card>();

        var groups = cards.GroupBy(card => card.cardInfo.cardNumber).OrderByDescending(group => group.Count()).ThenByDescending(group => group.Key);

        var maxGroup = groups.First();
        var secondMaxGroup = groups.Skip(1).FirstOrDefault();

        if (maxGroup.Count() == 4)
        {
            highValueCards.AddRange(maxGroup);
            highValueCards.AddRange(cards.Except(maxGroup).OrderByDescending(card => card.cardInfo.cardNumber).Take(1));
            return 8;
        }

        if (maxGroup.Count() == 3 && secondMaxGroup != null && secondMaxGroup.Count() >= 2)
        {
            highValueCards.AddRange(maxGroup);
            highValueCards.AddRange(secondMaxGroup.Take(2));
            return 7;
        }

        if (maxGroup.Count() == 3)
        {
            highValueCards.AddRange(maxGroup);
            highValueCards.AddRange(cards.Except(maxGroup).OrderByDescending(card => card.cardInfo.cardNumber).Take(2));
            return 4;
        }

        if (maxGroup.Count() == 2 && secondMaxGroup != null && secondMaxGroup.Count() == 2)
        {
            highValueCards.AddRange(maxGroup);
            highValueCards.AddRange(secondMaxGroup.Take(2));
            highValueCards.AddRange(cards.Except(maxGroup).Except(secondMaxGroup).OrderByDescending(card => card.cardInfo.cardNumber).Take(1));
            return 3;
        }

        if (maxGroup.Count() == 2)
        {
            highValueCards.AddRange(maxGroup);
            highValueCards.AddRange(cards.Except(maxGroup).OrderByDescending(card => card.cardInfo.cardNumber).Take(3));
            return 2;
        }

        highValueCards.AddRange(cards.OrderByDescending(card => card.cardInfo.cardNumber).Take(5));
        return 1;
    }



    List<Chamber> BreakTie(List<Chamber> tiedChambers)
    {
        List<Chamber> highestChambers = new List<Chamber>();

        if (tiedChambers == null || tiedChambers.Count == 0)
            return highestChambers;

        highestChambers.AddRange(tiedChambers);
        for (int j = 1; j < highestChambers.Count; j++)
        {
            for (int i = 0; i < highestChambers[j].chamberCards.Count; i++)
            {
                int previousCardNumber = highestChambers[j - 1].chamberCards[i].cardInfo.cardNumber;
                int currentCardNumber = highestChambers[j].chamberCards[i].cardInfo.cardNumber;

                if (currentCardNumber > previousCardNumber)
                {
                    highestChambers.RemoveAt(j - 1);
                    j--;
                    break;
                }
                else if (currentCardNumber < previousCardNumber)
                {
                    highestChambers.RemoveAt(j);
                    j--;
                    break;
                }
            }
        }

        return highestChambers;
    }






}

public class HandEvaluation
{
    public int HandValue { get; set; }
    public List<Card> HighValueCards { get; set; }
}