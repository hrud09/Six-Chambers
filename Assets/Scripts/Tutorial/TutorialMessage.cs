using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Text = TMPro.TextMeshProUGUI;

public class TutorialMessage : MonoBehaviour
{
    private static TutorialMessage instance;
    public static TutorialMessage Instance => instance;
    [SerializeField] private Image darkBg;
    [SerializeField] private Button button;
    [SerializeField] private Canvas tutorialCanvas;

    public Button Button => button;
    [SerializeField] private RectTransform elderCharacter;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease elderShowEase = Ease.Linear;
    [SerializeField] private Ease elderHideEase = Ease.Linear;
    [Space]

    [SerializeField] private RectTransform messageBubble;
    [SerializeField] private Ease messageBubbleShowEase = Ease.Linear;
    [SerializeField] private Ease messageBubbleHideEase = Ease.Linear;
    [SerializeField] private Text messageBubbleText;

    [Space]
    [SerializeField] private Text tapToContinueText;
    [SerializeField] private float delay = 2f;
    [Space]
    [SerializeField] private RectTransform tutorialCompleteRect;
    [SerializeField] private ParticleSystem firework;
    [SerializeField] private List<Transform> fireworks;
    private Tween tapTween;

    private Vector3 elderstartPosition = new Vector3(-450, 0, 0);
    private Vector3 elderendPosition = new Vector3(0, 0, 0);
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        elderCharacter.anchoredPosition3D = elderstartPosition;
       // messageBubble.Scale(0);
        button.interactable = false;
    }

    public void SetText(string text, float size)
    {
        this.messageBubbleText.text = text;
        this.messageBubbleText.fontSize = size;
    }

    private Vector3 initialScale = new Vector3(0, 1, 1);
    private Vector3 endScale = Vector3.one;

    public void ShowTutorialMessage(string message, float fontsize = 42,Action OnStart = null, Action OnComplete = null)
    {
        
        tutorialCanvas.sortingOrder = 3;
        SetText(message, fontsize);
        darkBg.gameObject.SetActive(true);
        darkBg.color = new Color(darkBg.color.r, darkBg.color.g, darkBg.color.b, 0f);
        darkBg.DOFade(0.85f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            DOTween.To(() => elderstartPosition, x => elderCharacter.anchoredPosition3D = x, elderendPosition, duration).SetEase(elderShowEase)
                 .OnStart(()=> OnStart?.Invoke())
                 .OnComplete(() =>
                 {
                     //AudioManager.CallPlaySFX(Sound.TutorialShow);
                     DOTween.To(() => initialScale, x => messageBubble.localScale = x, Vector3.one, duration).SetEase(messageBubbleShowEase)
                     .OnComplete(() =>
                     {
                         DOVirtual.DelayedCall(delay, () =>
                         {
                             tapTween = tapToContinueText.DOFade(1, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
                             button.interactable = true;
                             OnComplete?.Invoke();
                         }).SetUpdate(true);


                     }).SetUpdate(true);
                 }).SetUpdate(true);
        }).SetUpdate(true);

    }
    private WaitForSeconds waitime = new WaitForSeconds(0.2f);

    public void ShowTutorialCompleted(System.Action onComplete = null)
    {
        StartCoroutine(TutorialComplete(onComplete));
    }
    private IEnumerator TutorialComplete(System.Action onComplete = null)
    {
        tutorialCanvas.sortingOrder = 0;

        //AudioManager.CallPlaySFX(Sound.TutorialComplete);

        tutorialCompleteRect.DOScale(1, 0.5f).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(0.5f);

        //foreach (var f in fireworks)
        //{
        //    //AudioManager.CallPlaySFX(Sound.Confetti);
        //    Instantiate(firework, f);

        //    yield return waitime;
        //}

        onComplete?.Invoke();
        yield return new WaitForSeconds(1f);
        tutorialCompleteRect.DOScale(0.01f, 0.5f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            tutorialCompleteRect.gameObject.SetActive(false);
            tutorialCanvas.sortingOrder = -1;
        });
        // yield return new WaitForSeconds(1f);
    }

    public void HideTutorialMessage(System.Action OnComplete = null)
    {

        button.interactable = false;
        DOTween.To(() => Vector3.one, x => messageBubble.localScale = x, initialScale, duration).SetEase(messageBubbleHideEase)
            .OnComplete(() =>
            {
                DOTween.To(() => elderendPosition, x => elderCharacter.anchoredPosition3D = x, elderstartPosition, duration).SetEase(elderHideEase)
                .OnStart(() =>
                {
                    darkBg.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() =>
                    {
                        tapTween.Kill();
                        tapToContinueText.alpha = 0;
                        darkBg.gameObject.SetActive(false);
                        tutorialCanvas.sortingOrder = -1;
                        OnComplete?.Invoke();
                    }).SetUpdate(true);
                }).SetUpdate(true);
            }).SetUpdate(true);


    }
}
