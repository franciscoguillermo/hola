using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParalaxElement : MonoBehaviour
{
    public Transform First;
    public Transform Second;
    public Transform Target;
    public float YOffset;

    [Range(0, 1)]
    public float ParallaxXFactor;
    public float ParallaxYFactor;
    private float _width = 32;
    private int _loopCount = 0;

    void Update()
    {
        Vector3 newPostion = new Vector3(Target.position.x * ParallaxXFactor + _width * _loopCount, Target.position.y * ParallaxYFactor + YOffset, 2);

        First.position = newPostion;
        Second.position = First.position + new Vector3(_width, 0);

        if (First.position.x < CameraFollow.LeftEdge - _width / 2)
            _loopCount++;
    }
}