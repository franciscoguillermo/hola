using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorChildrenSprites : MonoBehaviour
{
    public Color SpriteColor;

    void OnValidate()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = SpriteColor;
        }
    }
}