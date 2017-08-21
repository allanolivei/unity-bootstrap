using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

[CustomPropertyDrawer (typeof (MethodFilterAttribute)),
 CustomPropertyDrawer (typeof(MetaMethod))]
public class MethodFilterDrawer : PropertyDrawer 
{
    //private string[] options;
	private int index = 0;
	private Dictionary<UnityEngine.Object, string[]> options = new Dictionary<UnityEngine.Object, string[]>();

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight;
	}

	public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label) 
	{
		SerializedProperty target = prop.FindPropertyRelative("_gameObjectTarget");
        SerializedProperty methodName = prop.FindPropertyRelative("_methodName");

        pos = EditorGUI.PrefixLabel(pos, label);
		label = EditorGUI.BeginProperty(pos, label, prop);

        if( target.objectReferenceValue == null )
        {
            EditorGUI.PropertyField(pos, target, GUIContent.none);
        }
        else
        {
            float half = pos.width * 0.4f;
			
			//prop 01
            EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(new Rect(pos.x, pos.y, half, pos.height), target, GUIContent.none);
			UnityEngine.Object ob = target.objectReferenceValue;
			if( EditorGUI.EndChangeCheck() || !options.ContainsKey(ob) || options[ob] == null )
			{
				MethodFilterAttribute p = attribute as MethodFilterAttribute;
				string[] opt = FindMethods( target.objectReferenceValue as GameObject, p != null ? p.methods : null );
				if( options.ContainsKey(target.objectReferenceValue) )
					options.Add( ob, opt );
				else
					options[ob] = opt;
			}

			//prop 02
			EditorGUI.BeginChangeCheck();
			index = Mathf.Max(0, Array.IndexOf( options[ob], methodName.stringValue ));
			index = EditorGUI.Popup(
				new Rect(pos.x + half, pos.y, pos.width - half, pos.height),
				index,
				options[ob]);
			if( EditorGUI.EndChangeCheck() )
				methodName.stringValue = options[ob][index];
        }

		EditorGUI.EndProperty();
	}

	private string[] FindMethods( GameObject target, string[] methodName = null )
	{
		List<string> ops = new List<string>();
		foreach (Component cp in target.GetComponents<Component>())
		{
			Type t = cp.GetType();
			MethodInfo[] ms = t.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
			                                BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly );
			foreach( MethodInfo m in ms )
				if( methodName == null || Array.IndexOf(methodName, m.Name) != -1 ) ops.Add( t.ToString()+"."+m.Name );
		}
		return ops.ToArray();
	}
}