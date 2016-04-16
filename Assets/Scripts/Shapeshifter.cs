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
    private bool _shapeshiftInProgress;
    private float _shapeshiftDuration = 0.5f;

    [SerializeField]
    private AnimationCurve _easeInCurve;
    [SerializeField]
    private AnimationCurve _easeOutCurve;

    // Wolf Stuff
    [SerializeField]
    private Transform _wolfTransform;
    private float _wolfAngle;

    private Vector3 _wolfGravity = new Vector3(0, -60);
    private bool _wolfGrounded;
    private float _wolfGroundCheck = 1.0f;
    private float _wolfHeight = 0.6f;
    private float _wolfJumpSpeed = 14.0f;
    private float _wolfJumpSustainFactor = .4f;
    private float _wolfGroundRotSmoothFactor = 10.0f;
    private float _wolfAirRotSmoothFactor = 5.0f;
    private float _wolfBaseRunSpeed = 14.0f;

    // Bird Stuff
    [SerializeField]
    private Transform _birdTransform;
    private Vector3 _birdGravity = new Vector3(0, -20);
    private Vector3 _birdAscendingSpeed = new Vector3(0, 30);

    // Fish Stuff
    [SerializeField]
    private Transform _fishTransform;

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
                    BirdUpdate();
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

        if (Input.GetButtonDown("Cycle Right"))
            CycleForms();
    }

    void CycleForms()
    {
        Transform shiftingOut = null;

        switch (_form)
        {
            case AnimalForms.Wolf:
                shiftingOut = _wolfTransform;
                break;
            case AnimalForms.Bird:
                shiftingOut = _birdTransform;
                break;
            case AnimalForms.Fish:
                shiftingOut = _fishTransform;
                break;
        }

        if (_shapeshiftInProgress)
            return;

        _form++;
        if ((int)_form >= (int)AnimalForms.Fish)
            _form = 0;

        Transform shiftingIn = null;

        switch (_form)
        {
            case AnimalForms.Wolf:
                shiftingIn = _wolfTransform;
                break;
            case AnimalForms.Bird:
                shiftingIn = _birdTransform;
                break;
            case AnimalForms.Fish:
                shiftingIn = _fishTransform;
                break;
        }

        StartCoroutine(ShapeShiftAsync(shiftingIn, shiftingOut));
    }

    IEnumerator ShapeShiftAsync(Transform shiftingIn, Transform shiftingOut)
    {
        _shapeshiftInProgress = true;
        shiftingIn.gameObject.SetActive(true);

        for (float t = 0; t < _shapeshiftDuration; t += Time.deltaTime)
        {
            float normalT = t / _shapeshiftDuration;

            shiftingIn.localScale = Vector3.one * _easeInCurve.Evaluate(normalT);
            shiftingOut.localScale = Vector3.one * _easeOutCurve.Evaluate(normalT);

            yield return null;
        }

        shiftingOut.gameObject.SetActive(false);
        _shapeshiftInProgress = false;
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
        RaycastHit2D hit = Physics2D.Raycast(_wolfTransform.position, Vector2.down, _wolfGroundCheck);

        if (_wolfGrounded)
            _wolfTransform.up = Vector3.Lerp(_wolfTransform.up, hit.normal, Time.deltaTime * _wolfGroundRotSmoothFactor);
        else
            _wolfTransform.up = Vector3.Lerp(_wolfTransform.up, Vector3.up, Time.deltaTime * _wolfAirRotSmoothFactor);
    }

    void BirdUpdate()
    {
        BirdGravity();
    }

    void BirdGravity()
    {
        if (Input.GetButton("Primary Action"))
            _velocity += _birdAscendingSpeed * Time.deltaTime;
        else
            _velocity += _birdGravity * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wolfTransform.position, _wolfTransform.position + Vector3.down * _wolfGroundCheck);
    }
}