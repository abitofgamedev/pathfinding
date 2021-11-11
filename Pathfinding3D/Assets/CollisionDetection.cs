using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] Collider _Collider;
    IEnumerator Start()
    {
        yield return null;
        _Collider.enabled = true;
    }


    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision Detected From "+gameObject.name+" to " + collision.gameObject.name);
        //Debug.Break();
    }
}
