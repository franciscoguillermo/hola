using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RingTrigger : MonoBehaviour
{
    Animator _anim;
    int _isActiveHash = Animator.StringToHash("IsActive");

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _anim.SetBool(_isActiveHash, true);
    }

    public void Spawn(Vector3 position)
    {
        gameObject.SetActive(true);
        _anim.SetBool(_isActiveHash, true);
        transform.position = position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player") && other.transform.position.x < transform.position.x)
        {
            _anim.SetBool(_isActiveHash, false);
            GameController.Instance.CollectRing();
            AudioManager.PlayRingChime();
        }
    }

    void Update()
    {
        if (transform.position.x < CameraFollow.LeftEdge - 5)
            ObjectPool.Push(gameObject);
    }
}