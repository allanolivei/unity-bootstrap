// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Cheat {
	public string title;
	public string key;
	public System.Action method;
	public Cheat( string title, string key, System.Action method )
	{
		this.title = title;
		this.key = key;
		this.method = method;
	}
}

public class KeyCodeCheats : MonoBehaviour {

	private static KeyCodeCheats instance;
	
	public static bool Exist()
	{
		return instance != null;
	}
	
	public static KeyCodeCheats GetInstance() 
	{
		if( instance == null ) {
			instance = (KeyCodeCheats) GameObject.FindObjectOfType<KeyCodeCheats>();
			if( instance == null )
			{
				GameObject g = new GameObject( "KeyCodeCheats" );
				instance = g.AddComponent<KeyCodeCheats>();
			}
		}

		return instance;
	}


	private const float timeForKeyDown = 5.0f;

	[System.NonSerialized]
	public List<Cheat> cheats = new List<Cheat>();

	public string currentSequence;
	private float previousTime;
	private List<string> message = new List<string>();
	private GUIStyle style;

	public void AddMessage( string message )
	{
		this.message.Add( message );
		StartCoroutine("RemoveMessage");
	}

	void Awake()
	{
		if( instance != null && instance != this  ) 
		{
			Destroy( this );
			return;
		}
		
		instance = this;
		
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		ClearCheats();
	}

	void OnGUI()
	{
		if( Event.current.type == EventType.keyDown && Event.current.keyCode != KeyCode.None )
		{
			float currentTime = Time.realtimeSinceStartup;

			if( ( GUIUtility.hotControl != 0) && currentSequence.Length == 0 ) 
			{
				ClearCheats();
				return;
			}

			if( currentSequence.Length == 0 || currentTime - previousTime < timeForKeyDown )
			{
				currentSequence += Event.current.keyCode.ToString().ToLower();
				bool contain = false;

				do{
					foreach( Cheat s in cheats )
					{
						if( s.key.StartsWith( currentSequence ) )
						{
							contain = true;
							if( s.key == currentSequence ) 
							{
								ApplyCheat( s );
								ClearCheats();
								break;
							}
						}
					}

					if( !contain )
					{
						if( GUIUtility.hotControl != 0 ) ClearCheats();
						else currentSequence = currentSequence.Substring( 1, currentSequence.Length-1 );
					}

				} while( !contain && !string.IsNullOrEmpty(currentSequence) );

				if( !contain ) 
					ClearCheats();
			}
			else
				ClearCheats();

			previousTime = Time.realtimeSinceStartup;
		}

		if( style == null )
		{
			style = new GUIStyle( GUI.skin.label );
			style.fontSize = 20;
			style.normal.textColor = Color.green;
			style.fontStyle = FontStyle.Bold;
		}

		string result = string.Empty;
		foreach( string s in message )
		{
			result += s+"\n";
		}

		GUI.Label( new Rect( 10, Screen.height - 30 * message.Count, 300, 30 * message.Count ), result, style );

	}

	void ApplyCheat( Cheat cheat )
	{
		if( cheat.method != null ) cheat.method();
		AddMessage( "Cheat "+cheat.title+" Activated" );

	}

	void ClearCheats()
	{
		currentSequence = string.Empty;
		previousTime = Time.realtimeSinceStartup;
	}

	private IEnumerator RemoveMessage()
	{
		float time = Time.realtimeSinceStartup;
		while( Time.realtimeSinceStartup - time < 5.0f )
			yield return null;

		message.RemoveAt(0);
	}

}
