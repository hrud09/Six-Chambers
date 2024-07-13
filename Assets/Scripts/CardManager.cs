using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;

public enum CardType
{
    Clove,
    Heart,
    Dice,
    Spade
}

[System.Serializable]
public class CardInfo
{
    public CardType CardType;
    public int cardNumber;
    public Sprite cardTexture;
}

[System.Serializable]
public class SignWiseCard
{
    public CardType CardType;
    public Sprite[] sprites;
}

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;

    public List<SignWiseCard> signWiseCards;

    public List<CardInfo> cardsInDeck;
    public List<CardInfo> tempDeck;
    public List<CardInfo> cardsInHands;

    public List<Transform> cardsOnHands;

    public ChamberManager chamberManager;
    public BoardManager boardManager;
    public Transform cardSpawnPos;
    public GameObject showCardButton;

    public TMP_Text winText;

    void Start()
    {
        foreach (var cardDeck in signWiseCards)
        {
            foreach (var cardSprite in cardDeck.sprites)
            {
                CardInfo cardInfo = new CardInfo
                {
                    cardTexture = cardSprite,
                    CardType = cardDeck.CardType,
                    cardNumber = cardDeck.sprites.ToList().IndexOf(cardSprite) + 2
                };
                cardsInDeck.Add(cardInfo);
            }
        }
        tempDeck = new List<CardInfo>(cardsInDeck);
        StartCoroutine(DrawCardsForChambers());
        StartCoroutine(DrawCardsOnBoard());
    }

    private CardInfo DrawRandomCard()
    {
        int index = Random.Range(0, tempDeck.Count);
        CardInfo cardInfo = tempDeck[index];
        tempDeck.RemoveAt(index);

        return cardInfo;
    }

    public IEnumerator DrawCardsForChambers()
    {
        foreach (var chamberTransform in chamberManager.chamberTransforms)
        {
            Chamber chamber = chamberTransform.GetComponent<Chamber>();
            CardInfo cardInfo1 = DrawRandomCard();
            CardInfo cardInfo2 = DrawRandomCard();

            cardsInHands.Add(cardInfo1);
            cardsInHands.Add(cardInfo2);

            Vector3 targetDirection = (chamberTransform.parent.position - chamberTransform.position).normalized;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            GameObject card1 = Instantiate(cardPrefab, cardSpawnPos.position, targetRotation, chamber.cardParent);
            GameObject card2 = Instantiate(cardPrefab, cardSpawnPos.position, targetRotation, chamber.cardParent);
            cardsOnHands.Add(card1.transform);
            cardsOnHands.Add(card2.transform);
            card1.GetComponent<Card>().InitiateCard(cardInfo1);
            card2.GetComponent<Card>().InitiateCard(cardInfo2);

            chamber.chamberCards.Add(card2.GetComponent<Card>());
            chamber.chamberCards.Add(card1.GetComponent<Card>());

            Vector3 pos1 = chamber.cardParent.position;
            Vector3 pos2 = pos1 - chamberTransform.forward * 1f + Vector3.up * 0.2f;

            if (chamberTransform.localPosition.x > 0) card1.transform.localRotation = Quaternion.Euler(new Vector3(0, 20, 180));
            else card1.transform.localRotation = Quaternion.Euler(new Vector3(0, -20, 180));

            card1.transform.DOMove(pos1, 1f);
            card2.transform.DOMove(pos2, 1f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator DrawCardsOnBoard()
    {
        for (int i = 0; i < boardManager.boardCardParents.Length; i++)
        {
            CardInfo cardInfo = DrawRandomCard();
            GameObject card = Instantiate(cardPrefab, cardSpawnPos.position, Quaternion.Euler(0, 0, 180), boardManager.boardCardParents[i]);
            card.GetComponent<Card>().InitiateCard(cardInfo);

            Vector3 pos = boardManager.boardCardParents[i].position;
            boardManager.cardsOnBoard.Add(card);
            boardManager.cards.Add(card.GetComponent<Card>());
            int index = i;
            card.transform.DOMove(pos, 1f).OnComplete(() =>
            {
                if (index < 3) card.transform.DOLocalRotate(Vector3.zero, 1f);
            });

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1);
        chamberManager.playerChipsManager.playersTurn = true;
    }

    public void RevealLastTwoCards()
    {
        for (int i = 0; i < boardManager.cardsOnBoard.Count; i++)
        {
            int index = i;
            Transform card = boardManager.cardsOnBoard[i].transform;

            card.DOLocalRotate(Vector3.zero, 0.5f).SetDelay(0.2f).OnComplete(() =>
            {
                if (index == boardManager.cardsOnBoard.Count - 1)
                {
                    RevealEveryOnesCards();
                }
            });
        }
    }

    public void RevealEveryOnesCards()
    {
        foreach (Transform t in cardsOnHands)
        {
            int index = cardsOnHands.IndexOf(t);
            t.DOLocalRotate(Vector3.zero, 0.5f).SetDelay(cardsOnHands.IndexOf(t) * 0.03f).OnComplete(() =>
            {
                if (index == cardsOnHands.Count - 1)
                {
                    CheckPokerLogics();
                }
            });
        }
    }

    void CheckPokerLogics()
    {
        List<Card> cardsOnBoard = boardManager.cards;
        Chamber winningChamber = null;
        List<Chamber> winningChambers;
        int highestHandValue = -1;
        List<Chamber> tiedChambers = new List<Chamber>();

        foreach (Transform t in chamberManager.chamberTransforms)
        {
            Chamber chamberSelected = t.GetComponent<Chamber>();

            List<Card> chamberCards = new List<Card>(cardsOnBoard);
            chamberCards.AddRange(chamberSelected.chamberCards);

            HandEvaluation handEvaluation = EvaluateHand(chamberCards);
            int handValue = handEvaluation.HandValue;

          /*  foreach (Card card in handEvaluation.HighValueCards)
            {
                GameObject newCard = Instantiate(card.gameObject, card.gameObject.transform.position, card.transform.rotation, chamberSelected.bestCardsHolders[handEvaluation.HighValueCards.IndexOf(card)]);
                newCard.transform.DOScale(Vector3.one, 0.5f);
                newCard.transform.DOLocalMove(Vector3.zero, 1f).OnComplete(() => {
                    newCard.transform.DOLocalRotate(Vector3.zero, 0.5f);
                });
            }*/

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
            winningChambers = BreakTie(tiedChambers, highestHandValue);
            if (winningChambers.Count == 1)
            {
                HandEvaluation winningHand = EvaluateHand(new List<Card>(cardsOnBoard.Concat(winningChambers[0].chamberCards)));
                
                winText.text = $"{winningChambers[0].index} Wins by Tie-Breaker among Chambers: {tiedChambersIndices} by {winType(highestHandValue)}";
            }
            else
            {
                string message = string.Join(", ", winningChambers.Select(chamber => chamber.index.ToString()).ToArray());
                message += " Wins by " + winType(highestHandValue).ToString();
                winText.text = message;
            }
        }
    }

        List<Chamber> BreakTie(List<Chamber> tiedChambers, int handValue)
        {
            List<Chamber> tiedWinners = new List<Chamber>();

            switch (handValue)
            {
                case 1:
                    tiedWinners = BreakHighCardTie(tiedChambers);
                    break;
                case 2:
                    tiedWinners = BreakOnePairTie(tiedChambers);
                    break;
                case 3:
                    tiedWinners = BreakTwoPairTie(tiedChambers);
                    break;
                case 4:
                    tiedWinners = BreakThreeOfAKindTie(tiedChambers);
                    break;
                case 5:
                    tiedWinners = BreakStraightTie(tiedChambers);
                    break;
                case 6:
                    tiedWinners = BreakFlushTie(tiedChambers);
                    break;
                case 7:
                    tiedWinners = BreakFullHouseTie(tiedChambers);
                    break;
                case 8:
                    tiedWinners = BreakFourOfAKindTie(tiedChambers);
                    break;
                case 9:
                    tiedWinners = BreakStraightFlushTie(tiedChambers);
                    break;
                default:
                    break;
            }

            return tiedWinners;
        }

        List<Chamber> BreakHighCardTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetHighestCardValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakOnePairTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetPairValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakTwoPairTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetTwoPairValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakThreeOfAKindTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetThreeOfAKindValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakStraightTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetHighestCardValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakFlushTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetHighestCardValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakFullHouseTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetThreeOfAKindValue(chamber.chamberCards)).ThenByDescending(chamber => GetPairValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakFourOfAKindTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetFourOfAKindValue(chamber.chamberCards)).ToList();
        }

        List<Chamber> BreakStraightFlushTie(List<Chamber> tiedChambers)
        {
            return tiedChambers.OrderByDescending(chamber => GetHighestCardValue(chamber.chamberCards)).ToList();
        }

        int GetHighestCardValue(List<Card> cards)
        {
            return cards.Max(card => card.cardInfo.cardNumber);
        }

        int GetPairValue(List<Card> cards)
        {
            var groups = cards.GroupBy(card => card.cardInfo.cardNumber);
            var pairGroup = groups.FirstOrDefault(group => group.Count() == 2);
            if (pairGroup != null)
            {
                return pairGroup.Key;
            }
            return 0; // Return 0 or handle case where no pair is found
        }

        int GetTwoPairValue(List<Card> cards)
        {
            var groups = cards.GroupBy(card => card.cardInfo.cardNumber);
            var pairs = groups.Where(group => group.Count() == 2).Select(group => group.Key).ToList();

            if (pairs.Count == 2)
            {
                // Sort pairs in descending order and return the highest pair value
                pairs.Sort((a, b) => b.CompareTo(a));
                return pairs[0];
            }

            return 0; // Return 0 or handle case where two pairs are not found
        }

        int GetThreeOfAKindValue(List<Card> cards)
        {
            var groups = cards.GroupBy(card => card.cardInfo.cardNumber);
            var threeOfAKind = groups.FirstOrDefault(group => group.Count() == 3);

            if (threeOfAKind != null)
            {
                return threeOfAKind.Key;
            }

            return 0; // Return 0 or handle case where three of a kind is not found
        }

        int GetFourOfAKindValue(List<Card> cards)
        {
            var groups = cards.GroupBy(card => card.cardInfo.cardNumber);
            var fourOfAKind = groups.FirstOrDefault(group => group.Count() == 4);

            if (fourOfAKind != null)
            {
                return fourOfAKind.Key;
            }

            return 0; // Return 0 or handle case where four of a kind is not found
        }

        // Additional methods for Straight and Flush are not needed for tie-breaking, as the highest card determines the winner

        // Add methods for additional hand ranks (Straight, Flush, etc.) if necessary

    HandEvaluation EvaluateHand(List<Card> cards)
    {
        List<Card> sortedCards = cards.OrderBy(card => card.cardInfo.cardNumber).ToList();
        List<Card> sortedCardWithoutDuplicates = cards.OrderBy(card => card.cardInfo.cardNumber).Distinct().ToList();

        HandEvaluation result = new HandEvaluation();
        result.HandValue = 0;

        bool isStraight = IsStraight(sortedCardWithoutDuplicates);
        bool isFlush = CheckFlush(sortedCards);


        int tempHandValue = GetMultiplesValue(sortedCards, out List<Card> highValueCards);
        
        if (isStraight && isFlush)
        {
            List<Card> flushedCards = cards
      .GroupBy(card => card.cardInfo.CardType)
      .Where(group => group.Count() >= 5)
      .SelectMany(group => group.ToList())
      .ToList();

            flushedCards.Distinct();
            if (flushedCards.Count >=5 && IsStraight(flushedCards))
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
}

public class HandEvaluation
{
    public int HandValue { get; set; }
   // public List<Card> HighValueCards { get; set; }
}