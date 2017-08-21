using UnityEngine;
using System.Reflection;

[System.Serializable]
public class MetaMethod 
{
	[SerializeField]
    private GameObject _gameObjectTarget;
	[SerializeField]
    private string _methodName;

	private MethodInfo mi;
	private Object target;

	public GameObject gameObjectTarget {
		get {
			return _gameObjectTarget;
		}
		set {
			_gameObjectTarget = value;
			ApplyReflection();
		}
	}

	public string methodName {
		get {
			return _methodName;
		}
		set {
			_methodName = value;
			ApplyReflection();
		}
	}

	protected void ApplyReflection()
	{
		string[] part = _methodName.Split('.');
		target = _gameObjectTarget.GetComponent(part[0]);
		if( target && !string.IsNullOrEmpty(_methodName) )
			mi = target.GetType().GetMethod( part[1],   BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
			                                			BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly  );
	}

	public void Invoke( params object[] args )
	{
		if( mi == null ) ApplyReflection();
		mi.Invoke( target, args );
	}
}
