using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokerEvaluator : MonoBehaviour
{
    public static PokerEvaluator Instance;

    public CardManager cardManager;
    public BoardManager boardManager;
    public ChamberManager chamberManager;
    public PlayerManager playerHandManager;
    public int[] rankwisePoints;
    public PlayerEconomyManager playerEconomyManager;
    public Chamber winningChamber;

    [Header("Winning Hand Display")]
    public Transform[] topCardParents;
    public Color[] winLoseColors;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Returns the name of the hand based on its value
    string GetWinType(int handValue)
    {
        return handValue switch
        {
            1 => "High Card",
            2 => "One Pair",
            3 => "Two Pair",
            4 => "Three of a Kind",
            5 => "Straight",
            6 => "Flush",
            7 => "Full House",
            8 => "Four of a Kind",
            9 => "Straight Flush",
            _ => "Unknown Hand",
        };
    }

    // Evaluates the hand for a given chamber
    public HandEvaluation EvaluateHand(Chamber chamber, List<Card> cards)
    {
        var sortedCards = cards.OrderBy(c => c.cardInfo.cardNumber).ToList();
        var uniqueCards = sortedCards.GroupBy(c => c.cardInfo.cardNumber).Select(g => g.First()).ToList();

        HandEvaluation result = new HandEvaluation();
        List<Card> highValueCards = new List<Card>();

        // Check for flush
        bool isFlush = CheckFlush(sortedCards);
        List<Card> flushCards = isFlush ? GetFlushCards(sortedCards) : new List<Card>();

        // Check for straight
        bool isStraight = IsStraight(uniqueCards);
        List<Card> straightCards = isStraight ? GetStraightCards(uniqueCards) : new List<Card>();

        // Check for straight flush or royal flush
        if (isFlush && isStraight)
        {
            List<Card> straightFlushCards = flushCards.Intersect(straightCards).ToList();
            if (straightFlushCards.Count >= 5)
            {
                result.HandValue = 9; // Straight Flush
                result.HighValueCards = straightFlushCards.Take(5).ToList();
                return result;
            }
        }

        // Check for four of a kind, full house, etc.
        result.HandValue = GetMultiplesValue(sortedCards, out highValueCards);
        result.HighValueCards = highValueCards;

        // Update result if flush is better than current hand
        if (isFlush && result.HandValue < 6)
        {
            result.HandValue = 6; // Flush
            result.HighValueCards = flushCards.Take(5).ToList();
        }

        // Update result if straight is better than current hand
        if (isStraight && result.HandValue < 5)
        {
            result.HandValue = 5; // Straight
            result.HighValueCards = straightCards.Take(5).ToList();
        }

        chamber.chamberRankCards = new List<Card>();
        chamber.chamberRankCards.AddRange(result.HighValueCards);

        return result;
    }

    // Determines the value of multiples (pairs, three of a kind, etc.)
    int GetMultiplesValue(List<Card> cards, out List<Card> highValueCards)
    {
        var groups = cards.GroupBy(c => c.cardInfo.cardNumber)
                          .OrderByDescending(g => g.Count())
                          .ThenByDescending(g => g.Key)
                          .ToList();

        highValueCards = new List<Card>();

        if (groups.Any(g => g.Count() == 4))
        {
            // Four of a Kind
            var fourOfAKind = groups.First(g => g.Count() == 4).Take(4).ToList();
            var kicker = cards.Except(fourOfAKind).OrderByDescending(c => c.cardInfo.cardNumber).First();
            highValueCards = fourOfAKind.Concat(new List<Card> { kicker }).ToList();
            return 8;
        }

        if (groups.Any(g => g.Count() == 3))
        {
            var threeOfAKind = groups.First(g => g.Count() == 3).Take(3).ToList();
            var pair = groups.FirstOrDefault(g => g.Count() == 2);

            if (pair != null)
            {
                // Full House
                highValueCards = threeOfAKind.Concat(pair.Take(2)).ToList();
                return 7;
            }
            else
            {
                // Three of a Kind
                var kickers = cards.Except(threeOfAKind).OrderByDescending(c => c.cardInfo.cardNumber).Take(2).ToList();
                highValueCards = threeOfAKind.Concat(kickers).ToList();
                return 4;
            }
        }

        if (groups.Count(g => g.Count() == 2) >= 2)
        {
            // Two Pair
            var pairs = groups.Where(g => g.Count() == 2).Take(2).SelectMany(g => g.Take(2)).ToList();
            var kicker = cards.Except(pairs).OrderByDescending(c => c.cardInfo.cardNumber).First();
            highValueCards = pairs.Concat(new List<Card> { kicker }).ToList();
            return 3;
        }

        if (groups.Any(g => g.Count() == 2))
        {
            // One Pair
            var pair = groups.First(g => g.Count() == 2).Take(2).ToList();
            var kickers = cards.Except(pair).OrderByDescending(c => c.cardInfo.cardNumber).Take(3).ToList();
            highValueCards = pair.Concat(kickers).ToList();
            return 2;
        }

        // High Card
        highValueCards = cards.OrderByDescending(c => c.cardInfo.cardNumber).Take(5).ToList();
        return 1;
    }

    // Checks if the hand contains a straight
    bool IsStraight(List<Card> uniqueCards)
    {
        if (uniqueCards.Count < 5) return false;

        // Check for regular straight
        for (int i = 0; i <= uniqueCards.Count - 5; i++)
        {
            if (uniqueCards[i + 4].cardInfo.cardNumber == uniqueCards[i].cardInfo.cardNumber + 4)
                return true;
        }

        // Check for wheel straight (A-2-3-4-5)
        if (uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(14) && // Ace
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(2) &&
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(3) &&
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(4) &&
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(5))
        {
            return true;
        }

        return false;
    }

    // Gets the cards that form a straight
    List<Card> GetStraightCards(List<Card> uniqueCards)
    {
        if (uniqueCards.Count < 5) return new List<Card>();

        // Check for regular straight
        for (int i = 0; i <= uniqueCards.Count - 5; i++)
        {
            if (uniqueCards[i + 4].cardInfo.cardNumber == uniqueCards[i].cardInfo.cardNumber + 4)
                return uniqueCards.Skip(i).Take(5).ToList();
        }

        // Check for wheel straight (A-2-3-4-5)
        if (uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(14) && // Ace
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(2) &&
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(3) &&
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(4) &&
            uniqueCards.Select(c => c.cardInfo.cardNumber).Contains(5))
        {
            return uniqueCards.Where(c => c.cardInfo.cardNumber == 14 || c.cardInfo.cardNumber <= 5)
                              .OrderBy(c => c.cardInfo.cardNumber == 14 ? 1 : c.cardInfo.cardNumber)
                              .Take(5).ToList();
        }

        return new List<Card>();
    }

    // Checks if the hand contains a flush
    bool CheckFlush(List<Card> cards)
    {
        return cards.GroupBy(c => c.cardInfo.CardType).Any(g => g.Count() >= 5);
    }

    // Gets the cards that form a flush
    List<Card> GetFlushCards(List<Card> cards)
    {
        var flushSuit = cards.GroupBy(c => c.cardInfo.CardType).FirstOrDefault(g => g.Count() >= 5)?.Key;
        return cards.Where(c => c.cardInfo.CardType == flushSuit)
                    .OrderByDescending(c => c.cardInfo.cardNumber)
                    .Take(5)
                    .ToList();
    }

    public void CallForRevealAction()
    {
        // Show a tutorial for revealing all hands (if applicable)
        TutorialManager.Instance.ShowTutorial(TutorialType.RevealAllHand);

        // Update the game state to "Revealing Chamber Cards"
        GameManager.GetInstance().SetGameState(GameState.RevealingChamberCards);

    }
    public void CheckHand(Chamber chamber)
    {
        // Combine the board cards and the chamber's cards
        var allCards = new List<Card>(boardManager.cardsOnBoard);
        allCards.AddRange(chamber.chamberCards);

        // Evaluate the hand
        HandEvaluation evaluation = EvaluateHand(chamber, allCards);

        // Update the chamber's UI with the hand rank
        chamber.chamberUI.rankUICanvasGroup.alpha = 1;
        chamber.chamberUI.rankText.enabled = true;
        chamber.chamberUI.rankText.text = GetWinType(evaluation.HandValue);

        // Highlight the top cards of the evaluated hand
        chamber.PaintTopCards(evaluation.HighValueCards);

        // If this is the player's chosen chamber, award credits based on the hand rank
        if (chamber == playerHandManager.playerChosenChamber)
        {
            playerEconomyManager.UpdateCredit(rankwisePoints[evaluation.HandValue - 1]);
        }
    }
    // Determines the winning hand among all chambers
    public void CheckPokerLogics()
    {
        var cardsOnBoard = boardManager.cardsOnBoard;
        winningChamber = null;
        List<Chamber> tiedChambers = new List<Chamber>();
        int highestHandValue = -1;

        foreach (var chamber in chamberManager.chambers)
        {
            var chamberCardsInRiver = new List<Card>(cardsOnBoard);
            chamberCardsInRiver.AddRange(chamber.chamberCards);

            HandEvaluation evaluation = EvaluateHand(chamber, chamberCardsInRiver);

            if (evaluation.HandValue > highestHandValue)
            {
                highestHandValue = evaluation.HandValue;
                winningChamber = chamber;
                tiedChambers = new List<Chamber> { chamber };
            }
            else if (evaluation.HandValue == highestHandValue)
            {
                tiedChambers.Add(chamber);
            }
        }

        if (tiedChambers.Count == 1)
            HandleSingleWinner(tiedChambers[0], highestHandValue, cardsOnBoard);
        else if (tiedChambers.Count > 1)
            HandleTieBreaker(tiedChambers, highestHandValue, cardsOnBoard);
    }

    // Handles the case where there is a single winner
    void HandleSingleWinner(Chamber winner, int handValue, List<Card> cardsOnBoard)
    {
        winner.chamberUI.rankTextBG.color = winLoseColors[0];
      //  HighlightWinningHand(winner, handValue, cardsOnBoard);
        playerHandManager.CheckSelectedChamber(winner);
    }

    // Handles the case where there is a tie
    void HandleTieBreaker(List<Chamber> tiedChambers, int handValue, List<Card> cardsOnBoard)
    {
        var winningChambers = BreakTie(tiedChambers);
        if (winningChambers.Count == 1)
        {
            HandleSingleWinner(winningChambers[0], handValue, cardsOnBoard);
        }
        else
        {
            foreach (var chamber in winningChambers)
            {
                chamber.chamberUI.rankTextBG.color = winLoseColors[0];
                chamber.chamberUI.rankText.fontSize = 0.65f;
            }
          //  HighlightWinningHand(winningChambers[0], handValue, cardsOnBoard);
            playerHandManager.CheckSelectedChamber(winningChambers);
        }
    }

    // Breaks a tie between chambers with the same hand value
    List<Chamber> BreakTie(List<Chamber> tiedChambers)
    {
        tiedChambers.Sort((x, y) => CompareHighCards(x.chamberRankCards, y.chamberRankCards));
        return tiedChambers.TakeWhile(c => CompareHighCards(c.chamberRankCards, tiedChambers[0].chamberRankCards) == 0).ToList();
    }

    // Compares the high cards of two hands to determine the winner
    int CompareHighCards(List<Card> first, List<Card> second)
    {
        for (int i = 0; i < Mathf.Min(first.Count, second.Count); i++)
        {
            if (first[i].cardInfo.cardNumber != second[i].cardInfo.cardNumber)
                return second[i].cardInfo.cardNumber.CompareTo(first[i].cardInfo.cardNumber);
        }
        return 0; // Cards are equal
    }

  
}

// Class to store hand evaluation results
public class HandEvaluation
{
    public int HandValue { get; set; }
    public List<Card> HighValueCards { get; set; }
}