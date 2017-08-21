using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;

namespace ES
{
	public class ESTriggerEvent : MonoBehaviour {

	    public enum EVENT_TYPE
	    {
	        OnTriggerEnter,
	        OnTriggerStay,
	        OnTriggerExit,
	        OnCollisionEnter,
	        OnCollisionStay,
	        OnCollisionExit
	    }

	    [System.Serializable]
	    public class Entry
	    {
	        public UnityEvent callback;
	        public EVENT_TYPE eventType;
	    }

		public List<ESTriggerEvent.Entry> triggers;

	    private void Dispatch(EVENT_TYPE eventType, Object ob = null)
	    {
			foreach (ESTriggerEvent.Entry entry in triggers)
	        {
	            if (entry.eventType == eventType) entry.callback.Invoke();
	        }
	    }

	    //Unity Actions
	    private void OnTriggerEnter(Collider other) { Dispatch(EVENT_TYPE.OnTriggerEnter); }
	    private void OnTriggerStay(Collider other) { Dispatch(EVENT_TYPE.OnTriggerStay); }
	    private void OnTriggerExit(Collider other) { Dispatch(EVENT_TYPE.OnTriggerExit); }
	    private void OnCollisionEnter(Collision other) { Dispatch(EVENT_TYPE.OnCollisionEnter); }
	    private void OnCollisionStay(Collision other) { Dispatch(EVENT_TYPE.OnCollisionStay); }
	    private void OnCollisionExit(Collision other) { Dispatch(EVENT_TYPE.OnCollisionExit); }
	}
}
