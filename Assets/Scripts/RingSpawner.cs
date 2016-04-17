using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RingSpawner : MonoBehaviour
{
    static GameObject _ringPrefab;

    void Awake()
    {
        if (_ringPrefab == null)
            _ringPrefab = Resources.Load<GameObject>("Pooled/ring");
    }

    public void Spawn()
    {
        ObjectPool.Pop<RingTrigger>(_ringPrefab).Spawn(transform.position);
    }
}