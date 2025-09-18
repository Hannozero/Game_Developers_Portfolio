using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCast : MonoBehaviour
{
    ParticleSystem system;
    bool firsthit = false;
    // Start is called before the first frame update
    void Start()
    {
        system = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int layerMask = 1 << 8;

        layerMask = ~layerMask;

        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            if(firsthit)
            {
                firsthit = true;
                system.Stop();
                system.Play();
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit : " + hit.distance);
            system.startLifetime = 0.7f * hit.distance;
        }
        else
        {
            firsthit = false;
        }
    }
}
