using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public CardManager cardManager;
    public ChamberManager chamberManager;
    public PlayerManager playerManager;
    public RangerManager rangerManager;

    void Start()
    {
        if (!cardManager || !chamberManager || !playerManager || !rangerManager)
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


        if (Input.GetKeyDown(KeyCode.A))
        {
            PowerData powerData = GameManager.GetInstance().GetPowerData();
            powerData.SetPowerState(PowerType.Ghost, true);
            powerData.SetPowerState(PowerType.Investigate, true);
            powerData.SetPowerState(PowerType.Crystal_Ball, true);
            SaveLoadManager.SavePowerData(powerData);
        }

    }

    private void ExecuteMouseDownAction()
    {
        
        if (GameplayManager.GetInstance().GetCurrentGameState() == GameState.DealingChamberCards)
        {
            GameplayManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.DrawCardsForChambers();
        }
        else if (GameplayManager.GetInstance().GetCurrentGameState() == GameState.DealingBoardCards1)
        {
            GameplayManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.DrawCardsOnBoard();
        }
        else if (GameplayManager.GetInstance().GetCurrentGameState() == GameState.DealingBoardCards2)
        {
            GameplayManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.DrawAnotherCardOnBoard();

        }
        else if (GameplayManager.GetInstance().GetCurrentGameState() == GameState.RevealingChamberCards)
        {
            GameplayManager.GetInstance().SetGameState(GameState.NoInputState);
            chamberManager.RevealAllHandCardsAtOnce();
        }
        else if (GameplayManager.GetInstance().GetCurrentGameState() == GameState.CollectingAllCards)
        {
            GameplayManager.GetInstance().SetGameState(GameState.NoInputState);
            cardManager.CollectAllCards();
        }
    }

}
