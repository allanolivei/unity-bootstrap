// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class TexOffsetAnim : MonoBehaviour {

    public Material mat;
    public Vector2 speed;

    public void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    public void Update()
    {
        Vector2 offset = mat.mainTextureOffset;
        offset += speed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }

}
