using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public Tutorial[] tutorialObjects;
    public GameObject tutorialObject;
    public TMP_Text tutorialText;
    private void Awake()
    {
        Instance = this;
    }
    public Tutorial GetTutorial(TutorialType _tutorialType)
    {
        foreach (Tutorial tutorial in tutorialObjects)
        {
            if (tutorial.tutorialType == _tutorialType)
            {
                return tutorial;
            }
        }
        return null;
    }

    public void ShowTutorial(TutorialType _tutorialType)
    {
        Tutorial tutorial = GetTutorial(_tutorialType);
        tutorialText.text = tutorial.tutorialMessage;
        tutorialObject.SetActive(true);

    }

    public void HideTutorial(TutorialType _tutorialType)
    {
        Tutorial tutorial = GetTutorial(_tutorialType);
        tutorialText.text = "";
        tutorialObject.SetActive(false);
    }
}
[System.Serializable]
public class Tutorial {

    public TutorialType tutorialType;

    public string tutorialMessage;

}

public enum TutorialType { 

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