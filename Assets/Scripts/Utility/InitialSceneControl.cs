using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class InitialSceneControl : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;

    void Start()
    {
        StartCoroutine(ChangeScene(1));
    }

    IEnumerator ChangeScene(int nextScene)
    {
        progressSlider.value = 0f;
        yield return new WaitForSeconds(1f);

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float progress = 0f;

        while (!op.isDone)
        {
            progress = Mathf.MoveTowards(progress, op.progress, Time.deltaTime);
            progressSlider.value = progress;

            if (progress >= 0.9f)
            {
                progressSlider.value = 1f;
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}


