using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // To restart the scene

public class GameManager : MonoBehaviour
{
    private bool isPaused = false;
    public SetAndRoundManager setAndRoundManager;
    public PlayerManager playerHandManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
        PlayerPrefs.SetInt("PlayerPoint", setAndRoundManager.pointsQuota);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
