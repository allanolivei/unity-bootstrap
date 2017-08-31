using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour
{

    public CanvasGroup[] objts;
    public string sceneName;

    void Start()
    {
        StartCoroutine("Splash");
    }

    IEnumerator Splash()
    {
        for (int i = 0; i < objts.Length; i++)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(WaitForSecondsUnscale(1.0f, objts[i], 1f));
            yield return new WaitForSeconds(2);
            StartCoroutine(WaitForSecondsUnscale(1.0f, objts[i], 0));
        }
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator WaitForSecondsUnscale(float duration, CanvasGroup cGroup, float toDo)
    {
        float initTime = Time.unscaledTime;
        float init = cGroup.alpha;
        while (true)
        {
            float percent = (Time.unscaledTime - initTime) / duration;
            cGroup.alpha = Mathf.Lerp(init, toDo, percent);
            if (percent >= 1.0f)
            {
                cGroup.alpha = toDo;
                break;
            }
            yield return null;
        }
    }
}
