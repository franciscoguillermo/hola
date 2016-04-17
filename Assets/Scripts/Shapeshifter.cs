using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimalForms
{
    Fox,
    Bird,
    Fish
}

public class Shapeshifter : MonoBehaviour
{
    public Vector3 Velocity
    {
        get
        {
            return _velocity;
        }
    }

    // All Forms Stuff
    private AnimalForms _form = AnimalForms.Fox;
    private Vector3 _velocity;
    private bool _shapeshiftInProgress;
    private float _shapeshiftDuration = 0.5f;
    private float _xVelocitySmoothFactor = 10.0f;
    private float _landXSpeed = 14.0f;
    private float _waterXSpeed = 12.0f;
    [SerializeField]
    private Transform _cameraTarget;
    private float _cameraSmoothFactor = 10.0f;
    private bool _dead = false;
    private bool _underwater = false;
    private float _waterTensionVelocityFactor = 0.5f;
    private float _worldCeil = 4.8f;
    private float _ceilingBounceCoef = 0.5f;
    private float _worldFloor = -20.0f;
    private float _floorBounceCoef = .75f;

    [SerializeField]
    private GameObject _foxDeathEffectPrefab;
    [SerializeField]
    private GameObject _birdDeathEffectPrefab;
    [SerializeField]
    private GameObject _fishDeathEffectPrefab;

    [SerializeField]
    private InkEffect _inkEffect;

    private bool _postShiftInvicibilityActive;
    private float _postShiftInvicibilityDuration = 0.1f;

    [SerializeField]
    private AnimationCurve _easeInCurve;
    [SerializeField]
    private AnimationCurve _easeOutCurve;

    // Fox Stuff
    [SerializeField]
    private Transform _foxTransform;
    [SerializeField]
    private ParticleSystem _foxTail;
    private Vector3 _foxGravity = new Vector3(0, -60);
    private bool _foxGrounded;
    private float _foxGroundCheck = 1.0f;
    private float _foxHeight = 0.6f;
    private float _foxJumpVelocity = 16.0f;
    private float _foxJumpSustainFactor = .6f;
    private float _foxGroundRotSmoothFactor = 10.0f;
    private float _foxAirRotSmoothFactor = 5.0f;
    private Vector3 _foxCameraOffset = new Vector3(10, 3);
    private float _foxCameraFloor = -11;
    private float _foxCameraCeil = -3;

    // Bird Stuff
    [SerializeField]
    private Transform _birdTransform;
    [SerializeField]
    private ParticleSystem _birdTail;
    private Vector3 _birdGravity = new Vector3(0, -20);
    private Vector3 _birdAscendingSpeed = new Vector3(0, 30);
    private float _birdCameraFloor = -11;
    private float _birdCameraCeil = -3;
    private Vector3 _birdCameraOffset = new Vector3(10, -3);

    // Fish Stuff
    [SerializeField]
    private Transform _fishTransform;
    [SerializeField]
    private ParticleSystem _fishTail;
    private Vector3 _fishBouyancy = new Vector3(0, 60);
    private Vector3 _fishGravity = new Vector3(0, -60);
    private Vector3 _fishDivingSpeed = new Vector3(0, -80);
    private Vector3 _fishCameraOffset = new Vector3(10, -4);
    private float _fishCameraFloor = -11;
    private float _fishCameraCeil = -4;
    private float _fishRotSmoothFactor = 10.0f;

    void Update()
    {
        // Reset Test
        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(0);

        if (_dead)
            return;

        switch (_form)
        {
            case AnimalForms.Fox:
                {
                    FoxUpdate();
                    break;
                }
            case AnimalForms.Bird:
                {
                    BirdUpdate();
                    break;
                }
            case AnimalForms.Fish:
                {
                    FishUpdate();
                    break;
                }
        }

        AllFormsUpdate();
    }

    void AllFormsUpdate()
    {
        float xVel = !_underwater ? _landXSpeed : _waterXSpeed;
        _velocity.x = Mathf.Lerp(_velocity.x, xVel, Time.deltaTime * _xVelocitySmoothFactor);

        transform.Translate(_velocity * Time.deltaTime);

        if (Input.GetButtonDown("Cycle Right"))
            CycleForms(1);
        else if (Input.GetButtonDown("Cycle Left"))
            CycleForms(-1);

        CeilingUpdate();
        FloorUpdate();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathTrigger"))
            Die();

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("WaterTrigger"))
        {
            BoxCollider2D box = (BoxCollider2D)other;

            if (transform.position.y < box.transform.position.y + box.size.y / 2)
            {
                if (!_underwater && _velocity.y < 0)
                    _velocity.y *= _waterTensionVelocityFactor;

                _underwater = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WaterTrigger"))
            _underwater = false;
    }

    void Die()
    {
        if (!_postShiftInvicibilityActive && !_dead)
        {
            GameObject prefab = null;

            switch (_form)
            {
                case AnimalForms.Fox:
                    prefab = _foxDeathEffectPrefab;
                    break;
                case AnimalForms.Bird:
                    prefab = _birdDeathEffectPrefab;
                    break;
                case AnimalForms.Fish:
                    prefab = _fishDeathEffectPrefab;
                    break;
            }

            GameObject deathEffect = ObjectPool.Pop(prefab);
            deathEffect.SetActive(true);
            deathEffect.transform.position = transform.position;
            deathEffect.transform.up = _velocity.normalized;
            _dead = true;

            StartCoroutine(DieAsync());
        }
    }

    IEnumerator DieAsync()
    {
        for (float t = 0; t < _shapeshiftDuration; t += Time.deltaTime)
        {
            float normalT = t / _shapeshiftDuration;
            transform.localScale = Vector3.one * _easeOutCurve.Evaluate(normalT);
            yield return null;
        }

        _inkEffect.BleedOut();
    }

    void CycleForms(int dir)
    {
        if (_shapeshiftInProgress)
            return;

        Transform shiftingOut = null;

        switch (_form)
        {
            case AnimalForms.Fox:
                shiftingOut = _foxTransform;
                _foxTail.Stop();
                break;
            case AnimalForms.Bird:
                shiftingOut = _birdTransform;
                _birdTail.Stop();
                break;
            case AnimalForms.Fish:
                shiftingOut = _fishTransform;
                _fishTail.Stop();
                break;
        }

        _form += dir;

        if (_form > AnimalForms.Fish)
            _form = AnimalForms.Fox;
        else if (_form < AnimalForms.Fox)
            _form = AnimalForms.Fish;

        Transform shiftingIn = null;

        switch (_form)
        {
            case AnimalForms.Fox:
                shiftingIn = _foxTransform;
                _foxTail.Play();
                break;
            case AnimalForms.Bird:
                shiftingIn = _birdTransform;
                _birdTail.Play();
                break;
            case AnimalForms.Fish:
                shiftingIn = _fishTransform;
                _fishTail.Play();
                break;
        }

        StartCoroutine(ShapeShiftAsync(shiftingIn, shiftingOut));
    }

    void CeilingUpdate()
    {
        if (transform.position.y > _worldCeil && _velocity.y > 0)
            _velocity.y = -_velocity.y * _ceilingBounceCoef;
    }

    void FloorUpdate()
    {
        if (transform.position.y < _worldFloor && _velocity.y < 0)
        {
            if (_form == AnimalForms.Fish)
                _velocity.y = -_velocity.y * _floorBounceCoef;
            else
                Die();
        }
    }

    IEnumerator ShapeShiftAsync(Transform shiftingIn, Transform shiftingOut)
    {
        _shapeshiftInProgress = true;
        _postShiftInvicibilityActive = true;
        shiftingIn.gameObject.SetActive(true);

        for (float t = 0; t < _shapeshiftDuration; t += Time.deltaTime)
        {
            if (_postShiftInvicibilityActive && t > _postShiftInvicibilityDuration)
                _postShiftInvicibilityActive = false;

            float normalT = t / _shapeshiftDuration;

            shiftingIn.localScale = Vector3.one * _easeInCurve.Evaluate(normalT);
            shiftingOut.localScale = Vector3.one * _easeOutCurve.Evaluate(normalT);

            yield return null;
        }

        shiftingOut.gameObject.SetActive(false);
        _shapeshiftInProgress = false;
    }

    void FoxUpdate()
    {
        FoxGravity();
        FoxCameraTarget();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _foxGroundCheck);
        _foxGrounded = hit.collider != null;

        if (_foxGrounded)
        {
            if (_velocity.y < 0)
            {
                transform.position = (Vector3)hit.point + new Vector3(0, _foxHeight, 0);
                _velocity.y = 0;
            }

            if (Input.GetButtonDown("Primary Action"))
                FoxJump();
        }

        FoxRotation();
    }

    void FoxCameraTarget()
    {
        Vector3 targetPos = _foxTransform.position + _foxCameraOffset;
        targetPos.y = Mathf.Clamp(targetPos.y, _foxCameraFloor, _foxCameraCeil);

        _cameraTarget.position = Vector3.Lerp(_cameraTarget.position, targetPos, Time.deltaTime * _cameraSmoothFactor);
    }

    void FoxJump()
    {
        _foxGrounded = false;
        _velocity.y = _foxJumpVelocity;
    }

    void FoxGravity()
    {
        float sustain = 1.0f;
        if (Input.GetButton("Primary Action"))
            sustain = _foxJumpSustainFactor;
        _velocity += _foxGravity * Time.deltaTime * sustain;
    }

    void FoxRotation()
    {
        RaycastHit2D hit = Physics2D.Raycast(_foxTransform.position, Vector2.down, _foxGroundCheck);

        if (_foxGrounded)
            _foxTransform.up = Vector3.Lerp(_foxTransform.up, hit.normal, Time.deltaTime * _foxGroundRotSmoothFactor);
        else
            _foxTransform.up = Vector3.Lerp(_foxTransform.up, Vector3.up, Time.deltaTime * _foxAirRotSmoothFactor);
    }

    void BirdUpdate()
    {
        BirdGravity();
        BirdGroundDeath();
        FoxCameraTarget();
    }

    void BirdGravity()
    {
        if (Input.GetButton("Primary Action"))
            _velocity += _birdAscendingSpeed * Time.deltaTime;
        else
            _velocity += _birdGravity * Time.deltaTime;
    }

    void BirdGroundDeath()
    {
        if (Physics2D.OverlapPoint(_birdTransform.position) != null)
            Die();
    }

    void BirdCameraTarget()
    {
        Vector3 targetPos = _birdTransform.position + _birdCameraOffset;
        targetPos.y = Mathf.Clamp(targetPos.y, _birdCameraFloor, _birdCameraCeil);

        _cameraTarget.position = Vector3.Lerp(_cameraTarget.position, targetPos, Time.deltaTime * _cameraSmoothFactor);
    }

    void FishUpdate()
    {
        FishBuoyancy();
        FishRotation();
        FishOutOfWater();
        FishCameraTarget();
    }

    void FishBuoyancy()
    {
        if (_underwater)
        {
            if (Input.GetButton("Primary Action"))
                _velocity += _fishDivingSpeed * Time.deltaTime;
            else
                _velocity += _fishBouyancy * Time.deltaTime;
        }
        else
            _velocity += _fishGravity * Time.deltaTime;
    }

    void FishRotation()
    {
        _fishTransform.right = Vector3.Lerp(_fishTransform.right, _velocity.normalized, Time.deltaTime * _fishRotSmoothFactor);
    }

    void FishOutOfWater()
    {
        if (Physics2D.OverlapPoint(_fishTransform.position) != null)
            Die();
    }

    void FishCameraTarget()
    {
        Vector3 targetPos = _fishTransform.position + _fishCameraOffset;
        targetPos.y = Mathf.Clamp(targetPos.y, _fishCameraFloor, _fishCameraCeil);

        _cameraTarget.position = Vector3.Lerp(_cameraTarget.position, targetPos, Time.deltaTime * _cameraSmoothFactor);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_foxTransform.position, _foxTransform.position + Vector3.down * _foxGroundCheck);
    }
}