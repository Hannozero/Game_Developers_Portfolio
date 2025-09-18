using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODPlus;

public class DropCas : BaseDropItem
{
    [SerializeField] FMODAudioSource sound;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            sound.Play();
            player.GetComponent<BasePlayer>().activeGauge = player.GetComponent<BasePlayer>().maxActiveGauge;
            player.GetComponent<BasePlayer>().CasEff();
            base.OnCollisionEnter(collision);
        }
    
    }

}
