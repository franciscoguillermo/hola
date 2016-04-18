using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField]
    private WaterWiggle _waterWiggle;
    private GameObject _bigSplash;
    private GameObject _smallSplash;
    private float _bigSplashThreshold = 20.0f;

    void Awake()
    {
        _bigSplash = Resources.Load<GameObject>("Pooled/big_splash");
        _smallSplash = Resources.Load<GameObject>("Pooled/small_splash");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Shapeshifter player = other.transform.root.GetComponent<Shapeshifter>();

        if (player != null)
        {
            _waterWiggle.Splash(player.transform.position, player.Velocity);
            SpawnSplashEffect(player.transform.position, player.Velocity.y);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Shapeshifter player = other.transform.root.GetComponent<Shapeshifter>();

        if (player != null)
        {
            _waterWiggle.Splash(player.transform.position, player.Velocity);
            SpawnSplashEffect(player.transform.position, player.Velocity.y);
        }
    }

    void SpawnSplashEffect(Vector3 position, float yVelocity)
    {
        GameObject splash;
        if (Mathf.Abs(yVelocity) > _bigSplashThreshold)
            splash = ObjectPool.Pop(_bigSplash);
        else
            splash = ObjectPool.Pop(_smallSplash);

        splash.SetActive(true);
        splash.transform.position = position;
        splash.GetComponent<ParticleSystem>().Play();

        AudioManager.PlaySplash(Mathf.Abs(yVelocity) / 30f, Random.Range(0.8f, 1.2f));
    }
}