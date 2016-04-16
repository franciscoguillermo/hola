using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParalaxElement : MonoBehaviour
{
    public Transform First;
    public Transform Second;
    public Transform Target;

    [Range (0,1)]
    public float ParalaxFactor;
    private float _width = 32;

	void Update ()
	{
        First.position = Target.position * ParalaxFactor + Vector3.forward;
        Second.position = Target.position * ParalaxFactor + Vector3.forward;

        if (First.position.x < CameraFollow.LeftEdge - _width/2)
            First.position += Vector3.right * _width;

        if (Second.position.x < CameraFollow.LeftEdge - _width/2)
            Second.position += Vector3.right * _width;
    }
}