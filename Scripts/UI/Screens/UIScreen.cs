using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class CompleteEvent : UnityEvent<UIScreen> {}


[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    public CompleteEvent CompleteEvent;

    public virtual void Show()
    {
        StopAllCoroutines();

        gameObject.SetActive(true);
        if (gameObject.activeInHierarchy)
            StartCoroutine("_ShowCoroutine");
    }

    public virtual void Hide()
    {
        StopAllCoroutines();

        if( gameObject.activeInHierarchy )
            StartCoroutine("_HideCoroutine");
    }

    public void AddScreenByName( string objName )
    {
        UIScreenManager.GetInstance().AddScreen(objName);
    }

    public void AddScreenByType<T>() where T : UIScreen
    {
        UIScreenManager.GetInstance().AddScreen<T>();
    }

    public void PrevScene()
    {
        this.LoadScene( UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1 );
    }

    public void NextScene()
    {
        this.LoadScene( UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void LoadScene( string sceneName )
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public bool IsShow()
    {
        return this.gameObject.activeInHierarchy;
    }
	
    public void BackScreen()
    {
        UIScreenManager.GetInstance().BackScreen();
    }

    protected virtual IEnumerator _ShowCoroutine()
    {
        yield return StartCoroutine(_AnimateState(1.0f, 0.3f));
    }

    protected virtual IEnumerator _HideCoroutine()
    {
        yield return StartCoroutine(_AnimateState(0.0f, 0.05f));
        gameObject.SetActive(false);
    }

    protected virtual IEnumerator _AnimateState ( float value, float duration )
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.interactable = value > 0;

        float initTime = Time.unscaledTime,
              initAlpha = cg.alpha,
              percent = 0;
        while (percent < 1.0f)
        {
            percent = Mathf.Min(1.0f, (Time.unscaledTime - initTime) / duration);
            cg.alpha = Mathfx.Sinerp(initAlpha, value, percent);
            yield return null;
        }
    }
}
