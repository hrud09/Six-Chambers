using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private List<TutorialInfo> allTutorials = new List<TutorialInfo>();
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private float typingSpeed = 0.05f;

    private const string tutorialString = "isTutorialFinished";
    private const string tutorialID = "TUTORIAL_ID_";

    private int currentTutorialId = 0;
    private bool isTutorialFinished = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        isTutorialFinished = PlayerPrefs.GetInt(tutorialString, 0) == 1;
    }

    private IEnumerator Start()
    {
        yield return null;
        StartTutorial();
    }

    public int CurrentTutorialID
    {
        get => PlayerPrefs.GetInt(tutorialID, 1);
        set => PlayerPrefs.SetInt(tutorialID, value);
    }

    public void StartTutorial(bool applyDelay = false, float delay = 0f)
    {
        if (CurrentTutorialID - 1 >= allTutorials.Count || isTutorialFinished)
            return;

        if (applyDelay)
        {
            DOVirtual.DelayedCall(delay, () => allTutorials[CurrentTutorialID - 1].OnTutorialStart());
        }
        else
        {
            allTutorials[CurrentTutorialID - 1].OnTutorialStart();
        }
    }

    public void StartTutorial(int id)
    {
        if (id - 1 < allTutorials.Count)
            allTutorials[id - 1].OnTutorialStart();
    }

    public void EndTutorial(int id, bool applyDelay = false)
    {
        if (id - 1 < allTutorials.Count)
        {
            allTutorials[id - 1].OnTutorialEnd();

            if (applyDelay)
            {
                DOVirtual.DelayedCall(1.25f, () => EndTutorialBehavior(id));
            }
            else
            {
                EndTutorialBehavior(id);
            }
        }
    }

    private void EndTutorialBehavior(int id)
    {
        CurrentTutorialID++;

        if (CurrentTutorialID - 1 >= allTutorials.Count)
        {
            PlayerPrefs.SetInt(tutorialString, 1);
            isTutorialFinished = true;
            return;
        }

        if ((CurrentTutorialID == 3 && GameManager.GetInstance().GetPlayerData().currentLevelId != 3) ||
            (CurrentTutorialID == 4 && GameManager.GetInstance().GetPlayerData().currentLevelId != 6) ||
            (CurrentTutorialID == 5 && GameManager.GetInstance().GetPlayerData().currentLevelId != 10))
        {
            return;
        }

        StartTutorial();
    }

    public bool IsTutorialFinished() => isTutorialFinished;
    public bool IsTutorialEnded(int id) => CurrentTutorialID > id;

    public void ShowTutorial(TutorialType tutorialType)
    {
        TutorialInfo tutorial = GetTutorial(tutorialType);
        if (tutorial == null) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        tutorialObject.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(tutorial.tutorialMessage));
    }

    public void HideTutorial()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        tutorialText.text = "";
        tutorialObject.SetActive(false);
    }

    private IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            tutorialText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private TutorialInfo GetTutorial(TutorialType tutorialType)
    {
        return allTutorials.Find(tutorial => tutorial.tutorialType == tutorialType);
    }
}

[System.Serializable]
public class TutorialInfo
{
    public TutorialType tutorialType;
    public string tutorialMessage;

    public void OnTutorialStart()
    {
        TutorialManager.Instance.ShowTutorial(tutorialType);
    }

    public void OnTutorialEnd()
    {
        TutorialManager.Instance.HideTutorial();
    }
}

public enum TutorialType
{
    None,
    DealCardsToChamber,
    DealCardsOnTable,
    PlayersTurn,
    RangersTurn,
    DealNextCard,
    RevealAllHand,
    ShowDown,
    NextRoundStarter
}
