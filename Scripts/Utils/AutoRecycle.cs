﻿// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using UnityEngine;
using System.Collections;

public class AutoRecycle : MonoBehaviour
{

    public float duration = 2.0f;
    
    public IObjectPool pool;

    public void OnEnable()
    {
        StartCoroutine("WaitForRecycle");
    }

    IEnumerator WaitForRecycle()
    {
        yield return new WaitForSeconds(duration);

        if (pool != null) pool.Recycle(this);
        this.gameObject.SetActive(false);
    }
    


}
