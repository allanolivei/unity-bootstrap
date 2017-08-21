using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{

    public float delay = 2.5f;

    private void OnEnable()
    {
        Destroy(gameObject, delay);
    }
	
}
