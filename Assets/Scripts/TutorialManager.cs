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
    public float typingSpeed = 0.05f; // Speed of the typing effect (in seconds per character)

    private Coroutine typingCoroutine;

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
        if (tutorial == null) return;

        // Stop any existing typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Set the tutorial message and activate the object
        tutorialObject.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(tutorial.tutorialMessage));
    }

    public void HideTutorial(TutorialType _tutorialType)
    {
        Tutorial tutorial = GetTutorial(_tutorialType);
        if (tutorial == null) return;

        // Stop the typing coroutine if it's running
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Clear the text and hide the tutorial object
        tutorialText.text = "";
        tutorialObject.SetActive(false);
    }

    private IEnumerator TypeText(string message)
    {
        tutorialText.text = ""; // Clear the text initially
        foreach (char letter in message.ToCharArray())
        {
            tutorialText.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(typingSpeed); // Wait before adding the next letter
        }
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