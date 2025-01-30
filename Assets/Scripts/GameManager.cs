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
    NoInputState
}
public class GameManager : MonoBehaviour
{
    private static GameManager Instance;
    public GameState currentRoundState;
    private bool isPaused = false;
    public SetAndRoundManager setAndRoundManager;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static GameManager GetInstance()
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
       setAndRoundManager.EndRound();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ReloadGameScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void RestartGame()
    {
        setAndRoundManager.ResetAll();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
