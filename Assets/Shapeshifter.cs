using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimalForms
{
    Wolf,
    Bird,
    Fish
}

public class Shapeshifter : MonoBehaviour
{
    // All Forms Stuff
    private AnimalForms _form = AnimalForms.Wolf;
    private Vector3 _velocity;

    // Wolf Stuff
    [SerializeField]
    private Transform[] _wolfFrontTransforms;
    private float _wolfFrontAngle;
    [SerializeField]
    private Transform _wolfFrontRayOrigin;
    [SerializeField]
    private Transform[] _wolfBackTransforms;
    private float _wolfBackAngle;
    [SerializeField]
    private Transform _wolfBackRayOrigin;

    private Vector3 _wolfGravity = new Vector3(0, -60);
    private bool _wolfGrounded;
    private float _wolfGroundCheck = 1.0f;
    private float _wolfHeight = 0.6f;
    private float _wolfJumpSpeed = 14.0f;
    private float _wolfJumpSustainFactor = .4f;
    private float _wolfGroundRotSmoothFactor = 10.0f;
    private float _wolfAirRotSmoothFactor = 2.0f;
    private float _wolfBaseRunSpeed = 14.0f;

    void Awake()
    {

    }

    void Update()
    {
        switch (_form)
        {
            case AnimalForms.Wolf:
                {
                    WolfFormUpdate();
                    break;
                }
            case AnimalForms.Bird:
                {
                    break;
                }
            case AnimalForms.Fish:
                {
                    break;
                }
        }

        AllFormsUpdate();
    }

    void AllFormsUpdate()
    {
        transform.Translate(_velocity * Time.deltaTime);
    }

    void WolfFormUpdate()
    {
        WolfGravity();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _wolfGroundCheck);
        _wolfGrounded = hit.collider != null;

        if (_wolfGrounded)
        {
            if (_velocity.y < 0)
            {
                transform.position = (Vector3)hit.point + new Vector3(0, _wolfHeight, 0);
                _velocity.y = 0;
            }

            if (Input.GetButtonDown("Primary Action"))
                WolfJump();
        }

        WolfRotation();
        _velocity.x = _wolfBaseRunSpeed;
    }

    void WolfJump()
    {
        _wolfGrounded = false;
        _velocity.y = _wolfJumpSpeed;
    }

    void WolfGravity()
    {
        float sustain = 1.0f;
        if (_velocity.y > 0 && Input.GetButton("Primary Action"))
            sustain = _wolfJumpSustainFactor;
        _velocity += _wolfGravity * Time.deltaTime * sustain;
    }

    void WolfRotation()
    {
        if (_wolfGrounded)
        {
            // Front
            RaycastHit2D hit = Physics2D.Raycast(_wolfFrontRayOrigin.position, Vector2.down, _wolfGroundCheck);
            float angle = (Mathf.Acos(Vector3.Cross(Vector3.down, hit.normal).z) * Mathf.Rad2Deg) - 90;
            _wolfFrontAngle = Mathf.Lerp(_wolfFrontAngle, angle, Time.deltaTime * _wolfGroundRotSmoothFactor);

            for (int i = 0; i < _wolfFrontTransforms.Length; i++)
                _wolfFrontTransforms[i].localEulerAngles = new Vector3(0, 0, _wolfFrontAngle / _wolfFrontTransforms.Length);

            // Back
            hit = Physics2D.Raycast(_wolfFrontRayOrigin.position, Vector2.down, _wolfGroundCheck);
            angle = (Mathf.Acos(Vector3.Cross(Vector3.down, hit.normal).z) * Mathf.Rad2Deg) - 90;
            _wolfBackAngle = Mathf.Lerp(_wolfBackAngle, angle, Time.deltaTime * _wolfGroundRotSmoothFactor);

            for (int i = 0; i < _wolfBackTransforms.Length; i++)
                _wolfBackTransforms[i].localEulerAngles = new Vector3(0, 0, _wolfFrontAngle / _wolfBackTransforms.Length);
        }
        //   _wolfTransform.up = Vector3.Lerp(_wolfTransform.up, hit.normal, Time.deltaTime * _wolfGroundRotSmoothFactor);
        //else
        //    _wolfTransform.up = Vector3.Lerp(_wolfTransform.up, Vector3.up, Time.deltaTime * _wolfAirRotSmoothFactor);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_wolfFrontRayOrigin.position, _wolfFrontRayOrigin.position + Vector3.down * _wolfGroundCheck);
        Gizmos.DrawLine(_wolfBackRayOrigin.position, _wolfBackRayOrigin.position + Vector3.down * _wolfGroundCheck);
    }
}