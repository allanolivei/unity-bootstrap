using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class NormalizeSpriteUV : MonoBehaviour
{
    void Start()
    {
        CreateNormalizedUVMatrix();
    }
    
#if UNITY_EDITOR
    void Update()
    {
        CreateNormalizedUVMatrix();
    }
#endif

    void CreateNormalizedUVMatrix()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        Renderer rend = GetComponent<Renderer>();
        rend.GetPropertyBlock(mpb);
        Sprite sp = GetComponent<SpriteRenderer>().sprite;
        
        Rect r = sp.rect;
        r.x /= sp.texture.width;
        r.y /= sp.texture.height;
        r.width /= sp.texture.width;
        r.height /= sp.texture.height;

        Vector3 scale = new Vector3((1.0f / r.width), 1.0f / r.height - 2, 1);
        Matrix4x4 uv0ToRect =
            Matrix4x4.TRS(new Vector3(-r.x * scale.x, -r.y * scale.y - 1, 0), Quaternion.identity, scale);

        mpb.SetMatrix("_TextureRotation", uv0ToRect);
        rend.SetPropertyBlock(mpb);

        //lightMaterial.SetMatrix("_TextureRotation", uv0ToRect);
    }


}
