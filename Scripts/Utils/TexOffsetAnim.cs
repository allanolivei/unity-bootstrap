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
