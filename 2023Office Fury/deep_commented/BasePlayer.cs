// ====== Deep Commented Version (BasePlayer.cs) — 함수/라인 설명 강화 ======

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 사운드 재생/효과음 트리거
using FMODPlus;
// 사운드 재생/효과음 트리거
using FMODUnity;
using System.Diagnostics.Tracing;
using System;

public class BasePlayer : MonoBehaviour
{
    GameObject player;
    GameObject monster;
// 사운드 재생/효과음 트리거
    public FMODAudioSource[] sound;
    [SerializeField] GameObject shop;


    bool isDead = true;

    public List<string> animArray;
    public ParticleSystem Heal;
    public ParticleSystem[] SkillBullet;
    public ParticleSystem[] Cas;
    public int index = 0;

    public Animator anima;

// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
    public float hp=3;
    public float fireDamage;
    public float cocktailDamage;
    public float axeDamage;
    public float activeGauge;
    public float Critical;

    public float maxActiveGauge = 100;

    public int coin = 0;

// 무적 프레임 — 연속 피격 방지
    public bool invincibility = false;
// 무적 프레임 — 연속 피격 방지
    public float invincibilityTime;

    void Awake()
    {
        shop = GameObject.FindGameObjectWithTag("DataManager");
        SetStat();
        SetItem();
    }
    void SetStat()
    {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        fireDamage = shop.GetComponent<ShopDataManager>().GetDamage;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<Movement>().speed = shop.GetComponent<ShopDataManager>().GetSpeed;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<FireCtrl>().maxGasGauge = shop.GetComponent<ShopDataManager>().GetAmmo;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        Critical = shop.GetComponent<ShopDataManager>().GetCri;
    }
    void SetItem()
    {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<PlayerItem>().activeFB = shop.GetComponent<ShopDataManager>().IsFBActive;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<PlayerItem>().activeAxe = shop.GetComponent<ShopDataManager>().IsAxeActive;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<PlayerItem>().activeBoom = shop.GetComponent<ShopDataManager>().IsBoomActive;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<PlayerItem>().battlecry = shop.GetComponent<ShopDataManager>().IsCryActive;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        if (this.GetComponent<PlayerItem>().activeFB)
        {
            maxActiveGauge = 100;
        }
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        else if (this.GetComponent<PlayerItem>().activeAxe)
        {
            maxActiveGauge = 300;
        }
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        else if (this.GetComponent<PlayerItem>().activeBoom)
        {
            maxActiveGauge = 150;
        }
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        else if (this.GetComponent<PlayerItem>().battlecry)
        {
            maxActiveGauge = 150;
        }
    }
    //────────────────────────────────────────────────────────────────────
    // PlayerAttack : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    public float PlayerAttack()
    {
// 난수 — 범위 내 무작위값
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

    //────────────────────────────────────────────────────────────────────

    // HitRange : 함수 개요

    //  - 목적: 피해 적용/이펙트

    //  - 주의: 중복 타격/무적 프레임 고려

    //────────────────────────────────────────────────────────────────────

    public void HitRange()
    {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
        hp -= 1f;
    }

    void  Update()
    {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
        if (hp <= 0) {
            anima.SetBool("Death", true);
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            this.gameObject.GetComponent<Movement>().isDead = true;
        }
    }

    //────────────────────────────────────────────────────────────────────

    // OnTriggerStay : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    private void OnTriggerStay(Collider other)
    {
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
        if (hp > 0)
        {
            if (other.gameObject.tag == "Monster")
            {
// 무적 프레임 — 연속 피격 방지
                if (invincibility == false)
                {
                    sound[1].Play();
// 무적 프레임 — 연속 피격 방지
                    invincibility = true;
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
                    hp--;
// 무적 프레임 — 연속 피격 방지
                    Invoke("invincibilityClear", invincibilityTime);
                }
            }

            if (other.gameObject.tag == "MonsterBullet")
            {
// 무적 프레임 — 연속 피격 방지
                if (invincibility == false)
                {
                    sound[1].Play();
// 무적 프레임 — 연속 피격 방지
                    invincibility = true;
// 체력 값 갱신/검사 — 0 이하 시 사망 처리 유의
                    hp--;
// 무적 프레임 — 연속 피격 방지
                    Invoke("invincibilityClear", invincibilityTime);
                }
            }
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
            cam.IsShake = true;
        }

    }

// 무적 프레임 — 연속 피격 방지
    void invincibilityClear() {
// 무적 프레임 — 연속 피격 방지
        invincibility = false;
    }

    //────────────────────────────────────────────────────────────────────

    // HealEff : 함수 개요

    //  - 목적: 회복 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    public void HealEff() {
        Heal.Play();
    }

    //────────────────────────────────────────────────────────────────────

    // SkillBulletEff : 함수 개요

    //  - 목적: 발사/투사체 생성

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    public void SkillBulletEff() {
        SkillBullet[0].Play();
        Invoke("SkillBulletEffOff", 1.0f);
    }
    //────────────────────────────────────────────────────────────────────
    // SkillBulletEffOff : 함수 개요
    //  - 목적: 발사/투사체 생성
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    public void SkillBulletEffOff()
    {
        SkillBullet[0].Stop();
    }

    //────────────────────────────────────────────────────────────────────

    // CasEff : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    public void CasEff()
    {
        Cas[0].Play();
        Cas[1].Play();

    }

    //────────────────────────────────────────────────────────────────────

    // Deadsound : 함수 개요

    //  - 목적: 사망 처리/정리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    public void Deadsound()
    {
        if (isDead)
            sound[0].Play();
        isDead = false;
    }
}