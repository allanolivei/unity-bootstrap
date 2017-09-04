// Copyright (c) 2017-2018 Allan Oliveira Marinho, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mouse2ShaderRadius : MonoBehaviour
{
	
	private Renderer rend;
	private Camera cam;

	private void Start()
	{
		rend = GetComponent<Renderer> ();
		cam = Camera.main;
	}

	private float radius
	{
		get {
			return rend.sharedMaterial.HasProperty ("_Radius") ? 
				rend.sharedMaterial.GetFloat ("_Radius"):
				0.08f;
		}
	}
	
	private void Update()
	{
		bool hasHit = false;
		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		foreach (RaycastHit hit in Physics.SphereCastAll (ray, radius)) 
		{
			if (hit.transform.gameObject != gameObject) continue;

			Vector3 point = ray.origin + ray.direction * (hit.distance+radius);
			rend.material.SetVector ("_Center", new Vector4 (point.x, point.y, point.z, 0));
			hasHit = true;
		}

		if( !hasHit ) 
			rend.material.SetVector ("_Center", new Vector4 (ray.origin.x, ray.origin.y, ray.origin.z, 0));
	}
		
	#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if (!rend) return;

		Gizmos.color = Color.yellow;

		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		foreach (RaycastHit hit in Physics.SphereCastAll (ray, radius))
			if (hit.transform.gameObject == gameObject)
				Gizmos.DrawLine (ray.origin, ray.origin + ray.direction * (hit.distance + radius));
	}
	#endif

}
