using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
    public GameObject Prefab;
    public bool AutoCleanup;
    public float CleanupTime;
    public int DefaultPoolSize;

    float _spawnTime;

    public float TimeRemaining
    {
        get
        {
            return Mathf.Max(0, (CleanupTime + _spawnTime) - Time.time);
        }
    }

    void Awake()
    {
        enabled = AutoCleanup;
    }

    public void Pop()
    {
        if (AutoCleanup)
            _spawnTime = Time.time;
    }

    void Update()
    {
        if (_spawnTime + CleanupTime <= Time.time)
            ObjectPool.Push(this);
    }
}