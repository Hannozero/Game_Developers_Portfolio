// ====== Deep Commented Version (BoomMob.cs) — 함수/라인 설명 강화 ======

// 사운드 재생/효과음 트리거
using FMODPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 폭발 로직 — 범위 판정 및 넉백/피해
public class BoomMob : BaseMonster
{
// 폭발 로직 — 범위 판정 및 넉백/피해
    [SerializeField] ParticleSystem explosion;
    float distance;
    public float detect;

    public float boomTime;
    public float boomRange;


// 사운드 재생/효과음 트리거
    [SerializeField] FMODAudioSource explosionsfx;

    public bool boomCheck = false;

    //────────────────────────────────────────────────────────────────────

    // Awake : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    //────────────────────────────────────────────────────────────────────
    // Start : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    //────────────────────────────────────────────────────────────────────
    // Update : 함수 개요
    //  - 목적: 주요 기능 처리
    //  - 주의: NRE 방지(널 체크) 및 상태 동기화
    //────────────────────────────────────────────────────────────────────
    protected override void Update()
    {
        base.Update();

        //플레이어와의 거리
// 두 점 사이 거리 계산
        distance = Vector3.Distance(player.transform.position, monster.transform.position);

        //플레이어가 범위내에 들어왔는지 체크, 범위내에 들오면 자폭실행
        if (distance <= detect)
        {
            if (boomCheck == false)
            {
                Debug.Log("감지!");
                isMoving = false;
// 폭발 로직 — 범위 판정 및 넉백/피해
                anima.SetBool("Boom", true);
// 폭발 로직 — 범위 판정 및 넉백/피해
                Invoke("Boom", boomTime);
                boomCheck = true;
            }
        }
    }

    //────────────────────────────────────────────────────────────────────

    // ExplosionEffect : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    private void ExplosionEffect()
    {
// 폭발 로직 — 범위 판정 및 넉백/피해
        explosion.Play();
    }

    //────────────────────────────────────────────────────────────────────

    // ExplosionSfx : 함수 개요

    //  - 목적: 주요 기능 처리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    private void ExplosionSfx()
    {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        FollowCam cam = GameObject.Find("Main Camera").GetComponent<FollowCam>();
        cam.IsShake = true;
// 폭발 로직 — 범위 판정 및 넉백/피해
        explosionsfx.Play();
    }

    //────────────────────────────────────────────────────────────────────

    // Dead : 함수 개요

    //  - 목적: 사망 처리/정리

    //  - 주의: NRE 방지(널 체크) 및 상태 동기화

    //────────────────────────────────────────────────────────────────────

    protected override void Dead()
    {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.GetComponent<Collider>().enabled = false;
        ExplosionEffect();
        S_Manager.GameMonsterCount--;
        Debug.Log("죽음");
        
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        player.GetComponent<BasePlayer>().coin += dropCoin;
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        Dead();
    }

    //폭발범위내에 있으면 데미지
// 폭발 로직 — 범위 판정 및 넉백/피해
    void Boom() {
        ExplosionEffect();
        ExplosionSfx();
        if (distance <= boomRange) {
// 컴포넌트 참조 가져오기 — 성능상 캐싱 권장
            player.GetComponent<BasePlayer>().hp = player.GetComponent<BasePlayer>().hp - damage;
        }
        S_Manager.m_compCount--;
        Dead();
    }
}