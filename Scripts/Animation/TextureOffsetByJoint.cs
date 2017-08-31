using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureOffsetByJoint : MonoBehaviour
{

    public Transform joint;
    public Material material;
    public int columns = 5;
    public int lines = 10;

    private float lastChangeTime;
    private int currentIndex = 0;

    private void Update()
    {
        if ( joint == null || material == null ) return;

        int newIndex = Mathf.RoundToInt(joint.localScale.x);

        if( newIndex != currentIndex && Time.time - lastChangeTime > 0.5f )
        {
            currentIndex = newIndex;
            lastChangeTime = Time.time;
            this.ApplyOffset();
        }

    }

    private void ApplyOffset()
    {
        float sizeX = 1 / (float)columns;
        float sizeY = 1 / (float)lines;
        material.mainTextureOffset = new Vector2(currentIndex % columns * sizeX, -Mathf.Floor(currentIndex / columns) * sizeY);
    }

}
