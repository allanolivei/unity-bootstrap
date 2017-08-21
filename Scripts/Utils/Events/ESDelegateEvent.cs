using UnityEngine;
using System.Collections;
using System.Reflection;

namespace ES
{
	public class ESDelegateEvent : MonoBehaviour 
	{
		[MethodFilter( "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit", 
		               "OnCollisionEnter", "OnCollisionStay", "OnCollisionExit" )]
		public MetaMethod[] methods;

		public void OnTriggerEnter(Collider other) { InvokeMethod("OnTriggerEnter", other); }
		public void OnTriggerStay(Collider other) { InvokeMethod("OnTriggerStay", other); }
		public void OnTriggerExit(Collider other) { InvokeMethod("OnTriggerExit", other); }
		public void OnCollisionEnter(Collision other) { InvokeMethod("OnCollisionEnter", other); }
		public void OnCollisionStay(Collision other) { InvokeMethod("OnCollisionStay", other); }
		public void OnCollisionExit(Collision other) { InvokeMethod("OnCollisionExit", other); }

		public void InvokeMethod( string methodName, params object[] args )
		{
			foreach( MetaMethod mm in methods )
				if( mm.methodName.Contains(methodName) ) mm.Invoke(args);
		}
	}
}