// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using UnityEngine;
using System.Collections;

public class MethodFilterAttribute : PropertyAttribute {

	public string[] methods;

	public MethodFilterAttribute( params object[] list )
	{
		this.methods = new string[list.Length];
		for (int i = 0; i < list.Length; i++)
		{
			this.methods[i] = list[i].ToString();
		}
	}

}