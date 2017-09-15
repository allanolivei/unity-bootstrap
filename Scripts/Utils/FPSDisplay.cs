
using UnityEngine;
using UnityEngine.EventSystems;

public class FPSDisplay : MonoBehaviour
{

#if UNITY_EDITOR// || !(UNITY_ANDROID && UNITY_IOS)

    float deltaTime = 0.0f;
    private const string format = "{0:0.0} ms ({1:0.} fps)\nScreen: {2}x{3} : {4}dpi\nTimeScale: {5}";

    [Header("Aperte = (igual) para mostrar/esconder o display")]
    [Space]
    public bool showDisplay = true;
    [Range(0, 1)]
    public float posX;
    [Range(0, 1)]
    public float posY;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        if (Input.GetKeyDown(KeyCode.Equals) && !EventSystem.current.currentSelectedGameObject)
        {
            showDisplay = !showDisplay;
        }
    }

    void OnGUI()
    {
        if (showDisplay)
        {
            //int w = Screen.width, h = Screen.height;

            //GUIStyle style = new GUIStyle();

            // Rect rect = new Rect(0, 0, w, h * 2 / 100);
            //style.alignment = TextAnchor.UpperLeft;
            //style.fontSize = h * 2 / 100;
            //style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            //string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            //GUI.Label(rect, text, style);
            string text = string.Format(format, msec, fps, Screen.width, Screen.height, Screen.dpi, Time.timeScale);
            GUI.Box(new Rect((Screen.width - 200) * posX, (Screen.height - 50) * posY, 200, 50), text);

            //GUILayout.Label("Screen: " + Screen.width + "x" + Screen.height + " : " + Screen.dpi + "dpi");
        }
    }
#endif

}