using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokerEvaluator : MonoBehaviour
{

    public static PokerEvaluator Instance;
    string GetWinType(int handValue)
    {
        return handValue switch
        {
            1 => "High Card",
            2 => "One Pair",
            3 => "Two Pair",
            4 => "3 o'a Kind",
            5 => "Straight",
            6 => "Flush",
            7 => "Full House",
            8 => "4 o'a Kind",
            9 => "Straight Flush",
            _ => "Unknown Hand",
        };
    }

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
    public bool ReadyToRevealChamberCards;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }


    public void CallForRevealAction()
    {
        TutorialManager.Instance.ShowTutorial(TutorialType.RevealAllHand);
        ReadyToRevealChamberCards = true;

    }


    public void CheckHand(Chamber chamber)
    {
        var allCards = new List<Card>(boardManager.cards);
        allCards.AddRange(chamber.chamberCards);

        HandEvaluation evaluation = EvaluateHand(chamber, allCards);
        chamber.chamberUI.rankUICanvasGroup.alpha = 1;
        chamber.chamberUI.rankText.enabled = true;
        chamber.chamberUI.rankText.text = GetWinType(evaluation.HandValue);
        chamber.PaintTopCards(evaluation.HighValueCards);

        if (chamber == playerHandManager.playerChosenChamber)
            playerEconomyManager.UpdateCredit(rankwisePoints[evaluation.HandValue - 1]);
    }

    public void CheckPokerLogics()
    {
        var cardsOnBoard = boardManager.cards;
        winningChamber = null;
        List<Chamber> tiedChambers = new List<Chamber>();
        int highestHandValue = -1;

        foreach (var chamber in chamberManager.chambers)
        {
            var chamberCardsincRivver = new List<Card>(cardsOnBoard);
            chamberCardsincRivver.AddRange(chamber.chamberCards);

            HandEvaluation evaluation = EvaluateHand(chamber, chamberCardsincRivver);
            // winningChamber.chamberCardsincRivver = evaluation.HighValueCards;

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

    void HandleSingleWinner(Chamber winner, int handValue, List<Card> cardsOnBoard)
    {
        winner.chamberUI.rankTextBG.color = winLoseColors[0];
        HighlightWinningHand(winner, handValue, cardsOnBoard);
        playerHandManager.CheckSelectedChamber(winner, rankwisePoints[handValue - 1]);
       // winText.text = $"Winner: {winner.chamberIndex} : {GetWinType(handValue)}";
    }

    void HandleTieBreaker(List<Chamber> tiedChambers, int handValue, List<Card> cardsOnBoard)
    {
        var winningChambers = BreakTie(tiedChambers);
        if (winningChambers.Count == 1)
        {
            HandleSingleWinner(winningChambers[0], handValue, cardsOnBoard);
           // winText.text = $"{winningChambers[0].chamberIndex} Wins by Tie-Breaker.";
        }
        else
        {
           // string winingText = GetWinType(handValue);
            foreach (var chamber in winningChambers)
            {
                chamber.chamberUI.rankTextBG.color = winLoseColors[0];
                chamber.chamberUI.rankText.fontSize = 0.65f;
            }
            HighlightWinningHand(winningChambers[0], handValue, cardsOnBoard);
            playerHandManager.CheckSelectedChamber(winningChambers[0], rankwisePoints[handValue - 1]);
            // winText.text = "Tie: Multiple Winners";
        }
    }

    void HighlightWinningHand(Chamber winningChamber, int handValue, List<Card> cardsOnBoard)
    {
        //winningHandShowBG.SetActive(true);

        var winningHand = EvaluateHand(winningChamber, new List<Card>(cardsOnBoard.Concat(winningChamber.chamberCards)));
        int index = 0;
        print(winningHand.HighValueCards.Count);
        foreach (var card in winningHand.HighValueCards)
        {
            GameObject newCard = Instantiate(card.gameObject, card.transform.position, card.transform.rotation, topCardParents[index]);
            newCard.GetComponent<Card>().playerSelectionAura.SetActive(false);
            newCard.transform.GetChild(1).gameObject.SetActive(false);
            newCard.transform.DOLocalRotate(Vector3.zero, 0.4f);
            newCard.transform.DOLocalMoveY(0.01f, 0.4f).SetDelay(index * 0.2f).OnComplete(() => {


                newCard.transform.DOLocalMove(Vector3.zero, 0.5f);
            });
            card.EnableTopCardVisual();
            index++;
        }

        winningChamber.chamberUI.rankText.fontSize = 0.65f;
    }

    HandEvaluation EvaluateHand(Chamber chamber, List<Card> cards)
    {

        var sortedCards = cards.OrderBy(c => c.cardInfo.cardNumber).ToList();
        var uniqueCards = sortedCards.GroupBy(c => c.cardInfo.cardNumber).Select(g => g.First()).ToList();

        HandEvaluation result = new HandEvaluation();
        List<Card> HighValueCards = new List<Card>();
        result.HandValue = GetMultiplesValue(sortedCards, out HighValueCards);
        result.HighValueCards = HighValueCards;
        chamber.chamberRankCards = new List<Card>();
        chamber.chamberRankCards.AddRange(HighValueCards);
        bool isStraight = IsStraight(uniqueCards);
        bool isFlush = CheckFlush(sortedCards);

        if (isStraight && isFlush)
            return CheckStraightFlush(cards, result);

        if (isFlush && result.HandValue < 6)
            return UpdateFlushResult(cards, result);

        if (isStraight && result.HandValue < 5)
            return UpdateStraightResult(uniqueCards, result);

        return result;
    }
    int GetMultiplesValue(List<Card> cards, out List<Card> highValueCards)
    {
        // Group cards by card number
        var groups = cards.GroupBy(c => c.cardInfo.cardNumber)
                          .OrderByDescending(g => g.Count()) // Sort by frequency
                          .ThenByDescending(g => g.Key); // Then by card number

        // Initialize high-value cards
        List<Card> _highCards = new List<Card>();
        highValueCards = new List<Card>();
        // Initialize rank variable
        int rank = 1; // Default to High Card

        // Check for hand types based on grouped card counts
        foreach (var group in groups)
        {
            var count = group.Count();
            if (count == 4)
            {
                // Four of a Kind
                _highCards = group.Take(4).ToList();
                var kicker = cards.Where(c => !_highCards.Contains(c))
                                  .OrderByDescending(c => c.cardInfo.cardNumber)
                                  .First(); // Add highest kicker
                _highCards.Add(kicker);
                highValueCards = _highCards;
                rank = 8;
                break;
            }
            else if (count == 3)
            {
                // Check for Full House
                var pair = groups.FirstOrDefault(g2 => g2.Count() == 2);
                if (pair != null)
                {
                    _highCards = group.Take(3).Concat(pair.Take(2)).ToList();
                    rank = 7; // Full House
                    highValueCards = _highCards;
                    break;
                }
                // Otherwise, Three of a Kind
                _highCards = group.Take(3).ToList();
                var kickers = cards.Where(c => !_highCards.Contains(c))
                                   .OrderByDescending(c => c.cardInfo.cardNumber)
                                   .Take(2); // Add 2 kickers
                _highCards.AddRange(kickers);
                highValueCards = _highCards;
                rank = 4;
                break;
            }
            else if (count == 2)
            {
                // Check for Two Pair
                var secondPair = groups.Skip(1).FirstOrDefault(g2 => g2.Count() == 2);
                if (secondPair != null)
                {
                    _highCards = group.Take(2).Concat(secondPair.Take(2)).ToList();
                    var kicker = cards.Where(c => !_highCards.Contains(c))
                                      .OrderByDescending(c => c.cardInfo.cardNumber)
                                      .First(); // Add highest kicker
                    _highCards.Add(kicker);
                    highValueCards = _highCards;
                    rank = 3;
                    break;
                }
                // Otherwise, One Pair
                _highCards = group.Take(2).ToList();
                var otherKickers = cards.Where(c => !_highCards.Contains(c))
                                        .OrderByDescending(c => c.cardInfo.cardNumber)
                                        .Take(3); // Add 3 kickers
                _highCards.AddRange(otherKickers);
                highValueCards = _highCards;
                rank = 2;
                break;
            }
            // print($"HighValueCards: {_highCards.Count}");

        }

        // If rank is still 1 (High Card), pick the top 5 cards
        if (rank == 1)
        {
            _highCards = cards.OrderByDescending(c => c.cardInfo.cardNumber)
                                  .Take(5).ToList();
            highValueCards = _highCards;
        }

        return rank;
    }


    HandEvaluation CheckStraightFlush(List<Card> cards, HandEvaluation result)
    {
        var flushedCards = cards.GroupBy(c => c.cardInfo.CardType).Where(g => g.Count() >= 5).SelectMany(g => g).Distinct().ToList();
        if (flushedCards.Count >= 5 && IsStraight(flushedCards))
        {
            result.HandValue = 9;
            result.HighValueCards = flushedCards;
        }
        return result;
    }

    HandEvaluation UpdateFlushResult(List<Card> cards, HandEvaluation result)
    {
        result.HandValue = 6;
        result.HighValueCards = cards.GroupBy(c => c.cardInfo.CardType).Where(g => g.Count() >= 5).SelectMany(g => g).ToList();
        return result;
    }

    HandEvaluation UpdateStraightResult(List<Card> uniqueCards, HandEvaluation result)
    {
        result.HandValue = 5;
        result.HighValueCards = GetStraightCards(uniqueCards);
        return result;
    }

    bool IsStraight(List<Card> cards)
    {
        var uniqueCards = cards.GroupBy(c => c.cardInfo.cardNumber).Select(g => g.First()).ToList();
        for (int i = 0; i <= uniqueCards.Count - 5; i++)
            if (Enumerable.Range(0, 5).All(j => uniqueCards[i + j].cardInfo.cardNumber == uniqueCards[i].cardInfo.cardNumber + j))
                return true;

        return uniqueCards.Count >= 5 &&
               uniqueCards[0].cardInfo.cardNumber == 2 &&
               uniqueCards[^1].cardInfo.cardNumber == 14 &&
               uniqueCards.Skip(1).Take(4).Select((c, j) => c.cardInfo.cardNumber == 10 + j).All(x => x);
    }

    bool CheckFlush(List<Card> cards)
    {
        return cards.GroupBy(c => c.cardInfo.CardType).Any(g => g.Count() >= 5);
    }

    List<Chamber> BreakTie(List<Chamber> tiedChambers)
    {
        tiedChambers.Sort((x, y) => CompareHighCards(x.chamberRankCards, y.chamberRankCards));
        return tiedChambers.TakeWhile(c => CompareHighCards(c.chamberRankCards, tiedChambers[0].chamberRankCards) == 0).ToList();
    }
    int CompareHighCards(List<Card> first, List<Card> second)
    {
        for (int i = 0; i < Mathf.Min(first.Count, second.Count); i++)
        {
            if (first[i].cardInfo.cardNumber != second[i].cardInfo.cardNumber)
                return second[i].cardInfo.cardNumber.CompareTo(first[i].cardInfo.cardNumber);
        }
        return 0; // Cards are equal
    }
    /*  List<Chamber> BreakTie(List<Chamber> tiedChambers)
      {
          print($"Breaking Tie : {tiedChambers.Count}");

          // Sort the cards in each winningChamber by descending card value
          var sortedChambers = tiedChambers.Select(winningChamber =>
              new
              {
                  Chamber = winningChamber,
                  SortedCards = winningChamber.chamberRankCards.OrderByDescending(card => card.cardInfo.cardNumber).ToList()
              }).ToList();

          // Now we need to compare the cards of each winningChamber one by one
          var highestChamber = sortedChambers.First();

          // Loop through each card position to compare the chambers
          foreach (var cardIndex in Enumerable.Range(0, highestChamber.SortedCards.Count))
          {
              // Find the highest card value at the current position
              int currentMaxValue = highestChamber.SortedCards[cardIndex].cardInfo.cardNumber;

              // Check if any winningChamber has a higher card at this position
              highestChamber = sortedChambers
                  .Where(sc => sc.SortedCards[cardIndex].cardInfo.cardNumber == currentMaxValue)
                  .FirstOrDefault(); // pick the one with the highest card at the current chamberIndex
          }

          // Now return the chambers that match the highest card at all chamberIndex positions
          return sortedChambers.Where(sc =>
              sc.SortedCards.Zip(highestChamber.SortedCards, (scCard, highestCard) =>
                  scCard.cardInfo.cardNumber == highestCard.cardInfo.cardNumber)
              .All(match => match)
          ).Select(sc => sc.Chamber).ToList();
      }
  */
    List<Card> GetStraightCards(List<Card> cards)
    {
        var orderedCards = cards.OrderBy(c => c.cardInfo.cardNumber).ToList();
        for (int i = 0; i <= orderedCards.Count - 5; i++)
            if (orderedCards.Skip(i).Take(5).Select((c, j) => c.cardInfo.cardNumber == orderedCards[i].cardInfo.cardNumber + j).All(x => x))
                return orderedCards.Skip(i).Take(5).ToList();

        return orderedCards.Count >= 5 && orderedCards[0].cardInfo.cardNumber == 2 &&
               orderedCards[^1].cardInfo.cardNumber == 14 &&
               orderedCards.Skip(1).Take(4).Select((c, j) => c.cardInfo.cardNumber == 10 + j).All(x => x)
            ? new List<Card> { orderedCards[^1] }.Concat(orderedCards.Skip(1).Take(4)).ToList()
            : new List<Card>();
    }
}

public class HandEvaluation
{
    public int HandValue { get; set; }
    public List<Card> HighValueCards { get; set; }
}
