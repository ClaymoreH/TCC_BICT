using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance; 
    public Image fadeImage; 
    public float fadeDuration = 1f; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false;
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(FadeAndSwitchScene(sceneIndex));
    }

    private IEnumerator FadeAndSwitchScene(int sceneIndex)
    {
        fadeImage.raycastTarget = true;

        yield return StartCoroutine(Fade(1));

        SceneManager.LoadScene(sceneIndex);

        yield return StartCoroutine(Fade(0));

        fadeImage.raycastTarget = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}
