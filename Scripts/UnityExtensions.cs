// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{

    public static void ChangeSharedMaterials( this Renderer rend, params Material[] materials )
    {
        Material[] current = rend.sharedMaterials;
        int total = Mathf.Min(current.Length, materials.Length);
        for( int i = 0 ; i < total ; i++ )
        {
            //copy data
            if (current[i].HasProperty("_MainTex"))
                materials[i].mainTexture = current[i].mainTexture;
            if( current[i].HasProperty("_Color") )
                materials[i].color = current[i].color;
            //override
            current[i] = materials[i];
        }
        //apply
        rend.sharedMaterials = current;
    }

}
