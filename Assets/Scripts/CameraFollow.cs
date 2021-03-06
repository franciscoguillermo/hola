﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    private static CameraFollow _instance;

    public Transform Target;

    public static float RightEdge
    {
        get { return _instance.transform.position.x + _instance._camera.aspect * _instance._camera.orthographicSize; }
    }

    public static float LeftEdge
    {
        get { return _instance.transform.position.x - _instance._camera.aspect * _instance._camera.orthographicSize; }
    }

    [SerializeField]
    private Camera _camera;

    void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        if (Target != null)
            transform.position = Target.position;
    }
}