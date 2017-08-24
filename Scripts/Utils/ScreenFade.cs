using UnityEngine;
using System.Collections;

public class ScreenFade : MonoBehaviour
{

	public float fadeTime = 2.0f;
	public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

	private Material fadeMaterial = null;
	private bool isFading = false;
	private YieldInstruction fadeInstruction = new WaitForEndOfFrame();


	private void Awake()
	{
		fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
    }

    private void OnDestroy()
    {
        if (fadeMaterial != null)
            Destroy(fadeMaterial);
    }

    private void OnEnable()
	{
		StartCoroutine("FadeIn");
	}
    
#if UNITY_5_4_OR_NEWER
	private void OnLevelFinishedLoading(int level)
#else
	private void OnLevelWasLoaded(int level)
#endif
	{
		StartCoroutine( "FadeIn" );
	}

    private void OnPostRender()
    {
        if ( isFading )
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(fadeMaterial.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
        }
    }

    private IEnumerator FadeIn()
	{
		float elapsedTime = 0.0f;
		fadeMaterial.color = fadeColor;
		Color color = fadeColor;
		isFading = true;
		while (elapsedTime < fadeTime)
		{
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime;
			color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
			fadeMaterial.color = color;
		}
		isFading = false;
	}
}
