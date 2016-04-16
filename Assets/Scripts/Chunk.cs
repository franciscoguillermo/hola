using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    void Update()
    {
        if (transform.position.x < CameraFollow.LeftEdge - 25)
            ObjectPool.Push(gameObject);
    }
}