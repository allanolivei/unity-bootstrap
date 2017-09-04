// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    public bool rotateWithCamera = false;

    private Transform cacheTr;
    private Transform camTr;

    private void Start()
    {
        cacheTr = GetComponent<Transform>();
        camTr = Camera.main.transform;
    }

    private void Update()
    {
        cacheTr.rotation = rotateWithCamera ? 
            camTr.rotation :
            Quaternion.LookRotation( cacheTr.position - camTr.position, Vector3.up );
    }
}
