using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public CardManager cardManager;
    public BoardManager boardManager;
    public ChamberManager chamberManager;
    public PlayerManager playerManager;
    public RangerManager rangerManager;

    void Start()
    {
        if (!cardManager || !boardManager || !chamberManager || !playerManager || !rangerManager)
        {
            Debug.LogError("PlayerInputManager: One or more references are missing!");
        }
    }

    void Update()
    {
        HandleKeyboardInputs();
    }

    private void HandleKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteMouseDownAction();
        }
    }

    private void ExecuteMouseDownAction()
    {
        
        if (GameManager.GetInstance().GetCurrentGameState() == GameState.DealingChamberCards)
        {
            GameManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.DrawCardsForChambers();
        }
        else if (GameManager.GetInstance().GetCurrentGameState() == GameState.DealingBoardCards1)
        {
            GameManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.DrawCardsOnBoard();
        }
        else if (GameManager.GetInstance().GetCurrentGameState() == GameState.DealingBoardCards2)
        {
            GameManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.DrawAnotherCardOnBoard();

        }
        else if (GameManager.GetInstance().GetCurrentGameState() == GameState.RevealingChamberCards)
        {
            GameManager.GetInstance().SetGameState(GameState.NoInputState);
            chamberManager.RevealAllHandCardsAtOnce();
        }
        else if (GameManager.GetInstance().GetCurrentGameState() == GameState.CollectingAllCards)
        {
            GameManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.CollectAllCards();
        }
    }

}
