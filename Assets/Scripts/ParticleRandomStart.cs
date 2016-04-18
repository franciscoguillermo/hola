using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleRandomStart : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(FrameDelay());
    }

    IEnumerator FrameDelay()
    {
        yield return null;
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.time = Random.Range(0, ps.duration);
    }
}