using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    RingSpawner[] _ringSpawners;

    void Awake()
    {
        _ringSpawners = GetComponentsInChildren<RingSpawner>();
    }

    public void Spawn(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;

        for (int i = 0; i < _ringSpawners.Length; i++)
            _ringSpawners[i].Spawn();
    }

    void Update()
    {
        if (transform.position.x < CameraFollow.LeftEdge - 25)
            ObjectPool.Push(gameObject);
    }
}