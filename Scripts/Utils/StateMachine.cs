using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class StateMachine : MonoBehaviour
{
    public enum DEFAULT
    {
        NOTHING
    }

    public class StateData
    {
        public System.Action Enter;
        public System.Action Step;
        public System.Action Exit;
    }

    public System.Enum _currentState;
    public StateData currentStateData;
    public UnityEvent StateChangeHandler;
    public float stateTime;
#if UNITY_EDITOR
    public string stateDebug;
#endif

    private Dictionary<System.Enum, StateData> states = new Dictionary<System.Enum, StateData>();


    public System.Enum currentState
    {
        get { return _currentState; }
        set
        {
            StateData data;
            if( states.TryGetValue( value, out data ) )
            { 
                if (currentStateData != null) currentStateData.Exit();

                _currentState = value;
                currentStateData = data;

                stateTime = Time.time;

                currentStateData.Enter();

                StateChangeHandler.Invoke();

#if UNITY_EDITOR
                stateDebug = currentState.ToString();
#endif
            }
        }
    }

    public virtual void Awake()
    {
        this.Register(DEFAULT.NOTHING);
        this.currentState = DEFAULT.NOTHING;
    }

    public virtual void Update()
    {
        currentStateData.Step();
    }

    public void Register( System.Enum stateType, System.Action enter = null, System.Action step = null, System.Action exit = null )
    {
        StateData data;

        if (states.TryGetValue(stateType, out data)) states.Remove(stateType);
        else data = new StateData();
        
        data.Enter = enter != null ? enter : EmptyMethod;
        data.Step = step != null ? step : EmptyMethod;
        data.Exit = exit != null ? exit : EmptyMethod;

        states.Add(stateType, data);
    }

    public void Register(System.Enum stateType, StateData stateData)
    {
        states.Add(stateType, stateData);
    }

    protected void EmptyMethod() { }
	
}
