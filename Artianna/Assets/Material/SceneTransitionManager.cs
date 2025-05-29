using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance; // Синглтон для глобального доступа

    public Image fadeImage; // Image для затемнения экрана
    public float fadeDuration = 1.0f; // Длительность затемнения/осветления

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем объект при смене сцены
        }
        else
        {
            Destroy(gameObject);
        }

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(false); // Отключаем Image при старте
        }
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(PerformSceneTransition(sceneName));
    }

    private IEnumerator PerformSceneTransition(string sceneName)
    {
        // Включаем затемнение
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f)); // Затемняем экран
        }

        // Уменьшаем громкость
        yield return StartCoroutine(FadeAudio(1f, 0f));

        // Загружаем следующую сцену
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Осветляем экран
        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(1f, 0f)); // Осветляем экран
            fadeImage.gameObject.SetActive(false); // Выключаем Image
        }

        // Восстанавливаем громкость
        yield return StartCoroutine(FadeAudio(0f, 1f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    private IEnumerator FadeAudio(float startVolume, float endVolume)
    {
        float elapsedTime = 0f;

        AudioListener.volume = startVolume;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        AudioListener.volume = endVolume;
    }
}
