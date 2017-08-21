using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static bool HasInstance()
    {
        return _instance != null && !applicationIsQuitting;
    }

    public static T GetInstance()
    {
        if (applicationIsQuitting)
        {
            Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                "' already destroyed on application quit." +
                " Won't create again - returning null.");

            throw new System.Exception("Tentativa de acesso ao (singleton)"+ typeof(T) + " com a cena descarregada!");
        }

        if (_instance == null)
        {
            _instance = (T)FindObjectOfType(typeof(T));

#if UNITY_EDITOR
            if (FindObjectsOfType(typeof(T)).Length > 1)
            {
                Debug.LogError("[Singleton] Something went really wrong " +
                    " - there should never be more than 1 singleton!" +
                    " Reopening the scene might fix it.");
                return _instance;
            }
#endif

            if ( _instance == null )
            {
                GameObject singleton = new GameObject();
                _instance = singleton.AddComponent<T>();
                singleton.name = "(singleton) " + typeof(T).ToString();
                Debug.Log("[Singleton] Uma nova instancia do (Singleton)" + typeof(T) + " foi requisitada na cena.");
            }
        }

        return _instance;
    }

    protected static bool applicationIsQuitting = false;


    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = (this as T);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneUnloaded += SceneUnload;
    }

    protected virtual void OnDestroy()
    {
        SceneManager.sceneUnloaded -= SceneUnload;
    }

    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    protected virtual void SceneUnload( Scene scene )
    {
        DestroyImmediate(gameObject);
    }
}
