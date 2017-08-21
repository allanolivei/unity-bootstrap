using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private Transform cacheTr;
    private Transform camTr;

    void Start()
    {
        cacheTr = GetComponent<Transform>();
        camTr = Camera.main.transform;
    }

    void Update()
    {
        cacheTr.rotation = camTr.rotation;
    }
}
