using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DeathTrigger : MonoBehaviour
{

    void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, box.size);
    }
}