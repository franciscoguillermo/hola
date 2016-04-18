using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterWiggle : MonoBehaviour
{
    [SerializeField]
    private Transform[] _controlPoints;
    private float[] _velocities;
    private float _maxLocalY = 30f;
    private float _minLocalY = -20f;
    private float _restingLocalY = 9f;
    private float _dampen = 5.0f;

    void Awake()
    {
        _velocities = new float[_controlPoints.Length];
    }

    void Update()
    {
        for (int i = 0; i < _controlPoints.Length; i++)
        {
            _velocities[i] = Mathf.Lerp(_velocities[i], _restingLocalY - _controlPoints[i].localPosition.y, Time.deltaTime * _dampen);

            Vector3 newPos = _controlPoints[i].localPosition;
            float y = Mathf.Clamp(_controlPoints[i].localPosition.y + _velocities[i], _minLocalY, _maxLocalY);
            newPos.y = y;
            _controlPoints[i].localPosition = newPos;
        }
    }

    public void Splash(Vector3 position, Vector3 velocity)
    {
        Vector3 localPoint = transform.InverseTransformPoint(position);

        for (int i = 0; i < _velocities.Length; i++)
        {
            float inverseDistance = 0.2f / Mathf.Max(1, Mathf.Abs(_controlPoints[i].localPosition.x - localPoint.x));
            _velocities[i] += velocity.y * inverseDistance;
        }
    }
}