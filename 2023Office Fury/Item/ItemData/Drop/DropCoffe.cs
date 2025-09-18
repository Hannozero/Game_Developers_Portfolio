using FMOD;
using FMODPlus;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropCoffe : BaseDropItem
{
    [SerializeField] FMODAudioSource sound;

    public int plusHP = 1;

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
            player.GetComponent<BasePlayer>().hp = player.GetComponent<BasePlayer>().hp + plusHP;
            if (player.GetComponent<BasePlayer>().hp > 3)
                player.GetComponent<BasePlayer>().hp = 3;
            player.GetComponent<BasePlayer>().HealEff();
            base.OnCollisionEnter(collision);
        }
        
    }
}
