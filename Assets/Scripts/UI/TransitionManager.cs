using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager instance;
    private const int maskScaleFactor = 30;
    [SerializeField]private TMP_Text loadingText;

    [SerializeField]
    private RectTransform maskImage;

    public static event Action OnLevelTransitionComplete;

    private GameManager _gameManager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        _gameManager = GameManager.GetInstance();
    }

    public static TransitionManager GetInstance()
    {
        return instance;
    }

    public void ChangeLevel(SCENE_NUM sceneNum)
    {
        FadeOut(() =>
        {
            StartCoroutine(AsyncLoadScene((int)sceneNum, null));
        });
    }

    private IEnumerator AsyncLoadScene(int index, Action callback)
    {
        var scene = SceneManager.LoadSceneAsync(index);
        while (!scene.isDone)
        {
            yield return null;
        }
        OnLevelTransitionComplete?.Invoke();
        FadeIn(null);
    }

    public void FadeOut(Action callback)
    {
        maskImage.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);
        loadingText.transform.localScale = Vector3.zero;
        loadingText.transform.DOScale(Vector3.one, 0.2f);
        maskImage.DOScale(Vector3.one * maskScaleFactor, 0.5f).OnComplete(()=> {
            if(callback != null)
            {
                callback();
            }
        });
    }

    public void FadeIn(Action callback)
    {
        loadingText.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
        {
            loadingText.gameObject.SetActive(false);
        });
        maskImage.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
            if (callback != null)
            {
                callback();
            }
            maskImage.gameObject.SetActive(false);
        });
    }
}