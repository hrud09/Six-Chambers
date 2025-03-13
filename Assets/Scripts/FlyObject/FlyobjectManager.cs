using System;
using System.Collections;
using System.Threading.Tasks;
using Farbood.Model;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class FlyobjectManager : MonoBehaviour
{
    private static FlyobjectManager instance;

    [SerializeField] private FlyingObject flyingObjectPrefab;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static FlyobjectManager GetInstance()
    {
        return instance;
    }

    public async Task CreateFlyObjects(int objectCount, RectTransform start, RectTransform target, FlyObjectType rewardType,
                                        Action callbackOnEachItem, Action completeCallback)
    {
        GetComponent<Canvas>().worldCamera = UiManager.GetInstance().GetUiController().GetUICamera();
        UiManager uiManager = UiManager.GetInstance();
        int spawnRadious = 20;
        Vector2 spawnDelayRange = new Vector2(0.01f, 0.1f);
        int waitTimer = 0;
        for (int i = 0; i < objectCount; i++)
        {
            var flyingObject = Instantiate(flyingObjectPrefab, start.position, Quaternion.identity, transform);
        flyingObject.GetComponent<Image>().sprite = uiManager.GetFlyObjectIcon(rewardType);
            RectTransform rect = flyingObject.GetComponent<RectTransform>();
            rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x + Random.Range(-spawnRadious, spawnRadious),
                                                  rect.anchoredPosition3D.y + Random.Range(-spawnRadious, spawnRadious), 0);
            _ = flyingObject.Fly(target.position, callbackOnEachItem);
            waitTimer = (int)(Random.Range(spawnDelayRange.x, spawnDelayRange.y) * 1000);
            await Task.Delay(waitTimer);
        }

        StartCoroutine(CompleteCallBackInDelay(waitTimer, () =>
        {
            completeCallback?.Invoke();
        }));
    }

    IEnumerator CompleteCallBackInDelay(int timer, Action completeCallback)
    {
        yield return new WaitForSeconds(1);
        completeCallback?.Invoke();
    }
}


