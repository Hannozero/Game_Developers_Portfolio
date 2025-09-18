using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using FMODPlus;

public class MonsterBullet : MonoBehaviour
{
    
    float Damage = 1;
    [SerializeField] FMODAudioSource hit;
    [SerializeField] MeshRenderer sim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(!hit.isPlaying)
            hit.Play();
        sim.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<SphereCollider>().enabled = false;
        Destroy(gameObject,1.15f);
    }
}
