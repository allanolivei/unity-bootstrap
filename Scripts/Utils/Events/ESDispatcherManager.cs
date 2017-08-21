using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace ES
{

	public class ESDispatcherManager : Singleton<ESDispatcherManager>
	{

        

	    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

        

        /*
	    public override void Awake()
	    {
            if( _instance != null && _instance != this )
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                base.Awake();
            }

            //if (eventManager == null) eventManager = this;

            //Debug.Log("Dont Destroy on Load");
            //Debug.Log("TESTE");
        }

	    public void OnLevelWasLoaded(int level)
	    {
	        //RemoveAllListener();
	    }
    */

	    public void AddListener(string eventName, UnityAction listener)
	    {
	        UnityEvent thisEvent = null;
	        if (eventDictionary.TryGetValue(eventName, out thisEvent))
	        {
	            thisEvent.AddListener(listener);
	        }
	        else
	        {
	            thisEvent = new UnityEvent();
	            thisEvent.AddListener(listener);
                eventDictionary.Add(eventName, thisEvent);
	        }
	    }

	    public void RemoveListener(string eventName, UnityAction listener)
	    {
	        UnityEvent thisEvent = null;
	        if (eventDictionary.TryGetValue(eventName, out thisEvent))
	        {
	            thisEvent.RemoveListener(listener);
	        }
	    }

	    public void RemoveListener(UnityAction listener)
	    {
	        foreach (KeyValuePair<string, UnityEvent> ev in eventDictionary)
	        { 
	            ev.Value.RemoveListener(listener);
	        }
	    }

	    public void RemoveListener(string eventName)
	    {
	        UnityEvent thisEvent = null;
	        if (eventDictionary.TryGetValue(eventName, out thisEvent))
	            thisEvent.RemoveAllListeners();
	    }

	    public void RemoveAllListener()
	    {
	        foreach (KeyValuePair<string, UnityEvent> ev in eventDictionary)
	            ev.Value.RemoveAllListeners();
	        eventDictionary.Clear();
	    }

		public void Dispatch( string eventName )
		{
			UnityEvent thisEvent = null;
			if (eventDictionary.TryGetValue(eventName, out thisEvent))
				thisEvent.Invoke();
		}
	}

}