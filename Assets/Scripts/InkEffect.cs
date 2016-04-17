using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class InkEffect : MonoBehaviour
{
    public RenderTexture PreviousFrameBuffer;
    public Material InkBleedMaterial;

    private float _bleedOutDuration = 3.0f;
    private float _defaultFadeMagnitude = 0.075f;
    private float _deathFadeMagnitude = -0.05f;

    void Start()
    {
        InkBleedMaterial.SetFloat("_InkFadeMag", _defaultFadeMagnitude);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        RenderTexture temp = RenderTexture.GetTemporary(src.width, src.height, 0);

        InkBleedMaterial.SetTexture("_PrevFrame", PreviousFrameBuffer);
        Graphics.Blit(src, temp, InkBleedMaterial);
        Graphics.Blit(temp, PreviousFrameBuffer);
        Graphics.Blit(temp, dst);

        RenderTexture.ReleaseTemporary(temp);
    }

    public void BleedOut()
    {
        StartCoroutine(BleedOutAsync());
    }

    IEnumerator BleedOutAsync()
    {
        float fade = _defaultFadeMagnitude;

        for (float t = 0; t < _bleedOutDuration; t += Time.deltaTime)
        {
            float normalT = t / _bleedOutDuration;

            fade = Mathf.Lerp(_defaultFadeMagnitude, _deathFadeMagnitude, normalT);
            InkBleedMaterial.SetFloat("_InkFadeMag", fade);

            yield return null;
        }
    }
}