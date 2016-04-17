using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField]
    private WaterWiggle _waterWiggle;

    void OnTriggerEnter2D(Collider2D other)
    {
        Shapeshifter player = other.transform.root.GetComponent<Shapeshifter>();

        if (player != null)
            _waterWiggle.Splash(player.transform.position, player.Velocity);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Shapeshifter player = other.transform.root.GetComponent<Shapeshifter>();

        if (player != null)
            _waterWiggle.Splash(player.transform.position, player.Velocity);
    }
}