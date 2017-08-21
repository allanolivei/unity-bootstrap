using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScript : MonoBehaviour {

    public GameObject[] objts;
    
    void Start()
    {
        StartCoroutine("Splash");
    }

    IEnumerator Splash()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(WaitForSecondsUnscale( 1.0f, objts[0],1f));
        yield return new WaitForSeconds(2);
        StartCoroutine(WaitForSecondsUnscale(1.0f, objts[0], 0));
        yield return new WaitForSeconds(1);
        StartCoroutine(WaitForSecondsUnscale(1.0f, objts[1], 1f));
        yield return new WaitForSeconds(2);
        StartCoroutine(WaitForSecondsUnscale(1.0f, objts[1], 0));
        yield return new WaitForSeconds(1.5f);
        
        SceneManager.LoadScene("MainMenu");
        
    }

    IEnumerator WaitForSecondsUnscale(float duration , GameObject go, float toDo)
    {
        float initTime = Time.unscaledTime;
        Color a = go.GetComponent<Image>().color;
        float init = a.a;
        //Debug.Log(a);
        while (true)
        {
            
            float percent = (Time.unscaledTime - initTime) / duration;

            go.GetComponent<Image>().color = Color.Lerp(new Color(1,1,1,init), new Color(1, 1, 1, toDo), percent);
           // Debug.Log(new Color(1, 1, 1, init) +" <<<<<<COLOR>>>>  "+ new Color(1, 1, 1, toDo));
            if (percent >= 1.0f) 
            {
                a = new Color(1, 1, 1, toDo);
                    break;
            }
            yield return null;
        }

    }
}
