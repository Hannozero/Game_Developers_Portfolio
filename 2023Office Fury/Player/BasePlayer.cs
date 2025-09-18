using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODPlus;
using FMODUnity;
using System.Diagnostics.Tracing;
using System;

public class BasePlayer : MonoBehaviour
{
    GameObject player;
    GameObject monster;
    public FMODAudioSource[] sound;
    [SerializeField] GameObject shop;


    bool isDead = true;

    public List<string> animArray;
    public ParticleSystem Heal;
    public ParticleSystem[] SkillBullet;
    public ParticleSystem[] Cas;
    public int index = 0;

    public Animator anima;

    public float hp=3;
    public float fireDamage;
    public float cocktailDamage;
    public float axeDamage;
    public float activeGauge;
    public float Critical;

    public float maxActiveGauge = 100;

    public int coin = 0;

    public bool invincibility = false;
    public float invincibilityTime;

    void Awake()
    {
        shop = GameObject.FindGameObjectWithTag("DataManager");
        SetStat();
        SetItem();
    }
    void SetStat()
    {
        fireDamage = shop.GetComponent<ShopDataManager>().GetDamage;
        this.GetComponent<Movement>().speed = shop.GetComponent<ShopDataManager>().GetSpeed;
        this.GetComponent<FireCtrl>().maxGasGauge = shop.GetComponent<ShopDataManager>().GetAmmo;
        Critical = shop.GetComponent<ShopDataManager>().GetCri;
    }
    void SetItem()
    {
        this.GetComponent<PlayerItem>().activeFB = shop.GetComponent<ShopDataManager>().IsFBActive;
        this.GetComponent<PlayerItem>().activeAxe = shop.GetComponent<ShopDataManager>().IsAxeActive;
        this.GetComponent<PlayerItem>().activeBoom = shop.GetComponent<ShopDataManager>().IsBoomActive;
        this.GetComponent<PlayerItem>().battlecry = shop.GetComponent<ShopDataManager>().IsCryActive;
        if (this.GetComponent<PlayerItem>().activeFB)
        {
            maxActiveGauge = 100;
        }
        else if (this.GetComponent<PlayerItem>().activeAxe)
        {
            maxActiveGauge = 300;
        }
        else if (this.GetComponent<PlayerItem>().activeBoom)
        {
            maxActiveGauge = 150;
        }
        else if (this.GetComponent<PlayerItem>().battlecry)
        {
            maxActiveGauge = 150;
        }
    }
    public float PlayerAttack()
    {
        int temp = UnityEngine.Random.Range(1, 101);
        if (Critical >= temp)
        {
            float dps = fireDamage * 2;
            return dps;
        }
        else
            return fireDamage;
    }
    void Start()
    {
        SkillBullet[0].Stop();
    }

    public void HitRange()
    {
        hp -= 1f;
    }

    void  Update()
    {
        if (hp <= 0) {
            anima.SetBool("Death", true);
            this.gameObject.GetComponent<Movement>().isDead = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (hp > 0)
        {
            if (other.gameObject.tag == "Monster")
            {
                if (invincibility == false)
                {
                    sound[1].Play();
                    invincibility = true;
                    hp--;
                    Invoke("invincibilityClear", invincibilityTime);
                }
            }

            if (other.gameObject.tag == "MonsterBullet")
            {
                if (invincibility == false)
                {
                    sound[1].Play();
                    invincibility = true;
                    hp--;
                    Invoke("invincibilityClear", invincibilityTime);
                }
            }
            FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            cam.IsShake = true;
        }

    }

    void invincibilityClear() {
        invincibility = false;
    }

    public void HealEff() {
        Heal.Play();
    }

    public void SkillBulletEff() {
        SkillBullet[0].Play();
        Invoke("SkillBulletEffOff", 1.0f);
    }
    public void SkillBulletEffOff()
    {
        SkillBullet[0].Stop();
    }

    public void CasEff()
    {
        Cas[0].Play();
        Cas[1].Play();

    }

    public void Deadsound()
    {
        if (isDead)
            sound[0].Play();
        isDead = false;
    }
}
