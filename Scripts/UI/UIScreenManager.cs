using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : Singleton<UIScreenManager>
{

    public List<UIScreen> screens;
    [HideInInspector]
    public Stack<UIScreen> history;
    [HideInInspector]
    public UIScreen currentScreen;

    public UIScreen startScreen;

    public UIScreen GetScreenByPartName(string name)
    {
        foreach (UIScreen sc in screens)
            if (sc.name.ToLower().Contains(name.ToLower())) return sc;
        return default(UIScreen);
    }

    public UIScreen GetScreen( string name )
    {
        foreach (UIScreen sc in screens)
            if (sc.name == name) return sc;
        return default(UIScreen);
    }

    public UIScreen GetScreen( System.Type typeClass )
    {
        foreach (UIScreen sc in screens)
            if (sc.GetType() == typeClass) return sc;
        return default(UIScreen);
    }

    public T GetScreen<T>() where T : UIScreen
    {
        foreach (UIScreen sc in screens)
            if (sc is T) return (T)sc;
        return default(T);
    }

    public T GetScreen<T>(string objName)
    {
        foreach (UIScreen sc in screens)
            if (sc.name == objName) return (T)System.Convert.ChangeType(sc, typeof(T));
        return default(T);
    }

    public void ClearScreen()
    {
        if (currentScreen != null) currentScreen.Hide();
        history.Clear();
        currentScreen = null;
    }

    public void HideAll()
    {
        foreach (UIScreen sc in screens)
            sc.Hide();
        history.Clear();
        currentScreen = null;
    }
    
    public T SetScreen<T>() where T : UIScreen
    {
        T screen = GetScreen<T>();
        SetScreen(screen);
        return screen;
    }

    public UIScreen SetScreen( string objName )
    {
        UIScreen screen = GetScreenByPartName(objName);
        SetScreen(screen);
        return screen;
    }

    public void SetScreen( UIScreen screen )
    {
        if (currentScreen != null)  currentScreen.Hide();
        currentScreen = screen;
        if ( screen != null) currentScreen.Show();
    }

    public T AddScreen<T>() where T : UIScreen
    {
        T screen = GetScreen<T>();
        AddScreen(screen);
        return screen;
    }

    public UIScreen AddScreen( string objName )
    {
        UIScreen screen = GetScreenByPartName(objName);
        AddScreen(screen);
        return screen;
    }

    public void AddScreen( UIScreen screen )
    {
        if( currentScreen != null )
        {
            //currentScreen.Hide();
            history.Push(currentScreen);
        }

        currentScreen = screen;
        if (screen != null) currentScreen.Show();
    }

    public void BackScreen()
    {
        if( currentScreen != null )
        {
            currentScreen.Hide();
            currentScreen = null;
        }

        if( history.Count > 0 ) SetScreen( history.Pop() );
    }

    protected override void Awake()
    {
        base.Awake();

        screens = new List<UIScreen>(gameObject.GetComponentsInChildren<UIScreen>(true));
        history = new Stack<UIScreen>();
        
        HideAll();
    }

    protected void Start()
    {
        if( startScreen != null)
            SetScreen(startScreen);
    }

}
