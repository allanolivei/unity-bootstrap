using UnityEngine;
using System.Collections;

public class DestroyDelay : MonoBehaviour
{

    public float seconds = 2.0f;
    
	IEnumerator Start ()
    {
        yield return new WaitForSeconds( seconds );

        Destroy(gameObject);
	}
}
