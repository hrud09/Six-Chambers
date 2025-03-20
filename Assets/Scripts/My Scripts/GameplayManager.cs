using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // To restart the scene

public enum GameState
{
    RoundStarted,
    DealingChamberCards,
    DealingBoardCards1,
    PlayersTurn,
    RangersTurn,
    SameChamberSelected,
    DealingBoardCards2,
    RevealingChamberCards,
    EvaluatingHands,
    Dueling,
    CollectingAllCards,
    RoundEnded,
    NoInputState,
    PowerInUse
}
public class GameplayManager : MonoBehaviour
{
    private static GameplayManager Instance;
    public GameState currentRoundState;
    private bool isPaused = false;
    public RoundManager roundManager;
    public PowerManager powerManager;
    public CardManager cardManager;
    PlayerEconomyManager playerEconomyManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerEconomyManager = FindObjectOfType<PlayerEconomyManager>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public PlayerEconomyManager GetPlayerEconomy()
    {
        if(playerEconomyManager == null) playerEconomyManager = FindObjectOfType<PlayerEconomyManager>();
        return playerEconomyManager;
    }
    private void OnEnable()
    {
        roundManager = FindObjectOfType<RoundManager>();
        powerManager = FindObjectOfType<PowerManager>();
    }
    public static GameplayManager GetInstance()
    {
        return Instance;
    }
   
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        SetGameState(GameState.DealingChamberCards);
    }

    public void SetGameState(GameState _currentRoundState)
    {
        currentRoundState = _currentRoundState;
    }
    public GameState GetCurrentGameState()
    {
        return currentRoundState;
    }
   
    public void OnRoundEnd()
    {
       roundManager.EndRound();

    }


    public void RestartGame()
    {
        ClearData.DeleteFiles();
        roundManager.ResetProgress();
        playerEconomyManager.ResetEconomy();
        foreach (Power item in powerManager.purchasedPowers)
        {
            item.InitiatePower();
        }
        // PlayerEconomyManager.Instance.UpdateCredit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
